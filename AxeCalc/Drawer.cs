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
using System.Drawing;
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Tables;

namespace AxeCalc
{
	interface Drawer
	{
		void AddLayer( string strName, Color c );
		void SetLimits( double fMinX, double fMinY, double fMaxX, double fMaxY );
		void DrawLine( string strLayer, double x1, double y1, double x2, double y2 );
		void DrawLine( string strLayer, Vec2 a, Vec2 b );
		void DrawLine( string strLayer, Line2 l );
		void DrawCircle( string strLayer, Vec2 vCentre, double dRadius );
		void DrawArc( string strLayer, Vec2 vCentre, double dRadius, double dStartAngle, double dEndAngle );
		void DrawEllipseArc( string strLayer, Vec2 majorAxisV1, Vec2 majorAxisV2, double minorAxisFactor, Vec2 vArcStart, Vec2 vArcEnd );

		void DrawPrefab( Vec2 vPosition, DxfDocument prefab );
	}

	class GraphicsDrawer : Drawer
	{
		Graphics m_g = null;
		RectangleF m_rect;
		Vec2 m_vScroll = new Vec2();
		double m_dZoom = 1.0;

		Dictionary<string, Pen> m_pens = new Dictionary<string, Pen>();
		double m_fXOffset = 0.0;
		double m_fYOffset = 0.0;
		double m_fXScale = 1.0;
		double m_fYScale = 1.0;

		public GraphicsDrawer( RectangleF rect )
		{
			m_rect = rect;
		}

		public void StartFrame( System.Drawing.Graphics g )
		{
			m_g = g;
		}

		public void EndFrame()
		{
			m_g = null;
		}

		public void ResetView( Vec2 vCentre, double dZoom )
		{
			m_vScroll.x = -vCentre.x;
			m_vScroll.y = vCentre.y;
			m_dZoom = dZoom;
		}

		public void Scroll( Vec2 vUIOffset )
		{
			m_vScroll.x += vUIOffset.x / ( m_fXScale * m_dZoom );
			m_vScroll.y -= vUIOffset.y / ( m_fYScale * m_dZoom );
		}

		public void Zoom( double dFactor, Vec2 vUIFocus )
		{
			// zoom so that the focus stays stationary
			// so we need to scroll while we zoom, in effect
			double dOldMSFocusX = uncalcX( vUIFocus.x );
			double dOldMSFocusY = uncalcY( vUIFocus.y );
			//m_vScroll.x += ( vUIFocus.x - m_rect.Width / 2 ) * m_fXScale;
			m_dZoom *= dFactor;
			if( m_dZoom < 0.000001 )
				m_dZoom = 0.000001;
			else if( m_dZoom > 100000 )
				m_dZoom = 100000;
			double dMSFocusX = uncalcX( vUIFocus.x );
			double dMSFocusY = uncalcY( vUIFocus.y );

			m_vScroll.x -= ( dOldMSFocusX - dMSFocusX );
			m_vScroll.y += ( dOldMSFocusY - dMSFocusY );
		}

		public void SetUIRect( RectangleF rect )
		{
			m_rect = rect;
		}

		public void AddLayer( string strName, Color c )
		{
			if( m_pens.ContainsKey( strName ) )
				m_pens[ strName ].Color = c;
			else
				m_pens.Add( strName, new Pen( c ) );
		}

		float calcX( double x ) { return ( float )( ( x + m_vScroll.x ) * m_fXScale * m_dZoom + m_fXOffset ); }
		float calcY( double y ) { return ( float )( ( y - m_vScroll.y ) * m_fYScale * m_dZoom + m_fYOffset ); }
		double uncalcX( double x ) { return ( x - m_fXOffset ) / (m_fXScale * m_dZoom) - m_vScroll.x; }
		double uncalcY( double y ) { return ( y - m_fYOffset ) / (m_fYScale * m_dZoom) + m_vScroll.y; }

		public void SetLimits( double fMinX, double fMinY, double fMaxX, double fMaxY )
		{
			// m_vScroll represents where the centre is in model space
			// that should become the centre of the screen
			double fXScale = m_rect.Width / ( fMaxX - fMinX );
			double fYScale = m_rect.Height / ( fMaxY - fMinY );
			// Use same scale for both X & Y, but flip the Y axis
			m_fXScale = Math.Min( fXScale, fYScale );
			m_fYScale = -m_fXScale;
			m_fXOffset = ( m_rect.Left + m_rect.Right ) * 0.5f;
			m_fYOffset = ( m_rect.Bottom + m_rect.Top ) * 0.5f;
		}

		public void DrawLine( string strLayer, double x1, double y1, double x2, double y2 )
		{
			try
			{
				m_g.DrawLine( m_pens[ strLayer ], calcX( x1 ), calcY( y1 ), calcX( x2 ), calcY( y2 ) );
			}
			catch
			{
			}
		}

		public void DrawLine( string strLayer, Vec2 a, Vec2 b )
		{
			DrawLine( strLayer, a.x, a.y, b.x, b.y );
		}

		public void DrawLine( string strLayer, Line2 l )
		{
			DrawLine( strLayer, l.v1.x, l.v1.y, l.v2.x, l.v2.y );
		}

