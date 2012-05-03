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

namespace AxeCalc
{
	partial class AxeCalc
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_mainMenu = new System.Windows.Forms.MenuStrip();
			this.m_mainMenu_File = new System.Windows.Forms.ToolStripMenuItem();
			this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pRSSinglecutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveProjectAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.saveAsDXFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.m_mainMenu_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.m_calcedProperties = new System.Windows.Forms.PropertyGrid();
			this.m_properties = new System.Windows.Forms.PropertyGrid();
			this.m_drawingBox = new System.Windows.Forms.PictureBox();
			this.m_mainMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_drawingBox)).BeginInit();
			this.SuspendLayout();
			// 
			// m_mainMenu
			// 
			this.m_mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_mainMenu_File});
			this.m_mainMenu.Location = new System.Drawing.Point(0, 0);
			this.m_mainMenu.Name = "m_mainMenu";
			this.m_mainMenu.Size = new System.Drawing.Size(1216, 24);
			this.m_mainMenu.TabIndex = 0;
			this.m_mainMenu.Text = "mainMenu";
			// 
			// m_mainMenu_File
			// 
			this.m_mainMenu_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripMenuItem2,
            this.saveProjectToolStripMenuItem,
            this.saveProjectAsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.saveAsDXFToolStripMenuItem,
            this.toolStripMenuItem1,
            this.m_mainMenu_File_Exit});
			this.m_mainMenu_File.Name = "m_mainMenu_File";
			this.m_mainMenu_File.Size = new System.Drawing.Size(37, 20);
			this.m_mainMenu_File.Text = "File";
			// 
			// newProjectToolStripMenuItem
			// 
			this.newProjectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pRSSinglecutToolStripMenuItem});
			this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
			this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.newProjectToolStripMenuItem.Text = "New";
			// 
			// pRSSinglecutToolStripMenuItem
			// 
			this.pRSSinglecutToolStripMenuItem.Name = "pRSSinglecutToolStripMenuItem";
			this.pRSSinglecutToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
			this.pRSSinglecutToolStripMenuItem.Text = "PRS Singlecut";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(137, 6);
			// 
			// saveProjectToolStripMenuItem
			// 
			this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
			this.saveProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.saveProjectToolStripMenuItem.Text = "Save";
			this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
			// 
			// saveProjectAsToolStripMenuItem
			// 
			this.saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
			this.saveProjectAsToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.saveProjectAsToolStripMenuItem.Text = "Save as...";
			this.saveProjectAsToolStripMenuItem.Click += new System.EventHandler(this.saveProjectAsToolStripMenuItem_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(137, 6);
			// 
			// saveAsDXFToolStripMenuItem
			// 
			this.saveAsDXFToolStripMenuItem.Name = "saveAsDXFToolStripMenuItem";
			this.saveAsDXFToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.saveAsDXFToolStripMenuItem.Text = "Export DXF...";
			this.saveAsDXFToolStripMenuItem.Click += new System.EventHandler(this.saveAsDXFToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(137, 6);
			// 
			// m_mainMenu_File_Exit
			// 
			this.m_mainMenu_File_Exit.Name = "m_mainMenu_File_Exit";
			this.m_mainMenu_File_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.m_mainMenu_File_Exit.Size = new System.Drawing.Size(140, 22);
			this.m_mainMenu_File_Exit.Text = "Exit";
			this.m_mainMenu_File_Exit.Click += new System.EventHandler(this.mainMenu_File_Exit_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.m_calcedProperties);
			this.splitContainer1.Panel1.Controls.Add(this.m_properties);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.m_drawingBox);
			this.splitContainer1.Panel2MinSize = 200;
			this.splitContainer1.Size = new System.Drawing.Size(1216, 742);
			this.splitContainer1.SplitterDistance = 511;
			this.splitContainer1.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(613, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(116, 17);
			this.label2.TabIndex = 3;
			this.label2.Text = "Calculated Info";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(4, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(141, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Guitar Parameters";
			// 
			// m_calcedProperties
			// 
			this.m_calcedProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.m_calcedProperties.Location = new System.Drawing.Point(616, 27);
			this.m_calcedProperties.Name = "m_calcedProperties";
			this.m_calcedProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.m_calcedProperties.Size = new System.Drawing.Size(600, 484);
			this.m_calcedProperties.TabIndex = 1;
			this.m_calcedProperties.ToolbarVisible = false;
			// 
			// m_properties
			// 
			this.m_properties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.m_properties.Location = new System.Drawing.Point(0, 27);
			this.m_properties.Name = "m_properties";
			this.m_properties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.m_properties.Size = new System.Drawing.Size(600, 484);
			this.m_properties.TabIndex = 0;
			this.m_properties.ToolbarVisible = false;
			// 
			// m_drawingBox
			// 
			this.m_drawingBox.BackColor = System.Drawing.Color.Black;
			this.m_drawingBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_drawingBox.Location = new System.Drawing.Point(0, 0);
			this.m_drawingBox.Name = "m_drawingBox";
			this.m_drawingBox.Size = new System.Drawing.Size(1216, 227);
			this.m_drawingBox.TabIndex = 0;
			this.m_drawingBox.TabStop = false;
			this.m_drawingBox.Paint += new System.Windows.Forms.PaintEventHandler(this.m_drawingBox_Paint);
			this.m_drawingBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_drawingBox_MouseDoubleClick);
			this.m_drawingBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_drawingBox_MouseDown);
			this.m_drawingBox.MouseLeave += new System.EventHandler(this.m_drawingBox_MouseLeave);
			this.m_drawingBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.m_drawingBox_MouseMove);
			this.m_drawingBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.m_drawingBox_MouseUp);
			// 
			// AxeCalc
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1216, 766);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.m_mainMenu);
			this.MainMenuStrip = this.m_mainMenu;
			this.Name = "AxeCalc";
			this.Text = "AxeCalc";
			this.m_mainMenu.ResumeLayout(false);
			this.m_mainMenu.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.m_drawingBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip m_mainMenu;
		private System.Windows.Forms.ToolStripMenuItem m_mainMenu_File;
		private System.Windows.Forms.ToolStripMenuItem m_mainMenu_File_Exit;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.PropertyGrid m_properties;
		private System.Windows.Forms.PictureBox m_drawingBox;
		private System.Windows.Forms.ToolStripMenuItem saveAsDXFToolStripMenuItem;
		private System.Windows.Forms.PropertyGrid m_calcedProperties;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pRSSinglecutToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem saveProjectAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
	}
}

