namespace Project.View.Client.DrawableScreens.Full_Screens
{
    partial class BaseScreenWF
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
            this.GoToShopButton = new System.Windows.Forms.Button();
            this.GoToHangarButton = new System.Windows.Forms.Button();
            this.FlyButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gobacktomainmenu
            // 
            this.gobacktomainmenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gobacktomainmenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gobacktomainmenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gobacktomainmenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.gobacktomainmenu.Location = new System.Drawing.Point(0, 410);
            this.gobacktomainmenu.Name = "gobacktomainmenu";
            this.gobacktomainmenu.Size = new System.Drawing.Size(203, 31);
            this.gobacktomainmenu.TabIndex = 1;
            this.gobacktomainmenu.Text = global::Project.Localisation.ReturnMainMenu;
            this.gobacktomainmenu.UseVisualStyleBackColor = false;
            // 
            // GoToShopButton
            // 
            this.GoToShopButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.GoToShopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GoToShopButton.Location = new System.Drawing.Point(12, 202);
            this.GoToShopButton.Name = "GoToShopButton";
            this.GoToShopButton.Size = new System.Drawing.Size(252, 181);
            this.GoToShopButton.TabIndex = 2;
            this.GoToShopButton.Text = global::Project.Localisation.VisitShop;
            this.GoToShopButton.UseVisualStyleBackColor = false;
            // 
            // GoToHangarButton
            // 
            this.GoToHangarButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.GoToHangarButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GoToHangarButton.Location = new System.Drawing.Point(339, 212);
            this.GoToHangarButton.Name = "GoToHangarButton";
            this.GoToHangarButton.Size = new System.Drawing.Size(252, 181);
            this.GoToHangarButton.TabIndex = 3;
            this.GoToHangarButton.Text = global::Project.Localisation.VisitHangar;
            this.GoToHangarButton.UseVisualStyleBackColor = false;
            // 
            // FlyButton
            // 
            this.FlyButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.FlyButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FlyButton.Location = new System.Drawing.Point(63, 40);
            this.FlyButton.Name = "FlyButton";
            this.FlyButton.Size = new System.Drawing.Size(491, 139);
            this.FlyButton.TabIndex = 4;
            this.FlyButton.Text = global::Project.Localisation.FlyToPlanet;
            this.FlyButton.UseVisualStyleBackColor = false;
            // 
            // BaseScreenWF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.FlyButton);
            this.Controls.Add(this.GoToHangarButton);
            this.Controls.Add(this.GoToShopButton);
            this.Controls.Add(this.gobacktomainmenu);
            this.Name = "BaseScreenWF";
            this.Text = "BaseScreenWF";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button gobacktomainmenu;
        private System.Windows.Forms.Button GoToShopButton;
        private System.Windows.Forms.Button GoToHangarButton;
        private System.Windows.Forms.Button FlyButton;
    }
}