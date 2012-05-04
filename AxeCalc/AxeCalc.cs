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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

namespace AxeCalc
{
	public partial class AxeCalc : Form, IMessageFilter
	{
		GuitarProperties m_guitar;
		GraphicsDrawer m_graphicsDrawer;
		CustomClass m_calculatedStuff = new CustomClass();
		string m_strProjectFilename = "";

		public AxeCalc()
		{
			Application.AddMessageFilter( this );
			InitializeComponent();

			m_guitar = new GuitarProperties( this, m_calculatedStuff );
			m_graphicsDrawer = new GraphicsDrawer( m_drawingBox.DisplayRectangle );
			m_properties.SelectedObject = m_guitar;
			m_calcedProperties.SelectedObject = m_calculatedStuff;
			m_properties.PropertyValueChanged += new PropertyValueChangedEventHandler( m_properties_PropertyValueChanged );

			m_drawingBox.MouseWheel += m_drawingBox_MouseWheel;
			
			m_bassFretChart.DataSource = m_guitar.BassFretDataTable;
			m_trebleFretChart.DataSource = m_guitar.TrebleFretDataTable;
		}

		private void mainMenu_File_Exit_Click( object sender, EventArgs e )
		{
			Application.Exit();
		}

		private void m_drawingBox_Paint( object sender, PaintEventArgs e )
		{
			if( m_guitar.NeedsViewReset )
			{
				RectangleF extents = m_guitar.RoughExtents;
				m_graphicsDrawer.ResetView( new Vec2( ( extents.Left + extents.Right ) * 0.5, ( extents.Top + extents.Bottom ) * 0.5 ), 1 );
				m_guitar.NeedsViewReset = false;
			} 
			
			m_graphicsDrawer.SetUIRect( m_drawingBox.DisplayRectangle );
			m_graphicsDrawer.StartFrame( e.Graphics );
			try
			{
				m_guitar.Draw( m_graphicsDrawer );
			}
			catch //( Exception ex )
			{ 
			}

			m_graphicsDrawer.EndFrame();
		}

		private void m_properties_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
		{
			RefreshAll();
		}

		public void RefreshAll()
		{
			m_guitar.Refresh( this );
			m_properties.Refresh();
			m_calcedProperties.Refresh();
			m_drawingBox.Refresh();
		}