		public void DrawCircle( string strLayer, Vec2 vCentre, double dRadius )
		{
			try
			{
				double left = calcX( vCentre.x - dRadius );
				double top = calcY( vCentre.y + dRadius );
				double right = calcX( vCentre.x + dRadius );
				double bottom = calcY( vCentre.y - dRadius );
				RectangleF rect = new RectangleF( ( float )left, ( float )top, ( float )( right - left ), ( float )( bottom - top ) );
				m_g.DrawEllipse( m_pens[ strLayer ], rect );
			}
			catch
			{
			}
		}

		double angleDiff( double dStartAngle, double dEndAngle )
		{
			// normalize both to be -180 to 180
			while( dStartAngle > 180 )
				dStartAngle -= 360;
			while( dEndAngle > 180 )
				dEndAngle -= 360;
			while( dStartAngle < -180 )
				dStartAngle += 360;
			while( dEndAngle < -180 )
				dEndAngle += 360;

			double dDiff = dEndAngle - dStartAngle;
			while( dDiff < 0 )
				dDiff += 360;
			return dDiff;
		}

		public void DrawArc( string strLayer, Vec2 vCentre, double dRadius, double dStartAngle, double dEndAngle )
		{
			//dEndAngle = 270;
			try
			{
				double left = calcX( vCentre.x - dRadius );
				double top = calcY( vCentre.y + dRadius );
				double right = calcX( vCentre.x + dRadius );
				double bottom = calcY( vCentre.y - dRadius );
				RectangleF rect = new RectangleF( ( float )left, ( float )top, ( float )( right - left ), ( float )( bottom - top ) );
				m_g.DrawArc( m_pens[ strLayer ], rect, -( float )dStartAngle, -( float )angleDiff( dStartAngle, dEndAngle ) );
			}
			catch( System.Exception e )
			{
				System.Diagnostics.Debug.WriteLine( "Exception: " + e.Message );
			}
		}

		public void DrawEllipseArc( string strLayer, Vec2 majorAxisV1, Vec2 majorAxisV2, double minorAxisFactor, Vec2 vArcStart, Vec2 vArcEnd )
		{
			//RectangleF rect;
			//rect.Left = Math.Min( majorAxisV1.x
			//m_g.DrawArc( m_pens[ strLayer ] 
		}

		public void DrawPrefab( Vec2 vPosition, DxfDocument prefab )
		{
			foreach( Line line in prefab.Lines )
				DrawLine( line.Layer.Name, line.StartPoint.X + vPosition.x, line.StartPoint.Y + vPosition.y, line.EndPoint.X + vPosition.x, line.EndPoint.Y + vPosition.y );

			foreach( Circle circle in prefab.Circles )
				DrawCircle( circle.Layer.Name, new Vec2( circle.Center.X + vPosition.x, circle.Center.Y + vPosition.y ), circle.Radius );

			foreach( Arc arc in prefab.Arcs )
				DrawArc( arc.Layer.Name, new Vec2( arc.Center.X + vPosition.x, arc.Center.Y + vPosition.y ), arc.Radius, arc.StartAngle, arc.EndAngle );
		}
	}

	class DXFDrawer : Drawer, IDisposable
	{
		System.IO.StreamWriter m_out;
		double m_dMinX;
		double m_dMaxX;
		double m_dMinY;
		double m_dMaxY;
		Dictionary<string, Color> m_layers;
		DXFPalette m_palette;
		int m_iEntityNumber;

