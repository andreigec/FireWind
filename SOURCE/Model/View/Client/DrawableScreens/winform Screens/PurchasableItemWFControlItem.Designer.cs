namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    partial class PurchasableItemWFControlItem
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
            this.statname = new System.Windows.Forms.Label();
            this.equippeditemstat = new System.Windows.Forms.Label();
            this.newitemstat = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statname
            // 
            this.statname.AutoSize = true;
            this.statname.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statname.ForeColor = System.Drawing.Color.Yellow;
            this.statname.Location = new System.Drawing.Point(0, 0);
            this.statname.Name = "statname";
            this.statname.Size = new System.Drawing.Size(76, 20);
            this.statname.TabIndex = 0;
            this.statname.Text = "statname";
            // 
            // equippeditemstat
            // 
            this.equippeditemstat.AutoSize = true;
            this.equippeditemstat.BackColor = System.Drawing.Color.LightSeaGreen;
            this.equippeditemstat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.equippeditemstat.ForeColor = System.Drawing.Color.Yellow;
            this.equippeditemstat.Location = new System.Drawing.Point(76, 0);
            this.equippeditemstat.Name = "equippeditemstat";
            this.equippeditemstat.Size = new System.Drawing.Size(67, 16);
            this.equippeditemstat.TabIndex = 1;
            this.equippeditemstat.Text = "equipitem";
            // 
            // newitemstat
            // 
            this.newitemstat.AutoSize = true;
            this.newitemstat.BackColor = System.Drawing.Color.DarkSlateGray;
            this.newitemstat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newitemstat.ForeColor = System.Drawing.Color.Yellow;
            this.newitemstat.Location = new System.Drawing.Point(143, 0);
            this.newitemstat.Name = "newitemstat";
            this.newitemstat.Size = new System.Drawing.Size(57, 16);
            this.newitemstat.TabIndex = 2;
            this.newitemstat.Text = "newitem";
            // 
            // PurchasableItemWFControlItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.Controls.Add(this.newitemstat);
            this.Controls.Add(this.equippeditemstat);
            this.Controls.Add(this.statname);
            this.Name = "PurchasableItemWFControlItem";
            this.Size = new System.Drawing.Size(200, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label statname;
        public System.Windows.Forms.Label equippeditemstat;
        public System.Windows.Forms.Label newitemstat;
    }
}
