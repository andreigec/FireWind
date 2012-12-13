using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using ANDREICSLIB;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    partial class ConnectWF
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gobacktobase = new System.Windows.Forms.Button();
            this.ippanel = new ANDREICSLIB.PanelReplacement();
            this.connectbutton = new System.Windows.Forms.Button();
            this.creategamebutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gobacktobase
            // 
            this.gobacktobase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gobacktobase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gobacktobase.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gobacktobase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.gobacktobase.Location = new System.Drawing.Point(-3, 385);
            this.gobacktobase.Name = "gobacktobase";
            this.gobacktobase.Size = new System.Drawing.Size(203, 31);
            this.gobacktobase.TabIndex = 4;
            this.gobacktobase.Text = global::Project.Localisation.BackToBase;
            this.gobacktobase.UseVisualStyleBackColor = false;
            // 
            // ippanel
            // 
            this.ippanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ippanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ippanel.BorderColour = System.Drawing.Color.Black;
            this.ippanel.BorderWidth = 0;
            this.ippanel.Location = new System.Drawing.Point(12, 24);
            this.ippanel.Name = "ippanel";
            this.ippanel.Size = new System.Drawing.Size(593, 355);
            this.ippanel.TabIndex = 5;
            // 
            // connectbutton
            // 
            this.connectbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.connectbutton.BackColor = System.Drawing.Color.Teal;
            this.connectbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectbutton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.connectbutton.Location = new System.Drawing.Point(411, 385);
            this.connectbutton.Name = "connectbutton";
            this.connectbutton.Size = new System.Drawing.Size(203, 31);
            this.connectbutton.TabIndex = 6;
            this.connectbutton.Text = "Connect To IP";
            this.connectbutton.UseVisualStyleBackColor = false;
            // 
            // creategamebutton
            // 
            this.creategamebutton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.creategamebutton.BackColor = System.Drawing.Color.Green;
            this.creategamebutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.creategamebutton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.creategamebutton.Location = new System.Drawing.Point(206, 385);
            this.creategamebutton.Name = "creategamebutton";
            this.creategamebutton.Size = new System.Drawing.Size(203, 31);
            this.creategamebutton.TabIndex = 7;
            this.creategamebutton.Text = "Create Game";
            this.creategamebutton.UseVisualStyleBackColor = false;
            // 
            // ConnectWF
            // 
            this.ClientSize = new System.Drawing.Size(617, 419);
            this.Controls.Add(this.creategamebutton);
            this.Controls.Add(this.connectbutton);
            this.Controls.Add(this.ippanel);
            this.Controls.Add(this.gobacktobase);
            this.Name = "ConnectWF";
            this.Text = "ConnectWF";
            this.ResumeLayout(false);

        }

        #endregion

        private Button gobacktobase;
        public ANDREICSLIB.PanelReplacement ippanel;
        private Button connectbutton;
        private Button creategamebutton;
    }
}