		public DXFDrawer( string strFilename )
		{
			m_out = new System.IO.StreamWriter( strFilename );
			//m_out.Write( "  0\nSECTION\n  2\nHEADER\n  9\n$ACADVER\n  1\nAC1018\n  0\nENDSEC\n" );
			//m_out.Write( "  0\nSECTION\n  2\nHEADER\n  9\n$ACADVER\n  1\nAC1015\n  9\n$ACADMAINTVER\n 70\n     6\n  9\n$DWGCODEPAGE\n  3\nANSI_1252\n  9\n$INSBASE\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$EXTMIN\n 10\n-30.0\n 20\n-55.925\n 30\n0.0\n  9\n$EXTMAX\n 10\n660.0\n 20\n55.925\n 30\n0.0\n  9\n$LIMMIN\n 10\n0.0\n 20\n0.0\n  9\n$LIMMAX\n 10\n12.0\n 20\n9.0\n  9\n$ORTHOMODE\n 70\n     0\n  9\n$REGENMODE\n 70\n     1\n  9\n$FILLMODE\n 70\n     1\n  9\n$QTEXTMODE\n 70\n     0\n  9\n$MIRRTEXT\n 70\n     1\n  9\n$LTSCALE\n 40\n1.0\n  9\n$ATTMODE\n 70\n     1\n  9\n$TEXTSIZE\n 40\n0.2\n  9\n$TRACEWID\n 40\n0.05\n  9\n$TEXTSTYLE\n  7\nStandard\n  9\n$CLAYER\n  8\n0  9\n$CELTYPE\n  6\nByBlock\n  9\n$CECOLOR\n 62\n   256\n  9\n$CELTSCALE\n 40\n1.0\n  9\n$DISPSILH\n 70\n     0\n  9\n$DIMSCALE\n 40\n1.0\n  9\n$DIMASZ\n 40\n0.18\n  9\n$DIMEXO\n 40\n0.0625\n  9\n$DIMDLI\n 40\n0.38\n  9\n$DIMRND\n 40\n0.0\n  9\n$DIMDLE\n 40\n0.0\n  9\n$DIMEXE\n 40\n0.18\n  9\n$DIMTP\n 40\n0.0\n  9\n$DIMTM\n 40\n0.0\n  9\n$DIMTXT\n 40\n0.18\n  9\n$DIMCEN\n 40\n0.09\n  9\n$DIMTSZ\n 40\n0.0\n  9\n$DIMTOL\n 70\n     0\n  9\n$DIMLIM\n 70\n     0\n  9\n$DIMTIH\n 70\n     1\n  9\n$DIMTOH\n 70\n     1\n  9\n$DIMSE1\n 70\n     0\n  9\n$DIMSE2\n 70\n     0\n  9\n$DIMTAD\n 70\n     0\n  9\n$DIMZIN\n 70\n     0\n  9\n$DIMBLK\n  1\n\n\n  9\n$DIMASO\n 70\n     1\n  9\n$DIMSHO\n 70\n     1\n  9\n$DIMPOST\n  1\n\n\n  9\n$DIMAPOST\n  1\n\n\n  9\n$DIMALT\n 70\n     0\n  9\n$DIMALTD\n 70\n     2\n  9\n$DIMALTF\n 40\n25.4\n  9\n$DIMLFAC\n 40\n1.0\n  9\n$DIMTOFL\n 70\n     0\n  9\n$DIMTVP\n 40\n0.0\n  9\n$DIMTIX\n 70\n     0\n  9\n$DIMSOXD\n 70\n     0\n  9\n$DIMSAH\n 70\n     0\n  9\n$DIMBLK1\n  1\n\n\n  9\n$DIMBLK2\n  1\n\nn  9\n\nDIMSTYLE\n  2\nStandard\n  9\n$DIMCLRD\n 70\n     0\n  9\n$DIMCLRE\n 70\n     0\n  9\n$DIMCLRT\n 70\n     0\n  9\n$DIMTFAC\n 40\n1.0\n  9\n$DIMGAP\n 40\n0.09\n  9\n$DIMJUST\n 70\n     0\n  9\n$DIMSD1\n 70\n     0\n  9\n$DIMSD2\n 70\n     0\n  9\n$DIMTOLJ\n 70\n     1\n  9\n$DIMTZIN\n 70\n     0\n  9\n$DIMALTZ\n 70\n     0\n  9\n$DIMALTTZ\n 70\n     0\n  9\n$DIMUPT\n 70\n     0\n  9\n$DIMDEC\n 70\n     4\n  9\n$DIMTDEC\n 70\n     4\n  9\n$DIMALTU\n 70\n     2\n  9\n$DIMALTTD\n 70\n     2\n  9\n$DIMTXSTY\n  7\nStandard\n  9\n$DIMAUNIT\n 70\n     0\n  9\n$DIMADEC\n 70\n     0\n  9\n$DIMALTRND\n 40\n0.0\n  9\n$DIMAZIN\n 70\n     0\n  9\n$DIMDSEP\n 70\n    46\n  9\n$DIMATFIT\n 70\n     3\n  9\n$DIMFRAC\n 70\n     0\n  9\n$DIMLDRBLK\n  1\n\n\n  9\n$DIMLUNIT\n 70\n     2\n  9\n$DIMLWD\n 70\n    -2\n  9\n$DIMLWE\n 70\n    -2\n  9\n$DIMTMOVE\n 70\n     2\n  9\n$LUNITS\n 70\n     2\n  9\n$LUPREC\n 70\n     4\n  9\n$SKETCHINC\n 40\n0.1\n  9\n$FILLETRAD\n 40\n0.5\n  9\n$AUNITS\n 70\n     0\n  9\n$AUPREC\n 70\n     0\n  9\n\nENU\n  1\n.\n  9\n$ELEVATION\n 40\n0.0\n  9\n$PELEVATION\n 40\n0.0\n  9\n$THICKNESS\n 40\n0.0\n  9\n$LIMCHECK\n 70\n     0\n  9\n$CHAMFERA\n 40\n0.5\n  9\n$CHAMFERB\n 40\n0.5\n  9\n$CHAMFERC\n 40\n1.0\n  9\n$CHAMFERD\n 40\n0.0\n  9\n$SKPOLY\n 70\n     0\n  9\n$TDCREATE\n 40\n2456040.944910336\n  9\n$TDUCREATE\n 40\n2456040.819910336\n  9\n$TDUPDATE\n 40\n2456040.944910336\n  9\n$TDUUPDATE\n 40\n2456040.819910336\n  9\n$TDINDWG\n 40\n0.000092581\n  9\n$TDUSRTIMER\n 40\n0.000092581\n  9\n$USRTIMER\n 70\n     1\n  9\n$ANGBASE\n 50\n0.0\n  9\n$ANGDIR\n 70\n     0\n  9\n$PDMODE\n 70\n     0\n  9\n$PDSIZE\n 40\n0.0\n  9\n$PLINEWID\n 40\n0.0\n  9\n$SPLFRAME\n 70\n     0\n  9\n$SPLINETYPE\n 70\n     6\n  9\n$SPLINESEGS\n 70\n     8\n  9\n$HANDSEED\n  5\n17C\n  9\n$SURFTAB1\n 70\n     6\n  9\n$SURFTAB2\n 70\n     6\n  9\n$SURFTYPE\n 70\n     6\n  9\n$SURFU\n 70\n     6\n  9\n$SURFV\n 70\n     6\n  9\n$UCSBASE\n  2\n\n\n  9\n$UCSNAME\n  2\n\n\n  9\n$UCSORG\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$UCSXDIR\n 10\n1.0\n 20\n0.0\n\n0\n0.0\n  9\n$UCSYDIR\n 10\n0.0\n 20\n1.0\n 30\n0.0\n  9\n$UCSORTHOREF\n  2\n\n\n  9\n$UCSORTHOVIEW\n 70\n     0\n  9\n$UCSORGTOP\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$UCSORGBOTTOM\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$UCSORGLEFT\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$UCSORGRIGHT\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$UCSORGFRONT\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$UCSORGBACK\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$PUCSBASE\n  2\n\n\n  9\n$PUCSNAME\n  2\n\n\n  9\n$PUCSORG\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$PUCSXDIR\n 10\n1.0\n 20\n0.0\n 30\n0.0\n  9\n$PUCSYDIR\n 10\n0.0\n 20\n1.0\n 30\n0.0\n  9\n$PUCSORTHOREF\n  2\n\n\n  9\n$PUCSORTHOVIEW\n 70\n     0\n  9\n$PUCSORGTOP\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$PUCSORGBOTTOM\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$PUCSORGLEFT\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$PUCSORGRIGHT\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$PUCSORGFRONT\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$PUCSORGBACK\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$USERI1\n 70\n     0\n  9\n$USERI2\n 70\n     0\n  9\n$USERI3\n 70\n     0\n\n9\n$USERI4\n 70\n     0\n  9\n$USERI5\n 70\n     0\n  9\n$USERR1\n 40\n0.0\n  9\n$USERR2\n 40\n0.0\n  9\n$USERR3\n 40\n0.0\n  9\n$USERR4\n 40\n0.0\n  9\n$USERR5\n 40\n0.0\n  9\n$WORLDVIEW\n 70\n     1\n  9\n$SHADEDGE\n 70\n     3\n  9\n$SHADEDIF\n 70\n    70\n  9\n$TILEMODE\n 70\n     1\n  9\n$MAXACTVP\n 70\n    64\n  9\n$PINSBASE\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  9\n$PLIMCHECK\n 70\n     0\n  9\n$PEXTMIN\n 10\n1.000000000000000E+20\n 20\n1.000000000000000E+20\n 30\n1.000000000000000E+20\n  9\n$PEXTMAX\n 10\n-1.000000000000000E+20\n 20\n-1.000000000000000E+20\n 30\n-1.000000000000000E+20\n  9\n$PLIMMIN\n 10\n0.0\n 20\n0.0\n  9\n$PLIMMAX\n 10\n12.0\n 20\n9.0\n  9\n$UNITMODE\n 70\n     0\n  9\n$VISRETAIN\n 70\n     1\n  9\n$PLINEGEN\n 70\n     0\n  9\n$PSLTSCALE\n 70\n     1\n  9\n$TREEDEPTH\n 70\n  3020\n  9\n$CMLSTYLE\n  2\nStandard\n  9\n$CMLJUST\n 70\n     0\n  9\n$CMLSCALE\n 40\n1.0\n  9\n$PROXYGRAPHICS\n 70\n     1\n  9\n$MEASUREMENT\n 70\n     0\n  9\n$CELWEIGHT\n370\n    -1\n  9\n$ENDCAPS\n280\n     0\n\n9\n$JOINSTYLE\n280\n     0\n  9\n$LWDISPLAY\n290\n     0\n  9\n$INSUNITS\n 70\n     0\n  9\n$HYPERLINKBASE\n  1\n\n\n  9\n$STYLESHEET\n  1\n\n\n  9\n$XEDIT\n290\n     1\n  9\n$CEPSNTYPE\n380\n     0\n  9\n$PSTYLEMODE\n290\n     1\n  9\n$FINGERPRINTGUID\n  2\n{43CF4C46-F6FC-45E0-8811-1562E0433FD5}\n  9\n$VERSIONGUID\n  2\n{FAEB1C32-E019-11D5-929B-00C0DF256EC4}\n  9\n$EXTNAMES\n290\n     1\n  9\n$PSVPSCALE\n 40\n0.0\n  9\n$OLESTARTUP\n290\n     0\n  0\nENDSEC\n  0\nSECTION\n  2\nCLASSES\n  0\nCLASS\n  1\nACDBDICTIONARYWDFLT\n  2\nAcDbDictionaryWithDefault\n  3\nObjectDBX Classes\n 90\n        0\n280\n     0\n281\n     0\n  0\nCLASS\n  1\nACDBPLACEHOLDER\n  2\nAcDbPlaceHolder\n  3\nObjectDBX Classes\n 90\n        0\n280\n     0\n281\n     0\n  0\nCLASS\n  1\nLAYOUT\n  2\nAcDbLayout\n  3\nObjectDBX Classes\n 90\n        0\n280\n     0\n281\n     0\n  0\nCLASS\n  1\nDICTIONARYVAR\n  2\nAcDbDictionaryVar\n  3\nObjectDBX Classes\n 90\n        0\n280\n     0\n281\n     0\n  0\nCLASS\n  1\nTABLESTYLE\n  2\nAcDbTableStyle\n\n3\nObjectDBX Classes\n 90\n     4095\n280\n     0\n281\n     0\n  0\nCLASS\n  1\nSCALE\n  2\nAcDbScale\n  3\nObjectDBX Classes\n 90\n     1153\n280\n     0\n281\n     0\n  0\nENDSEC\n  0\nSECTION\n  2\nTABLES\n  0\nTABLE\n  2\nVPORT\n  5\n8\n330\n0\n100\nAcDbSymbolTable\n 70\n     2\n  0\nVPORT\n  5\n97\n330\n8\n100\nAcDbSymbolTableRecord\n100\nAcDbViewportTableRecord\n  2\n*Active\n 70\n     0\n 10\n0.0\n 20\n0.0\n 11\n1.0\n 21\n1.0\n 12\n315.0\n 22\n0.0\n 13\n0.0\n 23\n0.0\n 14\n0.5\n 24\n0.5\n 15\n0.5\n 25\n0.5\n 16\n0.0\n 26\n0.0\n 36\n1.0\n 17\n0.0\n 27\n0.0\n 37\n0.0\n 40\n345.0\n 41\n2.0\n 42\n50.0\n 43\n0.0\n 44\n0.0\n 50\n0.0\n 51\n0.0\n 71\n     0\n 72\n   100\n 73\n     1\n 74\n     3\n 75\n     0\n 76\n     0\n 77\n     0\n 78\n     0\n281\n     0\n 65\n     1\n110\n0.0\n120\n0.0\n130\n0.0\n111\n1.0\n121\n0.0\n131\n0.0\n112\n0.0\n122\n1.0\n132\n0.0\n 79\n     0\n146\n0.0\n  0\nENDTAB\n  0\nTABLE\n  2\nLTYPE\n  5\n5\n330\n0\n100\nAcDbSymbolTable\n 70\n     1\n  0\nLTYPE\n  5\n14\n330\n5\n100\nAcDbSymbolTableRecord\n100\nAcDbLinetypeTableRecord\n  2\nByBlock\n 70\n     0\n  3\n\n\n 72\n    65\n 73\n     0\n 40\n0.0\n  0\nLTYPE\n  5\n15\n330\n5\n100\nAcDbSymbolTableRecord\n100\nAcDbLinetypeTableRecord\n  2\n\nyLayer\n 70\n     0\n  3\n\n\n 72\n    65\n 73\n     0\n 40\n0.0\n  0\nLTYPE\n  5\n16\n330\n5\n100\nAcDbSymbolTableRecord\n100\nAcDbLinetypeTableRecord\n  2\nContinuous\n 70\n     0\n  3\nSolid line\n 72\n    65\n 73\n     0\n 40\n0.0\n  0\nENDTAB\n  0\nTABLE\n  2\nLAYER\n  5\n2\n330\n0\n100\nAcDbSymbolTable\n 70\n     8\n  0\nLAYER\n  5\n10\n330\n2\n100\nAcDbSymbolTableRecord\n100\nAcDbLayerTableRecord\n  2\n0\n 70\n     0\n 62\n     7\n  6\nContinuous\n370\n    -3\n390\nF\n  0\nLAYER\n  5\n53\n330\n2\n100\nAcDbSymbolTableRecord\n100\nAcDbLayerTableRecord\n  2\nScale\n 70\n     0\n 62\n     7\n  6\nContinuous\n370\n    -3\n390\nF\n  0\nLAYER\n  5\n145\n330\n2\n100\nAcDbSymbolTableRecord\n100\nAcDbLayerTableRecord\n  2\nFretCentres\n 70\n     0\n 62\n   132\n  6\nContinuous\n370\n    -3\n390\nF\n  0\nLAYER\n  5\n146\n330\n2\n100\nAcDbSymbolTableRecord\n100\nAcDbLayerTableRecord\n  2\nFretwire\n 70\n     0\n 62\n     9\n  6\nContinuous\n370\n    -3\n390\nF\n  0\nLAYER\n  5\n147\n330\n2\n100\nAcDbSymbolTableRecord\n100\nAcDbLayerTableRecord\n  2\nStringCenterLines\n 70\n     0\n\n62\n     7\n  6\nContinuous\n370\n    -3\n390\nF\n  0\nLAYER\n  5\n148\n330\n2\n100\nAcDbSymbolTableRecord\n100\nAcDbLayerTableRecord\n  2\nStrings\n 70\n     0\n 62\n     7\n  6\nContinuous\n370\n    -3\n390\nF\n  0\nLAYER\n  5\n149\n330\n2\n100\nAcDbSymbolTableRecord\n100\nAcDbLayerTableRecord\n  2\nFretBoard\n 70\n     0\n 62\n     7\n  6\nContinuous\n370\n    -3\n390\nF\n  0\nLAYER\n  5\n14A\n330\n2\n100\nAcDbSymbolTableRecord\n100\nAcDbLayerTableRecord\n  2\nSaddles\n 70\n     0\n 62\n     2\n  6\nContinuous\n370\n    -3\n390\nF\n  0\nENDTAB\n  0\nTABLE\n  2\nSTYLE\n  5\n3\n330\n0\n100\nAcDbSymbolTable\n 70\n     1\n  0\nSTYLE\n  5\n11\n330\n3\n100\nAcDbSymbolTableRecord\n100\nAcDbTextStyleTableRecord\n  2\nStandard\n 70\n     0\n 40\n0.0\n 41\n1.0\n 50\n0.0\n 71\n     0\n 42\n0.2\n  3\ntxt\n  4\n\n\n  0\nENDTAB\n  0\nTABLE\n  2\nVIEW\n  5\n6\n330\n0\n100\nAcDbSymbolTable\n 70\n     0\n  0\nENDTAB\n  0\nTABLE\n  2\nUCS\n  5\n7\n330\n0\n100\nAcDbSymbolTable\n 70\n     0\n  0\nENDTAB\n  0\nTABLE\n  2\nAPPID\n\n5\n9\n330\n0\n100\nAcDbSymbolTable\n 70\n     1\n  0\nAPPID\n  5\n12\n330\n9\n100\nAcDbSymbolTableRecord\n100\nAcDbRegAppTableRecord\n  2\nACAD\n 70\n     0\n  0\nENDTAB\n  0\nTABLE\n  2\nDIMSTYLE\n  5\nA\n330\n0\n100\nAcDbSymbolTable\n 70\n     1\n100\nAcDbDimStyleTable\n  0\nDIMSTYLE\n105\n27\n330\nA\n100\nAcDbSymbolTableRecord\n100\nAcDbDimStyleTableRecord\n  2\nStandard\n 70\n     0\n340\n11\n  0\nENDTAB\n  0\nTABLE\n  2\nBLOCK_RECORD\n  5\n1\n330\n0\n100\nAcDbSymbolTable\n 70\n     0\n  0\nBLOCK_RECORD\n  5\n1F\n330\n1\n100\nAcDbSymbolTableRecord\n100\nAcDbBlockTableRecord\n  2\n*Model_Space\n340\n22\n  0\nBLOCK_RECORD\n  5\n73\n330\n1\n100\nAcDbSymbolTableRecord\n100\nAcDbBlockTableRecord\n  2\n*Paper_Space\n340\n74\n  0\nENDTAB\n  0\nENDSEC\n  0\nSECTION\n  2\nBLOCKS\n  0\nBLOCK\n  5\n20\n330\n1F\n100\nAcDbEntity\n  8\n0\n100\nAcDbBlockBegin\n  2\n*Model_Space\n 70\n     0\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  3\n*Model_Space\n  1\n\n\n  0\nENDBLK\n  5\n21\n330\n1F\n100\nAcDbEntity\n  8\n0\n100\nAcDbBlockEnd\n\n 0\nBLOCK\n  5\n75\n330\n73\n100\nAcDbEntity\n 67\n     1\n  8\n0\n100\nAcDbBlockBegin\n  2\n*Paper_Space\n 70\n     0\n 10\n0.0\n 20\n0.0\n 30\n0.0\n  3\n*Paper_Space\n  1\n\n\n  0\nENDBLK\n  5\n76\n330\n73\n100\nAcDbEntity\n 67\n     1\n  8\n0\n100\nAcDbBlockEnd\n  0\nENDSEC\n" );
			m_out.Write( "  0\nSECTION\n  2\nENTITIES\n" );
			m_layers = new Dictionary<string, Color>();
			m_palette = new DXFPalette();
			m_iEntityNumber = 0xf0;
			AddLayer( "0", Color.FromArgb( 255, 255, 255, 255 ) );
		}

