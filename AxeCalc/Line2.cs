// AxeCalc - guitar design software https://github.com/fearog/axecalc
// Copyright (C) 2012 Tristan Williams

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AxeCalc
{
	class Line2
	{
		public Vec2 v1;
		public Vec2 v2;
		public Vec2 origin;
		public Vec2 direction;
		public double distance;

		public Line2( double x1, double y1, double x2, double y2 )
		{
			init( new Vec2( x1, y1 ), new Vec2( x2, y2 ) );
		}

		public Line2( Vec2 _v1, Vec2 _v2 )
		{
			init( new Vec2( _v1.x, _v1.y ), new Vec2( _v2.x, _v2.y ) );
		}

		public Line2( Line2 other ) : this( other.v1, other.v2 )
		{
		}

		public void init( Vec2 _v1, Vec2 _v2 )
		{
			v1 = _v1;
			v2 = _v2;
			origin = v1;
			direction = v2 - v1;
			distance = direction.normalize();
		}

		//def __repr__( self ):
		//	return "v1" + repr( v1 ) + " v2" + repr( v2 ) + " origin" + repr( origin ) + " direction" + repr( direction ) + " distance %.5f" % distance

		public bool intersectInfiniteInternal( Line2 other, ref Vec2 result, ref double[] constants )
		{
			double x1 = v1.x;
			double y1 = v1.y;
			double x2 = v2.x;
			double y2 = v2.y;
			double x3 = other.v1.x;
			double y3 = other.v1.y;
			double x4 = other.v2.x;
			double y4 = other.v2.y;

			double determinant = ( y4 - y3 ) * ( x2 - x1 ) - ( x4 - x3 ) * ( y2 - y1 );
			if( determinant == 0 )
				return false;

			constants[ 0 ] = ( ( x4 - x3 ) * ( y1 - y3 ) - ( y4 - y3 ) * ( x1 - x3 ) ) / determinant;
			constants[ 1 ] = ( ( x2 - x1 ) * ( y1 - y3 ) - ( y2 - y1 ) * ( x1 - x3 ) ) / determinant;

			result.x = x1 + constants[ 0 ] * ( x2 - x1 );
			result.y = y1 + constants[ 0 ] * ( y2 - y1 );

			return true;
		}
    
		public bool intersectInfinite( Line2 other, ref Vec2 result )
		{
			double[] constants = new double[ 2 ];
			return intersectInfiniteInternal( other, ref result, ref constants );
		}
    
		public bool intersectSegment( Line2 other, ref Vec2 result )
		{
			double[] constants = new double[ 2 ];
			if( intersectInfiniteInternal( other, ref result, ref constants ) == false )
				return false;

			if( constants[ 0 ] < 0 || constants[ 0 ] > 1 || constants[ 1 ] < 0 || constants[ 1 ] > 1 )
				return false;

			return true;
		}

		public bool extendToInfinite( Line2 other )
		{
			double[] constants = new double[ 2 ];
			Vec2 result = new Vec2( 0, 0 );
			if( intersectInfiniteInternal( other, ref result, ref constants ) == false )
				return false;

			if( constants[ 0 ] > 1 )
			{
				init( v1, result );
				return true;
			}

			if( constants[ 0 ] < 0 )
			{
				init( result, v2 );
				return true;
			}

			return false;
		}

		public double length() { return distance; }

		public Line2 offset( double distance, Vec2 direction )
		{
			// make a normal vector
			Vec2 normal = new Vec2( -this.direction.y, this.direction.x );

			// make sure its in the same direction as the direction hint vector
			if( normal.dot( direction ) < 0.0 )
				normal = normal * -1;
			
			// now create an offset line
			Vec2 offset = normal * distance;
			return new Line2( v1 + offset, v2 + offset );
		}

		public bool trimToInfinite( Line2 other, Vec2 sideToKeep )
		{
			// trim to or extend to another
			double[] constants = new double[ 2 ];
			Vec2 intersectionPoint = new Vec2( 0, 0 );
			if( intersectInfiniteInternal( other, ref intersectionPoint, ref constants ) == false )
				return false;

			// extending to another
			if( constants[ 0 ] > 1 )
			{
				init( v1, intersectionPoint );
				return true;
			}

			if( constants[ 0 ] < 0 )
			{
				init( intersectionPoint, v2 );
				return true;
			}

			// trimming to another
		    //print( "trimToInfinite gave constant of %.3f" % constants[ 0 ])
			Vec2 deltaToV1 = v1 - intersectionPoint;
			Vec2 deltaToV2 = v2 - intersectionPoint;
			Vec2 deltaToSide = intersectionPoint;
			if( deltaToV1.dot( sideToKeep ) > 0 )
			{
				init( v1, intersectionPoint );
				//print( "   trimmed back in the direction of v1" )
				return true;
			}

			if( deltaToV2.dot( sideToKeep ) > 0 )
			{
				init( intersectionPoint, v2 );
				//print( "   trimmed back in the direction of v2" )
				return true;
			}
			return false;
		}

		public Vec2 projection( Vec2 point )
		{
			// return the projection of point onto this line
			// make a normal vector from the mirror line
			Vec2 normal = new Vec2( -direction.y, direction.x );

			double normalDist = ( point - v1 ).dot( normal );
			return point - normal * normalDist;
		}

		public Line2 mirror( Line2 mirrorLine )
		{
			// project each point of the line onto the mirrorline, use that as a way to construct new points
			Vec2 newv1 = mirrorLine.projection( v1 )*2 - v1;
			Vec2 newv2 = mirrorLine.projection( v2 )*2 - v2;
			return new Line2( newv1, newv2 );
		}

		public Line2 perpendicular( Vec2 direction )
		{
			// return a normal vector from this line, generally pointing in the direction provided
			Vec2 normal = new Vec2( -direction.y, direction.x );

			double directionality = normal.dot( direction );
			if( directionality >= 0 )
				return new Line2( v1, v1 + normal * distance );
			else
				return new Line2( v1, v1 - normal * distance );
		}
	}
}