		private void saveAsDXFToolStripMenuItem_Click( object sender, EventArgs e )
		{
			SaveFileDialog save = new SaveFileDialog();
			save.Filter = "DXF Files|*.dxf";
			save.ShowDialog();

			if( save.FileName != "" )
			{
				try
				{
					using( NetDxfDrawer drawer = new NetDxfDrawer( save.FileName ) )
					{
						m_guitar.Draw( drawer );
					}
				}
				catch( System.Exception ex )
				{
					// TODO: Message failure
					MessageBox.Show( "Export failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				}
			}
		}

		private void saveProjectToolStripMenuItem_Click( object sender, EventArgs e )
		{
			if( m_strProjectFilename == "" )
				saveProjectAsToolStripMenuItem_Click( sender, e );
			else
				save();
		}

		private void saveProjectAsToolStripMenuItem_Click( object sender, EventArgs e )
		{
			SaveFileDialog save = new SaveFileDialog();
			save.Filter = "AxeCalc project|*.fwp";
			save.ShowDialog();

			m_strProjectFilename = save.FileName;
			this.save();
		}

		private void save()
		{
			if( m_strProjectFilename != "" )
			{
				try
				{
					m_guitar.Save( m_strProjectFilename );
				}
				catch( System.Exception ex )
				{
					// TODO: Message failure
					MessageBox.Show( "Save failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				}
			}
		}

		private void openToolStripMenuItem_Click( object sender, EventArgs e )
		{
			OpenFileDialog open = new OpenFileDialog();
			open.Filter = "AxeCalc project|*.fwp";
			open.ShowDialog();

			if( open.FileName != "" )
			{
				try
				{
					m_guitar.Load( open.FileName );
					m_strProjectFilename = open.FileName;
					m_guitar.NeedsViewReset = true;
					RefreshAll();
				}
				catch( System.Exception ex )
				{
					// TODO: Message failure
					MessageBox.Show( "Open failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				}
			}
		}

		#region Mouse scrolling
		bool m_bMouseDown = false;
		Vec2 m_vMouseMoveStart = new Vec2();

		private void m_drawingBox_MouseDown( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle )
			{
				m_bMouseDown = true;
				m_vMouseMoveStart.x = e.X;
				m_vMouseMoveStart.y = e.Y;
			}
		}
		private void m_drawingBox_MouseUp( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle )
				m_bMouseDown = false;
		}
		private void m_drawingBox_MouseMove( object sender, MouseEventArgs e )
		{
			if( m_bMouseDown )
			{
				Vec2 vNewMousePos = new Vec2( e.X, e.Y );
				Vec2 vDiff = vNewMousePos - m_vMouseMoveStart;
				m_graphicsDrawer.SetUIRect( m_drawingBox.DisplayRectangle );
				m_graphicsDrawer.Scroll( vDiff );
				m_vMouseMoveStart = vNewMousePos;
				m_drawingBox.Refresh();
			}
		}
		private void m_drawingBox_MouseLeave( object sender, EventArgs e )
		{
			m_bMouseDown = false;
		}
		private void m_drawingBox_MouseWheel( object sender, MouseEventArgs e )
		{
			m_graphicsDrawer.SetUIRect( m_drawingBox.DisplayRectangle );
			if( e.Delta < 0 )
				m_graphicsDrawer.Zoom( 0.9 * -e.Delta / 120.0, new Vec2( e.X, e.Y ) );
			else
				m_graphicsDrawer.Zoom( 1.1 * e.Delta / 120.0, new Vec2( e.X, e.Y ) );
			m_drawingBox.Refresh();
		}

		// This is necessary to redirect the mouse wheel message to the image box
		public bool PreFilterMessage( ref Message m )
		{
			if( m.Msg == 0x20a )
			{
				// WM_MOUSEWHEEL, find the control at screen position m.LParam
				Point pos = new Point( m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16 );
				IntPtr hWnd = WindowFromPoint( pos );
				if( hWnd != IntPtr.Zero && hWnd != m.HWnd )
				{
					Control control = Control.FromHandle( hWnd );
					if( control == m_drawingBox )
					{
						SendMessage( hWnd, m.Msg, m.WParam, m.LParam );
						return true;
					}
				}
			}
			return false;
		}

		// P/Invoke declarations
		[DllImport( "user32.dll" )]
		private static extern IntPtr WindowFromPoint( Point pt );
		[DllImport( "user32.dll" )]
		private static extern IntPtr SendMessage( IntPtr hWnd, int msg, IntPtr wp, IntPtr lp );

		#endregion

		private void m_drawingBox_MouseDoubleClick( object sender, MouseEventArgs e )
		{
			// resize & centre the view
			m_guitar.NeedsViewReset = true;
			m_drawingBox.Refresh();
		}

		public double FretboardBlankWidth
		{
			get
			{
				return m_dFretboardBlankWidth;
			}
		}

		double m_dFretboardBlankWidth = 70;
		private void m_FretboardBlankWidthChanged( object sender, EventArgs e )
		{
			try
			{
				m_dFretboardBlankWidth = Double.Parse( m_fretboardBlankWidth.Text );
			}
			catch
			{
			}
			m_guitar.RefreshFretCharts( this );
		}

		private int m_iPageNumber;
		private void printDocument1_BeginPrint( object sender, System.Drawing.Printing.PrintEventArgs e )
		{
			m_iPageNumber = 0;
		}
		private void printDocument1_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e )
		{
		//	Bitmap bm = new Bitmap( this.m_bassFretChart.Width, this.m_bassFretChart.Height );
		//	m_bassFretChart.DrawToBitmap( bm, new Rectangle( 0, 0, this.m_bassFretChart.Width, this.m_bassFretChart.Height ) );
		//	e.Graphics.DrawImage( bm, 0, 0 ); 

			++m_iPageNumber;

			if( m_guitar.MultiScale )
			{
			/*	if( m_iPageNumber == 1 )
				{
					PrintDataGrid( "Bass Fret Chart", m_bassFretChart, e.MarginBounds, e );
					e.HasMorePages = true;
				}
				else if( m_iPageNumber == 2 )
					PrintDataGrid( "Treble Fret Chart", m_trebleFretChart, e.MarginBounds, e );*/

				Rectangle halfPage = new Rectangle( e.MarginBounds.X, e.MarginBounds.Y, e.MarginBounds.Width, e.MarginBounds.Height / 2 );
				PrintDataGrid( "Bass Fret Chart", m_bassFretChart, halfPage, e.Graphics );
				halfPage.Y += halfPage.Height;
				PrintDataGrid( "Treble Fret Chart", m_trebleFretChart, halfPage, e.Graphics );
			}
			else
			{
				PrintDataGrid( "Fret Chart", m_bassFretChart, e.MarginBounds, e.Graphics );
			}
		}

		private static void PrintDataGrid( string strTitle, DataGridView grid, Rectangle bounds, Graphics g )
		{
			FontFamily fontFamily;
			try
			{
				fontFamily = FontFamily.Families.Single( v => v.Name == "Consolas" );
			}
			catch
			{
				fontFamily = FontFamily.GenericMonospace;
			}

			Font titleFont = new Font( fontFamily, 16, FontStyle.Bold );
			g.DrawString( strTitle, titleFont, Brushes.Black, bounds.Left, bounds.Top );
			bounds.Y += titleFont.Height;
			bounds.Height -= titleFont.Height;

			float fMaxFontSize = Math.Min( 13, ( float )bounds.Height / ( float )( grid.RowCount + 1 ) - 8 );
			Font tempFont = new Font( fontFamily, fMaxFontSize );
			//e.Graphics.DrawString( textToPrint, printFont, Brushes.Black, 0, 0 );
			
			// evenly divide the columns across the screen
			float fColumnWidth = ( float )bounds.Width / ( float )( grid.ColumnCount );
			float[] fColumnPositions = new float[ grid.Columns.Count ];

			// make sure no text will go out of its cell
			float fMaxTextWidth = 0;
			float[] fMaxColumnTextWidth = new float[ grid.Columns.Count ];
			for( int i = 0; i < grid.ColumnCount; ++i )
			{
				DataGridViewColumn column = grid.Columns[ i ];
				SizeF size = g.MeasureString( column.Name, tempFont );
				fMaxColumnTextWidth[ i ] = size.Width;
				fMaxTextWidth = Math.Max( fMaxColumnTextWidth[ i ], fMaxTextWidth );
			}

			// go through all cells
			foreach( DataGridViewRow row in grid.Rows )
			{
				for( int i = 0; i < row.Cells.Count; ++i )
				{
					DataGridViewCell cell = row.Cells[ i ];
					SizeF size = g.MeasureString( cell.Value.ToString(), tempFont );
					fMaxColumnTextWidth[ i ] = Math.Max( fMaxColumnTextWidth[ i ], size.Width );
					fMaxTextWidth = Math.Max( fMaxColumnTextWidth[ i ], fMaxTextWidth );
				}
			}

			if( fMaxTextWidth > fColumnWidth )
			{
				// scale down the font
				fMaxFontSize = fMaxFontSize * fColumnWidth / fMaxTextWidth;
			}

			Font font = new Font( fontFamily, fMaxFontSize );
			Font boldFont = new Font( fontFamily, fMaxFontSize, FontStyle.Bold );

			// assign the share of the page width based on the width of the column contents
			float fTotalColumnContents = fMaxColumnTextWidth.Sum();
			fColumnPositions[ 0 ] = bounds.Left;
			for( int i = 1; i < grid.ColumnCount; ++i )
				fColumnPositions[ i ] = fColumnPositions[ i - 1 ] + fMaxColumnTextWidth[ i - 1 ] * bounds.Width / fTotalColumnContents;


			Pen blackPen = new Pen( Brushes.Black );

			// draw column headings and column lines
			float fGridBottom = bounds.Top + font.Height * ( grid.RowCount + 1 );
			for( int i = 0; i < grid.ColumnCount; ++i )
			{
				DataGridViewColumn column = grid.Columns[ i ];
				g.DrawString( column.Name, boldFont, Brushes.Black, fColumnPositions[ i ], bounds.Top );
				g.DrawLine( blackPen, fColumnPositions[ i ], bounds.Top, fColumnPositions[ i ], fGridBottom );
			}
			g.DrawLine( blackPen, bounds.Right, bounds.Top, bounds.Right, fGridBottom );

			// draw rows and row lines
			g.DrawLine( blackPen, bounds.Left, bounds.Top, bounds.Right, bounds.Top );
			float fHeightUpto = bounds.Top + font.Height;
			foreach( DataGridViewRow row in grid.Rows )
			{
				for( int i = 0; i < row.Cells.Count; ++i )
				{
					DataGridViewCell cell = row.Cells[ i ];
					g.DrawString( cell.Value.ToString(), font, Brushes.Black, fColumnPositions[ i ], fHeightUpto );
				}
				g.DrawLine( blackPen, bounds.Left, fHeightUpto, bounds.Right, fHeightUpto );
				fHeightUpto += font.Height;
			}
			g.DrawLine( blackPen, bounds.Left, fHeightUpto, bounds.Right, fHeightUpto );
		}

		private void m_printFretCharts_Click( object sender, EventArgs e )
		{
			PrintDialog d = new PrintDialog();
			d.Document = printDocument1;
			if( d.ShowDialog() == DialogResult.OK )
				printDocument1.Print();
			
		/*	PrintPreviewDialog d = new PrintPreviewDialog();
			d.Document = printDocument1;
			d.ShowDialog();*/
		}
	}
}