		public void AddLayer( string strName, Color c )
		{
			m_layers.Add( strName, c );
		}

		public void Dispose()
		{
			m_out.Write( "  0\nENDSEC\n  0\n" );

			// write the viewport - mostly this is just C&P from a DXF file.
			m_out.Write( "SECTION\n  2\nTABLES\n  0\nTABLE\n  2\nVPORT\n  5\n8\n330\n0\n100\nAcDbSymbolTable\n 70\n     2\n  0\nVPORT\n  5\n97\n330\n8\n100\nAcDbSymbolTableRecord\n100\nAcDbViewportTableRecord\n  2\n*Active\n 70\n     0\n 10\n0.0\n 20\n0.0\n 11\n1.0\n 21\n1.0\n" );
//			m_out.Write( " 12\n314.1470437295359\n" ); // X value of view center point (in DCS)
//			m_out.Write( " 22\n3.661491764138873\n" ); // Y value of view center point (in DCS)
			m_out.Write( " 12\n" + ( ( m_dMinX + m_dMaxX ) * 0.5 ).ToString() + "\n" ); // X value of view center point (in DCS)
			m_out.Write( " 22\n" + ( ( m_dMinY + m_dMaxY ) * 0.5 ).ToString() + "\n" ); // X value of view center point (in DCS)
			m_out.Write( " 13\n0.0\n 23\n0.0\n 14\n0.5\n 24\n0.5\n 15\n0.5\n 25\n0.5\n 16\n0.0\n 26\n0.0\n 36\n1.0\n 17\n0.0\n 27\n0.0\n 37\n0.0\n" );
			//m_out.Write( " 40\n381.4799349159796\n" ); // View height
			m_out.Write( " 40\n" + ( ( m_dMaxY - m_dMinY ) * 0.5 ).ToString() + "\n" ); // View height
			//m_out.Write( " 41\n1.972972972850329\n 42\n50.0\n 43\n0.0\n 44\n0.0\n 50\n0.0\n 51\n0.0\n 71\n     0\n 72\n   100\n 73\n     1\n 74\n     3\n 75\n     0\n 76\n     0\n 77\n     0\n 78\n     0\n281\n     0\n 65\n     1\n110\n0.0\n120\n0.0\n130\n0.0\n111\n1.0\n121\n0.0\n131\n0.0\n112\n0.0\n122\n1.0\n132\n0.0\n 79\n     0\n146\n0.0\n  0\nENDTAB\n  0\nENDSEC\n  0\nEOF\n" );
			m_out.Write( " 41\n2.0\n 42\n50.0\n 43\n0.0\n 44\n0.0\n 50\n0.0\n 51\n0.0\n 71\n     0\n 72\n   100\n 73\n     1\n 74\n     3\n 75\n     0\n 76\n     0\n 77\n     0\n 78\n     0\n281\n     0\n 65\n     1\n110\n0.0\n120\n0.0\n130\n0.0\n111\n1.0\n121\n0.0\n131\n0.0\n112\n0.0\n122\n1.0\n132\n0.0\n 79\n     0\n146\n0.0\n  0\nENDTAB\n  0\n" );
			
			// write the layers table
			m_out.Write( "TABLE\n  2\n" );
			foreach( KeyValuePair<string, Color> layer in m_layers )
			{
				m_out.Write( "LAYER\n  5\n53\n330\n2\n100\nAcDbSymbolTableRecord\n100\nAcDbLayerTableRecord\n" );
				m_out.Write( "  2\n" + layer.Key + "\n" );
				m_out.Write( " 70\n     0\n" );
				m_out.Write( " 62\n     " + m_palette.GetClosestColor( layer.Value ) + "\n" );
				m_out.Write( "  6\nContinuous\n370\n    -3\n390\nF\n  0\n" );
			}
			m_out.Write( "ENDTAB\n  0\n" );
			m_out.Write( "ENDSEC\n  0\n" );
	 		m_out.Write( "EOF\n" );
			m_out.Close();
		}

