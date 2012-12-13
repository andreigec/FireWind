namespace Project.View.Client.DrawableScreens.Full_Screens
{
    partial class OptionsWF
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
            this.SuspendLayout();
            // 
            // gobacktomainmenu
            // 
            this.gobacktomainmenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gobacktomainmenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gobacktomainmenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gobacktomainmenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.gobacktomainmenu.Location = new System.Drawing.Point(2, 381);
            this.gobacktomainmenu.Name = "gobacktomainmenu";
            this.gobacktomainmenu.Size = new System.Drawing.Size(203, 31);
            this.gobacktomainmenu.TabIndex = 2;
            this.gobacktomainmenu.Text = global::Project.Localisation.ReturnMainMenu;
            this.gobacktomainmenu.UseVisualStyleBackColor = false;
            // 
            // OptionsWF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 413);
            this.Controls.Add(this.gobacktomainmenu);
            this.Name = "OptionsWF";
            this.Text = "OptionsWF";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button gobacktomainmenu;
    }
}