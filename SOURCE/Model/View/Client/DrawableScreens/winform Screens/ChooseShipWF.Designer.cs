namespace Project.View.Client.DrawableScreens.Full_Screens
{
    partial class ChooseShipWF
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gobacktomainmenu = new System.Windows.Forms.Button();
            this.ChooseShipListBox = new System.Windows.Forms.ListBox();
            this.ChooseSelectedShipButton = new System.Windows.Forms.Button();
            this.createnewshipbutton = new System.Windows.Forms.Button();
            this.selectdefault = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // gobacktomainmenu
            // 
            this.gobacktomainmenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gobacktomainmenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gobacktomainmenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gobacktomainmenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.gobacktomainmenu.Location = new System.Drawing.Point(3, 496);
            this.gobacktomainmenu.Name = "gobacktomainmenu";
            this.gobacktomainmenu.Size = new System.Drawing.Size(203, 31);
            this.gobacktomainmenu.TabIndex = 0;
            this.gobacktomainmenu.Text = global::Project.Localisation.ReturnMainMenu;
            this.gobacktomainmenu.UseVisualStyleBackColor = false;
            // 
            // ChooseShipListBox
            // 
            this.ChooseShipListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ChooseShipListBox.BackColor = System.Drawing.Color.White;
            this.ChooseShipListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChooseShipListBox.FormattingEnabled = true;
            this.ChooseShipListBox.ItemHeight = 24;
            this.ChooseShipListBox.Location = new System.Drawing.Point(3, 10);
            this.ChooseShipListBox.Name = "ChooseShipListBox";
            this.ChooseShipListBox.Size = new System.Drawing.Size(203, 364);
            this.ChooseShipListBox.TabIndex = 1;
            // 
            // ChooseSelectedShipButton
            // 
            this.ChooseSelectedShipButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ChooseSelectedShipButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ChooseSelectedShipButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.ChooseSelectedShipButton.Location = new System.Drawing.Point(3, 435);
            this.ChooseSelectedShipButton.Name = "ChooseSelectedShipButton";
            this.ChooseSelectedShipButton.Size = new System.Drawing.Size(203, 55);
            this.ChooseSelectedShipButton.TabIndex = 2;
            this.ChooseSelectedShipButton.Text = global::Project.Localisation.ChooseSelectedShip;
            this.ChooseSelectedShipButton.UseVisualStyleBackColor = false;
            // 
            // createnewshipbutton
            // 
            this.createnewshipbutton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.createnewshipbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.createnewshipbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createnewshipbutton.ForeColor = System.Drawing.Color.Yellow;
            this.createnewshipbutton.Location = new System.Drawing.Point(413, 435);
            this.createnewshipbutton.Name = "createnewshipbutton";
            this.createnewshipbutton.Size = new System.Drawing.Size(387, 92);
            this.createnewshipbutton.TabIndex = 3;
            this.createnewshipbutton.Text = global::Project.Localisation.CreateNewPlayerShip;
            this.createnewshipbutton.UseVisualStyleBackColor = false;
            // 
            // selectdefault
            // 
            this.selectdefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectdefault.AutoSize = true;
            this.selectdefault.Location = new System.Drawing.Point(3, 384);
            this.selectdefault.Name = "selectdefault";
            this.selectdefault.Size = new System.Drawing.Size(145, 17);
            this.selectdefault.TabIndex = 5;
            this.selectdefault.Text = "Selected Ship is Default?";
            this.selectdefault.UseVisualStyleBackColor = true;
            // 
            // ChooseShipWF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 529);
            this.Controls.Add(this.selectdefault);
            this.Controls.Add(this.createnewshipbutton);
            this.Controls.Add(this.ChooseSelectedShipButton);
            this.Controls.Add(this.ChooseShipListBox);
            this.Controls.Add(this.gobacktomainmenu);
            this.Name = "ChooseShipWF";
            this.Text = "ChooseShipWF";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button gobacktomainmenu;
        private System.Windows.Forms.ListBox ChooseShipListBox;
        private System.Windows.Forms.Button ChooseSelectedShipButton;
        private System.Windows.Forms.Button createnewshipbutton;
        private System.Windows.Forms.CheckBox selectdefault;
    }
}