		public void SetLimits( double fMinX, double fMinY, double fMaxX, double fMaxY )
		{
			m_dMinX = fMinX;
			m_dMinY = fMinY;
			m_dMaxX = fMaxX;
			m_dMaxY = fMaxY;
		}

		public void DrawLine( string strLayer, double x1, double y1, double x2, double y2 )
		{
			m_out.Write( "  0\nLINE\n  5\n" + m_iEntityNumber.ToString( "X" ) + "\n300\n1F\n100\nAcDbEntity\n100\nAcDbLine\n  8\n" );
			m_out.Write( strLayer );
			m_out.Write( "\n" );
			m_out.Write( "  10\n" + x1.ToString() + "\n" );
			m_out.Write( "  20\n" + y1.ToString() + "\n" );
			m_out.Write( "  11\n" + x2.ToString() + "\n" );
			m_out.Write( "  21\n" + y2.ToString() + "\n" );
			m_iEntityNumber += 4;
		}

		public void DrawLine( string strLayer, Vec2 a, Vec2 b )
		{
			DrawLine( strLayer, a.x, a.y, b.x, b.y );
		}

		public void DrawLine( string strLayer, Line2 l )
		{
			DrawLine( strLayer, l.v1.x, l.v1.y, l.v2.x, l.v2.y );
		}

