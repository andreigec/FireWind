using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ANDREICSLIB;

namespace FireWindServer
{
	partial class GameWindow
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		    this.consolelog = new ListView();
            this.columnHeader1 = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeader2 = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeader4 = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeader5 = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeader7 = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeader3 = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeader6 = ((ColumnHeader)(new ColumnHeader()));
            this.panel1 = new Panel();
            this.panelUpdates1 = new Panel();
            this.SuspendLayout();
            // 
            // consolelog
            // 
            this.consolelog.Columns.AddRange(new ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader7,
            this.columnHeader3,
            this.columnHeader6});
            this.consolelog.Dock = DockStyle.Top;
            this.consolelog.Location = new Point(0, 36);
            this.consolelog.Name = "consolelog";
            this.consolelog.Size = new Size(700, 290);
            this.consolelog.TabIndex = 0;
            this.consolelog.UseCompatibleStateImageBehavior = false;
            this.consolelog.View = View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            this.columnHeader1.Width = 54;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Client";
            this.columnHeader2.Width = 95;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "id";
            this.columnHeader4.Width = 36;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "ref";
            this.columnHeader5.Width = 35;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Extra Text:";
            this.columnHeader7.Width = 126;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Message Type";
            this.columnHeader3.Width = 146;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Params";
            this.columnHeader6.Width = 125;
            // 
            // panel1
            // 
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Location = new Point(0, 326);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(700, 50);
            this.panel1.TabIndex = 2;
            // 
            // panelUpdates1
            // 
            this.panelUpdates1.Dock = DockStyle.Top;
            this.panelUpdates1.Location = new Point(0, 0);
            this.panelUpdates1.Name = "panelUpdates1";
            this.panelUpdates1.Size = new Size(700, 36);
            this.panelUpdates1.TabIndex = 3;
            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.consolelog);
            this.Controls.Add(this.panelUpdates1);
            this.Name = "GameWindow";
            this.Size = new Size(700, 391);
            this.ResumeLayout(false);

		}

		#endregion

        public ListView consolelog;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader7;
        private Panel panel1;
        private Panel panelUpdates1;
	}
}
