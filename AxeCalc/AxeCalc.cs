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
		CustomClass m_calculatedStuff;
		string m_strProjectFilename;

		public AxeCalc()
		{
			Application.AddMessageFilter( this );
			InitializeComponent();

			m_strProjectFilename = "";
			m_calculatedStuff = new CustomClass();
			m_guitar = new GuitarProperties( m_calculatedStuff );
			m_graphicsDrawer = new GraphicsDrawer( m_drawingBox.DisplayRectangle );
			m_properties.SelectedObject = m_guitar;
			m_calcedProperties.SelectedObject = m_calculatedStuff;
//			AttributeCollection a = m_properties.It;
			m_properties.PropertyValueChanged += new PropertyValueChangedEventHandler( m_properties_PropertyValueChanged );

			m_drawingBox.MouseWheel += m_drawingBox_MouseWheel;
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

		public void m_properties_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
		{
			RefreshAll();
		}

		public void RefreshAll()
		{
			m_guitar.Refresh();
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

	}
}