		public void DrawCircle( string strLayer, Vec2 vCentre, double dRadius )
		{
			m_out.Write( "  0\nCIRCLE\n  8\n" );
			m_out.Write( strLayer );
			m_out.Write( "\n" );
			m_out.Write( "  10\n" + vCentre.x.ToString() + "\n" );
			m_out.Write( "  20\n" + vCentre.y.ToString() + "\n" );
			m_out.Write( "  40\n" + dRadius.ToString() + "\n" );
		}

		public void DrawArc( string strLayer, Vec2 vCentre, double dRadius, double dStartAngle, double dEndAngle )
		{
			m_out.Write( "  0\nARC\n  8\n" );
			m_out.Write( strLayer );
			m_out.Write( "\n" );
			m_out.Write( "  10\n" + vCentre.x.ToString() + "\n" );
			m_out.Write( "  20\n" + vCentre.y.ToString() + "\n" );
			m_out.Write( "  40\n" + dRadius.ToString() + "\n" );
			m_out.Write( "  50\n" + dStartAngle.ToString() + "\n" );
			m_out.Write( "  51\n" + dEndAngle.ToString() + "\n" );
		}

		public void DrawEllipseArc( string strLayer, Vec2 majorAxisV1, Vec2 majorAxisV2, double minorAxisFactor, Vec2 vArcStart, Vec2 vArcEnd )
		{
			Vec2 vCentre = ( majorAxisV1 + majorAxisV2 )*0.5;
			Vec2 vMajorAxisEndpoint = majorAxisV2 - vCentre;

			m_out.Write( "  0\nELLIPSE\n  5\n" + m_iEntityNumber.ToString( "X" ) + "\n300\n1F\n100\nAcDbEntity\n  8\n" );
			m_out.Write( strLayer + "\n" );
			m_out.Write( "  6\nBYBLOCK\n100\nAcDbEllipse\n" );
			m_out.Write( " 10\n" + vCentre.x.ToString() + "\n 20\n" + vCentre.y.ToString() + "\n 30\n0.0\n" );
			m_out.Write( " 11\n" + vMajorAxisEndpoint.x.ToString() + "\n 21\n" + vMajorAxisEndpoint.y.ToString() + "\n 31\n0.0\n" );
			m_out.Write( "210\n0.0\n220\n0.0\n230\n1.0\n" );
			m_out.Write( " 40\n" + minorAxisFactor.ToString() + "\n" );

			double dArcStart = Math.PI;
			double dArcEnd = 2*Math.PI;
			m_out.Write( " 41\n" + dArcStart.ToString() + "\n 42\n" + dArcEnd.ToString() + "\n" );
			m_iEntityNumber += 4;
		}

