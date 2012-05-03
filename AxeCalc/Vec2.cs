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
	class Vec2
	{
		public static Vec2 left = new Vec2( -1, 0 );
		public static Vec2 right = new Vec2( 1, 0 );
		public static Vec2 up = new Vec2( 0, 1 );
		public static Vec2 down = new Vec2( 0, -1 );

		public double x;
		public double y;

		public Vec2()
		{
			x = 0;
			y = 0;
		}

		public Vec2( double _x, double _y )
		{
			x = _x;
			y = _y;
		}

		public static Vec2 operator + ( Vec2 a, Vec2 b )
		{
			return new Vec2( a.x + b.x, a.y + b.y );
		}
        
		public static Vec2 operator - ( Vec2 a, Vec2 b )
		{
			return new Vec2( a.x - b.x, a.y - b.y );
		}

		public static Vec2 operator *( Vec2 a, double b )
		{
			return new Vec2( a.x * b, a.y * b );
		}

		//def __repr__( self ):
		//	return "( %.5f, %.5f)" % ( self.x, self.y )

		public double length()
		{
			return Math.Sqrt( x * x + y * y );
		}

		public double normalize()
		{
			double myLength = length();
			if( myLength > 0 )
			{
				x /= myLength;
				y /= myLength;
			}
			return myLength;
		}

		public double distanceTo( Vec2 other )
		{
			return ( other - this ).length();
		}

		public double dot( Vec2 other )
		{
			return x * other.x + y * other.y;
		}

		public double angleBetween( Vec2 other )
		{
			return Math.Acos( dot( other ) );
		}
	}
}
