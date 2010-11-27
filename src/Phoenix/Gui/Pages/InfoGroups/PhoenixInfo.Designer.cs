namespace Phoenix.Gui.Pages.InfoGroups
{
    partial class PhoenixInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhoenixInfo));
            this.phoenixIcon = new Phoenix.Gui.Controls.IconBox();
            this.copyrightBox = new System.Windows.Forms.Label();
            this.versionBox = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // phoenixIcon
            // 
            this.phoenixIcon.BackColor = System.Drawing.Color.Transparent;
            this.phoenixIcon.BackgroundImage = null;
            this.phoenixIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.phoenixIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.phoenixIcon.ForeColor = System.Drawing.Color.Transparent;
            this.phoenixIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("phoenixIcon.Icon")));
            this.phoenixIcon.Location = new System.Drawing.Point(3, 3);
            this.phoenixIcon.Name = "phoenixIcon";
            this.phoenixIcon.Size = new System.Drawing.Size(32, 32);
            this.phoenixIcon.TabIndex = 8;
            this.phoenixIcon.Text = "IconBox";
            this.phoenixIcon.Click += new System.EventHandler(this.phoenixIcon_Click);
            // 
            // copyrightBox
            // 
            this.copyrightBox.AutoSize = true;
            this.copyrightBox.Location = new System.Drawing.Point(41, 22);
            this.copyrightBox.Margin = new System.Windows.Forms.Padding(3);
            this.copyrightBox.Name = "copyrightBox";
            this.copyrightBox.Size = new System.Drawing.Size(182, 13);
            this.copyrightBox.TabIndex = 7;
            this.copyrightBox.Text = "Copyright 2005 Mikee The Wanderer";
            // 
            // versionBox
            // 
            this.versionBox.AutoSize = true;
            this.versionBox.Location = new System.Drawing.Point(134, 3);
            this.versionBox.Margin = new System.Windows.Forms.Padding(3);
            this.versionBox.Name = "versionBox";
            this.versionBox.Size = new System.Drawing.Size(40, 13);
            this.versionBox.TabIndex = 6;
            this.versionBox.Text = "0.0.0.0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Assembly version:";
            // 
            // PhoenixInfo
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.phoenixIcon);
            this.Controls.Add(this.copyrightBox);
            this.Controls.Add(this.versionBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "PhoenixInfo";
            this.Size = new System.Drawing.Size(228, 40);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Phoenix.Gui.Controls.IconBox phoenixIcon;
        private System.Windows.Forms.Label copyrightBox;
        private System.Windows.Forms.Label versionBox;
        private System.Windows.Forms.Label label1;

    }
}