		public void DrawPrefab( Vec2 vPosition, DxfDocument prefab )
		{
			foreach( Line line in prefab.Lines )
				DrawLine( line.Layer.Name, line.StartPoint.X + vPosition.x, line.StartPoint.Y + vPosition.y, line.EndPoint.X + vPosition.x, line.EndPoint.Y + vPosition.y );

			foreach( Circle circle in prefab.Circles )
				DrawCircle( circle.Layer.Name, new Vec2( circle.Center.X + vPosition.x, circle.Center.Y + vPosition.y ), circle.Radius );

			foreach( Arc arc in prefab.Arcs )
				DrawArc( arc.Layer.Name, new Vec2( arc.Center.X + vPosition.x, arc.Center.Y + vPosition.y ), arc.Radius, arc.StartAngle, arc.EndAngle );
		}
	}

	class NetDxfDrawer : Drawer, IDisposable
	{
		string m_strFilename;

		DxfDocument m_dxf;
		Dictionary<string, Layer> m_layers;

		public NetDxfDrawer( string strFilename )
		{
			m_strFilename = strFilename;
			m_layers = new Dictionary<string,Layer>();
			m_dxf = new DxfDocument();
		}

		public void Dispose()
		{
			m_dxf.Save( m_strFilename, DxfVersion.AutoCad2000 );
		}

