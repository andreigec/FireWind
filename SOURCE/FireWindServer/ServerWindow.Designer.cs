using System.Windows.Forms;

namespace FireWindServer
{
    partial class ServerWindow
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.toggleserverbutton = new System.Windows.Forms.Button();
            this.Rconpwtext = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.udpportText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tcpread = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.maxplayer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.servernametext = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gamewindowlist = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.creategamebutton = new System.Windows.Forms.Button();
            this.stopselectedgamesbutton = new System.Windows.Forms.Button();
            this.consolelog = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lanonlyCB = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // toggleserverbutton
            // 
            this.toggleserverbutton.Location = new System.Drawing.Point(0, 208);
            this.toggleserverbutton.Name = "toggleserverbutton";
            this.toggleserverbutton.Size = new System.Drawing.Size(255, 52);
            this.toggleserverbutton.TabIndex = 23;
            this.toggleserverbutton.Text = "Start Server";
            this.toggleserverbutton.UseVisualStyleBackColor = true;
            this.toggleserverbutton.Click += new System.EventHandler(this.toggleserverbutton_Click);
            // 
            // Rconpwtext
            // 
            this.Rconpwtext.Location = new System.Drawing.Point(105, 89);
            this.Rconpwtext.Name = "Rconpwtext";
            this.Rconpwtext.Size = new System.Drawing.Size(100, 20);
            this.Rconpwtext.TabIndex = 22;
            this.Rconpwtext.Text = "rconpw";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "RCON password:";
            // 
            // udpportText
            // 
            this.udpportText.Location = new System.Drawing.Point(105, 141);
            this.udpportText.Name = "udpportText";
            this.udpportText.Size = new System.Drawing.Size(100, 20);
            this.udpportText.TabIndex = 20;
            this.udpportText.Text = "3003";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "UDP port to host on:";
            // 
            // tcpread
            // 
            this.tcpread.Location = new System.Drawing.Point(105, 115);
            this.tcpread.Name = "tcpread";
            this.tcpread.Size = new System.Drawing.Size(100, 20);
            this.tcpread.TabIndex = 18;
            this.tcpread.Text = "3001";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "TCP Read";
            // 
            // maxplayer
            // 
            this.maxplayer.Location = new System.Drawing.Point(105, 56);
            this.maxplayer.Name = "maxplayer";
            this.maxplayer.Size = new System.Drawing.Size(100, 20);
            this.maxplayer.TabIndex = 16;
            this.maxplayer.Text = "10";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Max Players:";
            // 
            // servernametext
            // 
            this.servernametext.Location = new System.Drawing.Point(105, 17);
            this.servernametext.Name = "servernametext";
            this.servernametext.Size = new System.Drawing.Size(100, 20);
            this.servernametext.TabIndex = 14;
            this.servernametext.Text = "test firewind server";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Server Name:";
            // 
            // gamewindowlist
            // 
            this.gamewindowlist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gamewindowlist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader1,
            this.columnHeader2});
            this.gamewindowlist.FullRowSelect = true;
            this.gamewindowlist.Location = new System.Drawing.Point(256, 17);
            this.gamewindowlist.Name = "gamewindowlist";
            this.gamewindowlist.Size = new System.Drawing.Size(264, 179);
            this.gamewindowlist.TabIndex = 24;
            this.gamewindowlist.UseCompatibleStateImageBehavior = false;
            this.gamewindowlist.View = System.Windows.Forms.View.Details;
            this.gamewindowlist.SelectedIndexChanged += new System.EventHandler(this.gamewindowlist_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "GameID";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Game Type";
            this.columnHeader1.Width = 121;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Players";
            this.columnHeader2.Width = 89;
            // 
            // creategamebutton
            // 
            this.creategamebutton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.creategamebutton.Location = new System.Drawing.Point(265, 237);
            this.creategamebutton.Name = "creategamebutton";
            this.creategamebutton.Size = new System.Drawing.Size(255, 23);
            this.creategamebutton.TabIndex = 25;
            this.creategamebutton.Text = "Create Game";
            this.creategamebutton.UseVisualStyleBackColor = true;
            this.creategamebutton.Click += new System.EventHandler(this.creategamebutton_Click);
            // 
            // stopselectedgamesbutton
            // 
            this.stopselectedgamesbutton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stopselectedgamesbutton.Location = new System.Drawing.Point(265, 208);
            this.stopselectedgamesbutton.Name = "stopselectedgamesbutton";
            this.stopselectedgamesbutton.Size = new System.Drawing.Size(255, 23);
            this.stopselectedgamesbutton.TabIndex = 26;
            this.stopselectedgamesbutton.Text = "Stop Selected Games";
            this.stopselectedgamesbutton.UseVisualStyleBackColor = true;
            this.stopselectedgamesbutton.Click += new System.EventHandler(this.stopselectedgamesbutton_Click);
            // 
            // consolelog
            // 
            this.consolelog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.consolelog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10});
            this.consolelog.Location = new System.Drawing.Point(0, 266);
            this.consolelog.Name = "consolelog";
            this.consolelog.Size = new System.Drawing.Size(520, 292);
            this.consolelog.TabIndex = 27;
            this.consolelog.UseCompatibleStateImageBehavior = false;
            this.consolelog.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Time";
            this.columnHeader4.Width = 54;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Client";
            this.columnHeader5.Width = 95;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "id";
            this.columnHeader6.Width = 36;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "ref";
            this.columnHeader7.Width = 35;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Extra Text:";
            this.columnHeader8.Width = 126;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Message Type";
            this.columnHeader9.Width = 146;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Params";
            this.columnHeader10.Width = 125;
            // 
            // lanonlyCB
            // 
            this.lanonlyCB.AutoSize = true;
            this.lanonlyCB.Checked = true;
            this.lanonlyCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lanonlyCB.Location = new System.Drawing.Point(105, 179);
            this.lanonlyCB.Name = "lanonlyCB";
            this.lanonlyCB.Size = new System.Drawing.Size(71, 17);
            this.lanonlyCB.TabIndex = 28;
            this.lanonlyCB.Text = "LAN Only";
            this.lanonlyCB.UseVisualStyleBackColor = true;
            // 
            // ServerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lanonlyCB);
            this.Controls.Add(this.consolelog);
            this.Controls.Add(this.stopselectedgamesbutton);
            this.Controls.Add(this.creategamebutton);
            this.Controls.Add(this.gamewindowlist);
            this.Controls.Add(this.toggleserverbutton);
            this.Controls.Add(this.Rconpwtext);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.udpportText);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tcpread);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.maxplayer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.servernametext);
            this.Controls.Add(this.label1);
            this.Name = "ServerWindow";
            this.Size = new System.Drawing.Size(529, 572);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button toggleserverbutton;
        private System.Windows.Forms.TextBox Rconpwtext;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox udpportText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tcpread;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox maxplayer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox servernametext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView gamewindowlist;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button creategamebutton;
        private System.Windows.Forms.Button stopselectedgamesbutton;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        public ListView  consolelog;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private CheckBox lanonlyCB;
    }
}
