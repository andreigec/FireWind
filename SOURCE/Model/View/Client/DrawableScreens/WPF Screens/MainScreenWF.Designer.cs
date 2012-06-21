namespace Project.View.Client.DrawableScreens.Full_Screens
{
    partial class MainScreenWF
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
            this.exitbutton = new System.Windows.Forms.Button();
            this.basebutton = new System.Windows.Forms.Button();
            this.chooseshipbutton = new System.Windows.Forms.Button();
            this.GoToOptionsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // exitbutton
            // 
            this.exitbutton.BackColor = System.Drawing.Color.Maroon;
            this.exitbutton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.exitbutton.Font = new System.Drawing.Font("Segoe UI Mono", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitbutton.ForeColor = System.Drawing.Color.Yellow;
            this.exitbutton.Location = new System.Drawing.Point(387, 194);
            this.exitbutton.Name = "exitbutton";
            this.exitbutton.Size = new System.Drawing.Size(202, 168);
            this.exitbutton.TabIndex = 0;
            this.exitbutton.Text = global::Project.Localisation.Exit;
            this.exitbutton.UseVisualStyleBackColor = false;
            // 
            // basebutton
            // 
            this.basebutton.BackColor = System.Drawing.Color.CadetBlue;
            this.basebutton.Font = new System.Drawing.Font("Segoe UI Mono", 12F);
            this.basebutton.ForeColor = System.Drawing.Color.Yellow;
            this.basebutton.Location = new System.Drawing.Point(12, 12);
            this.basebutton.Name = "basebutton";
            this.basebutton.Size = new System.Drawing.Size(345, 170);
            this.basebutton.TabIndex = 1;
            this.basebutton.Text = global::Project.Localisation.GoToBase;
            this.basebutton.UseVisualStyleBackColor = false;
            // 
            // chooseshipbutton
            // 
            this.chooseshipbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.chooseshipbutton.ForeColor = System.Drawing.Color.Yellow;
            this.chooseshipbutton.Location = new System.Drawing.Point(363, 24);
            this.chooseshipbutton.Name = "chooseshipbutton";
            this.chooseshipbutton.Size = new System.Drawing.Size(288, 133);
            this.chooseshipbutton.TabIndex = 2;
            this.chooseshipbutton.Text = global::Project.Localisation.ChooseShip;
            this.chooseshipbutton.UseVisualStyleBackColor = false;
            // 
            // GoToOptionsButton
            // 
            this.GoToOptionsButton.BackColor = System.Drawing.Color.Purple;
            this.GoToOptionsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GoToOptionsButton.ForeColor = System.Drawing.Color.Yellow;
            this.GoToOptionsButton.Location = new System.Drawing.Point(25, 218);
            this.GoToOptionsButton.Name = "GoToOptionsButton";
            this.GoToOptionsButton.Size = new System.Drawing.Size(286, 144);
            this.GoToOptionsButton.TabIndex = 3;
            this.GoToOptionsButton.Text = Localisation.Options;
            this.GoToOptionsButton.UseVisualStyleBackColor = false;
            // 
            // MainScreenWF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 412);
            this.Controls.Add(this.GoToOptionsButton);
            this.Controls.Add(this.chooseshipbutton);
            this.Controls.Add(this.basebutton);
            this.Controls.Add(this.exitbutton);
            this.Name = "MainScreenWF";
            this.Text = "MainScreenWF";
            this.Load += new System.EventHandler(this.MainScreenWF_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button exitbutton;
        private System.Windows.Forms.Button basebutton;
        private System.Windows.Forms.Button chooseshipbutton;
        private System.Windows.Forms.Button GoToOptionsButton;
    }
}