		public void AddLayer( string strName, Color c )
		{
			if( !m_layers.ContainsKey( strName ) )
			{
				Layer newLayer = new Layer( strName );
				newLayer.Color = new AciColor( c.R, c.G, c.B );
				m_layers.Add( strName, newLayer );
			}
		}

		public void SetLimits( double fMinX, double fMinY, double fMaxX, double fMaxY )
		{
			//m_dxf.G
			//m_dxf.GetLayer
			ViewPort v = new ViewPort( "*Active" );
			v.LowerLeftCorner = new Vector2d( fMinX, fMinY );
			v.UpperRightCorner = new Vector2d( fMaxX, fMaxY );
			//v.
			m_dxf.AddViewPort( v );
//			v.LowerLeftCorner.Y = fMinY;
		}

		public void DrawLine( string strLayer, double x1, double y1, double x2, double y2 )
		{
			Line l = new Line( new Vector3d( x1, y1, 0 ), new Vector3d( x2, y2, 0 ) );
			l.Layer = m_layers[ strLayer ];
			m_dxf.AddEntity( l );
		}

		public void DrawLine( string strLayer, Vec2 a, Vec2 b )
		{
			DrawLine( strLayer, a.x, a.y, b.x, b.y );
		}

		public void DrawLine( string strLayer, Line2 l )
		{
			DrawLine( strLayer, l.v1.x, l.v1.y, l.v2.x, l.v2.y );
		}

		public void DrawCircle( string strLayer, Vec2 vCentre, double dRadius )
		{
			Circle c = new Circle( new Vector3d( vCentre.x, vCentre.y, 0 ), dRadius );
			c.Layer = m_layers[ strLayer ];
			m_dxf.AddEntity( c );
		}

		public void DrawArc( string strLayer, Vec2 vCentre, double dRadius, double dStartAngle, double dEndAngle )
		{
			Arc a = new Arc( new Vector3d( vCentre.x, vCentre.y, 0 ), dRadius, dStartAngle, dEndAngle );
			a.Layer = m_layers[ strLayer ];
			m_dxf.AddEntity( a );
		}

		public void DrawEllipseArc( string strLayer, Vec2 majorAxisV1, Vec2 majorAxisV2, double minorAxisFactor, Vec2 vArcStart, Vec2 vArcEnd )
		{
	//		Ellipse e = new Ellipse();
	//		e.Rotation
		}

		public void DrawPrefab( Vec2 vPosition, DxfDocument prefab )
		{
			foreach( Line line in prefab.Lines )
				DrawLine( line.Layer.Name, line.StartPoint.X + vPosition.x, line.StartPoint.Y + vPosition.y, line.EndPoint.X + vPosition.x, line.EndPoint.Y + vPosition.y );

			foreach( Circle circle in prefab.Circles )
				DrawCircle( circle.Layer.Name, new Vec2( circle.Center.X + vPosition.x, circle.Center.Y + vPosition.y ), circle.Radius );

			foreach( Arc arc in prefab.Arcs )
				DrawArc( arc.Layer.Name, new Vec2( arc.Center.X + vPosition.x, arc.Center.Y + vPosition.y ), arc.Radius, arc.StartAngle, arc.EndAngle );
		}
	}

}
