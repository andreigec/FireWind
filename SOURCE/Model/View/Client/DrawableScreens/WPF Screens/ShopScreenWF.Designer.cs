namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    partial class ShopScreenWF
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
            this.gobacktobase = new System.Windows.Forms.Button();
            this.buybutton = new System.Windows.Forms.Button();
            this.nextbutton = new System.Windows.Forms.Button();
            this.prevbutton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.slotbox = new System.Windows.Forms.ListBox();
            this.showitem = new Project.View.Client.DrawableScreens.WPF_Screens.PurchasableItemWFControl();
            this.SuspendLayout();
            // 
            // gobacktobase
            // 
            this.gobacktobase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gobacktobase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.gobacktobase.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gobacktobase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.gobacktobase.Location = new System.Drawing.Point(3, 468);
            this.gobacktobase.Name = "gobacktobase";
            this.gobacktobase.Size = new System.Drawing.Size(203, 31);
            this.gobacktobase.TabIndex = 2;
            this.gobacktobase.Text = global::Project.Localisation.BackToBase;
            this.gobacktobase.UseVisualStyleBackColor = false;
            // 
            // buybutton
            // 
            this.buybutton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buybutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.buybutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buybutton.ForeColor = System.Drawing.Color.Navy;
            this.buybutton.Location = new System.Drawing.Point(308, 469);
            this.buybutton.MinimumSize = new System.Drawing.Size(120, 29);
            this.buybutton.Name = "buybutton";
            this.buybutton.Size = new System.Drawing.Size(228, 29);
            this.buybutton.TabIndex = 5;
            this.buybutton.Text = global::Project.Localisation.BuyThisItem;
            this.buybutton.UseVisualStyleBackColor = false;
            // 
            // nextbutton
            // 
            this.nextbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nextbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.nextbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextbutton.Location = new System.Drawing.Point(542, 173);
            this.nextbutton.Name = "nextbutton";
            this.nextbutton.Size = new System.Drawing.Size(82, 78);
            this.nextbutton.TabIndex = 18;
            this.nextbutton.Text = "Next Item";
            this.nextbutton.UseVisualStyleBackColor = false;
            // 
            // prevbutton
            // 
            this.prevbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.prevbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prevbutton.Location = new System.Drawing.Point(212, 173);
            this.prevbutton.Name = "prevbutton";
            this.prevbutton.Size = new System.Drawing.Size(90, 78);
            this.prevbutton.TabIndex = 17;
            this.prevbutton.Text = "Previous Item";
            this.prevbutton.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 24);
            this.label1.TabIndex = 16;
            this.label1.Text = "Slots";
            // 
            // slotbox
            // 
            this.slotbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.slotbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.slotbox.FormattingEnabled = true;
            this.slotbox.ItemHeight = 20;
            this.slotbox.Location = new System.Drawing.Point(16, 36);
            this.slotbox.Name = "slotbox";
            this.slotbox.Size = new System.Drawing.Size(182, 424);
            this.slotbox.TabIndex = 15;
            // 
            // showitem
            // 
            this.showitem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.showitem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.showitem.Location = new System.Drawing.Point(308, 9);
            this.showitem.MaximumSize = new System.Drawing.Size(440, 600);
            this.showitem.MinimumSize = new System.Drawing.Size(220, 440);
            this.showitem.Name = "showitem";
            this.showitem.Size = new System.Drawing.Size(220, 454);
            this.showitem.TabIndex = 14;
            // 
            // ShopScreenWF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 502);
            this.Controls.Add(this.nextbutton);
            this.Controls.Add(this.prevbutton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.slotbox);
            this.Controls.Add(this.showitem);
            this.Controls.Add(this.buybutton);
            this.Controls.Add(this.gobacktobase);
            this.Name = "ShopScreenWF";
            this.Text = "ShopScreenWF";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button gobacktobase;
        private System.Windows.Forms.Button buybutton;
        private System.Windows.Forms.Button nextbutton;
        private System.Windows.Forms.Button prevbutton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox slotbox;
        private PurchasableItemWFControl showitem;
    }
}