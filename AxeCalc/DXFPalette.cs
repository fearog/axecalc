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

namespace AxeCalc
{
	class DXFPalette
	{
		Color[] m_palette;

		public DXFPalette()
		{
			// calculate the DXF palette - it is kinda insane.
			m_palette = new Color[ 260 ];
			// stock colours
			m_palette[ 0 ] = Color.FromArgb( 255, 0, 0, 0 );
			m_palette[ 1 ] = Color.FromArgb( 255, 255, 0, 0 );
			m_palette[ 2 ] = Color.FromArgb( 255, 255, 255, 0 );
			m_palette[ 3 ] = Color.FromArgb( 255, 0, 255, 0 );
			m_palette[ 4 ] = Color.FromArgb( 255, 0, 255, 255 );
			m_palette[ 5 ] = Color.FromArgb( 255, 0, 0, 255 );
			m_palette[ 6 ] = Color.FromArgb( 255, 255, 0, 255 );
			m_palette[ 7 ] = Color.FromArgb( 255, 255, 255, 255 );
			m_palette[ 8 ] = Color.FromArgb( 255, 128, 128, 128 );
			m_palette[ 9 ] = Color.FromArgb( 255, 192, 192, 192 );

			// Full strength
			CreatePaletteRow( 0, 0, 255 );
			// Darkened versions
			CreatePaletteRow( 2, 0, 204 );
			CreatePaletteRow( 4, 0, 153 );
			CreatePaletteRow( 6, 0, 127 );
			CreatePaletteRow( 8, 0, 76 );
			// Pastel versions
			CreatePaletteRow( 1, 127, 255 );
			CreatePaletteRow( 3, 102, 204 );
			CreatePaletteRow( 5, 76, 153 );
			CreatePaletteRow( 7, 64, 127 );
			CreatePaletteRow( 9, 38, 76 );


			m_palette[ 250 ] = Color.FromArgb( 255, 51, 51, 51 );
			m_palette[ 251 ] = Color.FromArgb( 255, 91, 91, 91 );
			m_palette[ 252 ] = Color.FromArgb( 255, 132, 132, 132 );
			m_palette[ 253 ] = Color.FromArgb( 255, 173, 173, 173 );
			m_palette[ 254 ] = Color.FromArgb( 255, 214, 214, 214 );
			m_palette[ 255 ] = Color.FromArgb( 255, 255, 255, 255 );
		}

		void CreatePaletteRow( int iRow, int iMin, int iMax )
		{
			m_palette[ iRow + 10 ] = Color.FromArgb( 255, iMax, iMin, iMin );
			m_palette[ iRow + 50 ] = Color.FromArgb( 255, iMax, iMax, iMin );
			m_palette[ iRow + 90 ] = Color.FromArgb( 255, iMin, iMax, iMin );
			m_palette[ iRow + 130 ] = Color.FromArgb( 255, iMin, iMax, iMax );
			m_palette[ iRow + 170 ] = Color.FromArgb( 255, iMin, iMin, iMax );
			m_palette[ iRow + 210 ] = Color.FromArgb( 255, iMax, iMin, iMax );
			m_palette[ iRow + 250 ] = Color.FromArgb( 255, iMax, iMin, iMin );
			PaletteBlend( iRow + 10, iRow + 50, 10 );
			PaletteBlend( iRow + 50, iRow + 90, 10 );
			PaletteBlend( iRow + 90, iRow + 130, 10 );
			PaletteBlend( iRow + 130, iRow + 170, 10 );
			PaletteBlend( iRow + 170, iRow + 210, 10 );
			PaletteBlend( iRow + 210, iRow + 250, 10 );
		}

		void PaletteBlend( int iFrom, int iTo, int iStep )
		{
			double dDistance = iTo - iFrom;
			double dStepR = ( m_palette[ iTo ].R - m_palette[ iFrom ].R ) / ( iTo - iFrom );
			double dStepG = ( m_palette[ iTo ].G - m_palette[ iFrom ].G ) / ( iTo - iFrom );
			double dStepB = ( m_palette[ iTo ].B - m_palette[ iFrom ].B ) / ( iTo - iFrom );
			for( int i = iFrom + iStep; i < iTo; i += iStep )
			{
				double dDelta = i - iFrom;
				m_palette[ i ] = Color.FromArgb( 255, ( int )( m_palette[ iFrom ].R + dStepR * dDelta ),
													( int )( m_palette[ iFrom ].G + dStepG * dDelta ),
													( int )( m_palette[ iFrom ].B + dStepB * dDelta ) );
			}
		}

		public void Draw( Graphics g )
		{
			int[] rows = { 9, 7, 5, 3, 1, 0, 2, 4, 6, 8 };
			int iColumn = 0;
			foreach( int iRow in rows )
			{
				for( int i = 0; i < 24; ++i )
				{
					g.FillRectangle( new SolidBrush( m_palette[ ( i + 1 ) * 10 + iRow ] ), i * 30, iColumn * 30, ( i + 1 ) * 30, ( iColumn + 1 ) * 30 );
				}
				++iColumn;
			}
		}

		public int GetClosestColor( Color c )
		{
			// Supremely naive nearest colour matching
			double dBestMatch = 256.0 * 256.0 + 256.0 * 256.0 + 256.0 * 256.0;
			int iBestMatchIndex = 7;
			for( int i = 0; i < 256; ++i )
			{
				double dDiffR = ( double )m_palette[ i ].R - ( double )c.R;
				double dDiffG = ( double )m_palette[ i ].G - ( double )c.G;
				double dDiffB = ( double )m_palette[ i ].B - ( double )c.B;
				double dDiffSqr = dDiffR * dDiffR + dDiffG * dDiffG + dDiffB * dDiffB;
				if( dDiffSqr < dBestMatch )
				{
					dBestMatch = dDiffSqr;
					iBestMatchIndex = i;
				}
			}

			return iBestMatchIndex;
		}
	}
}
