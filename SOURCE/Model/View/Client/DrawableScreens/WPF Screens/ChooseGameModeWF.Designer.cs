namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    partial class ChooseGameModeWF
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.backtoservers = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gamblebutton = new System.Windows.Forms.Button();
            this.colbutton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.groupBox1.Controls.Add(this.gamblebutton);
            this.groupBox1.Location = new System.Drawing.Point(6, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 290);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Single Player";
            // 
            // backtoservers
            // 
            this.backtoservers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.backtoservers.BackColor = System.Drawing.Color.Maroon;
            this.backtoservers.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backtoservers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.backtoservers.Location = new System.Drawing.Point(3, 308);
            this.backtoservers.Name = "backtoservers";
            this.backtoservers.Size = new System.Drawing.Size(409, 31);
            this.backtoservers.TabIndex = 2;
            this.backtoservers.Text = "Cancel Game Creation";
            this.backtoservers.UseVisualStyleBackColor = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.groupBox2.Controls.Add(this.colbutton);
            this.groupBox2.Location = new System.Drawing.Point(212, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 290);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Multi Player";
            // 
            // gamblebutton
            // 
            this.gamblebutton.BackColor = System.Drawing.Color.LightGreen;
            this.gamblebutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gamblebutton.Location = new System.Drawing.Point(6, 19);
            this.gamblebutton.Name = "gamblebutton";
            this.gamblebutton.Size = new System.Drawing.Size(188, 124);
            this.gamblebutton.TabIndex = 0;
            this.gamblebutton.Text = "Gamble Match";
            this.gamblebutton.UseVisualStyleBackColor = false;
            // 
            // colbutton
            // 
            this.colbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.colbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colbutton.Location = new System.Drawing.Point(7, 19);
            this.colbutton.Name = "colbutton";
            this.colbutton.Size = new System.Drawing.Size(188, 124);
            this.colbutton.TabIndex = 1;
            this.colbutton.Text = "Colosseum";
            this.colbutton.UseVisualStyleBackColor = false;
            // 
            // ChooseGameModeWF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 340);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.backtoservers);
            this.Controls.Add(this.groupBox1);
            this.Name = "ChooseGameModeWF";
            this.Text = "ChooseGameModeWF";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button backtoservers;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button gamblebutton;
        private System.Windows.Forms.Button colbutton;
    }
}