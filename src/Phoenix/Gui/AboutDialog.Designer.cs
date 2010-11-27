namespace Phoenix.Gui
{
    partial class AboutDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.licencesListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.licenceBox = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.versionBox = new System.Windows.Forms.Label();
            this.iconBox1 = new Phoenix.Gui.Controls.IconBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.copyrightBox = new System.Windows.Forms.Label();
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // licencesListBox
            // 
            this.licencesListBox.FormattingEnabled = true;
            this.licencesListBox.Items.AddRange(new object[] {
            "Phoenix",
            "UOEncryption",
            "MagicLibrary",
            "XPTable"});
            this.licencesListBox.Location = new System.Drawing.Point(12, 144);
            this.licencesListBox.Name = "licencesListBox";
            this.licencesListBox.Size = new System.Drawing.Size(123, 95);
            this.licencesListBox.TabIndex = 1;
            this.licencesListBox.SelectedIndexChanged += new System.EventHandler(this.licencesListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "View licence:";
            // 
            // licenceBox
            // 
            this.licenceBox.BackColor = System.Drawing.SystemColors.Window;
            this.licenceBox.Location = new System.Drawing.Point(152, 144);
            this.licenceBox.Name = "licenceBox";
            this.licenceBox.ReadOnly = true;
            this.licenceBox.Size = new System.Drawing.Size(323, 174);
            this.licenceBox.TabIndex = 3;
            this.licenceBox.Text = resources.GetString("licenceBox.Text");
            this.licenceBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.licenceBox_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(151, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Licence details:";
            // 
            // versionBox
            // 
            this.versionBox.AutoSize = true;
            this.versionBox.BackColor = System.Drawing.Color.Transparent;
            this.versionBox.Location = new System.Drawing.Point(151, 68);
            this.versionBox.Name = "versionBox";
            this.versionBox.Size = new System.Drawing.Size(42, 13);
            this.versionBox.TabIndex = 7;
            this.versionBox.Text = "Version";
            // 
            // iconBox1
            // 
            this.iconBox1.BackColor = System.Drawing.Color.Transparent;
            this.iconBox1.BackgroundImage = null;
            this.iconBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.iconBox1.ForeColor = System.Drawing.Color.Transparent;
            this.iconBox1.Icon = ((System.Drawing.Icon)(resources.GetObject("iconBox1.Icon")));
            this.iconBox1.Location = new System.Drawing.Point(0, 0);
            this.iconBox1.Margin = new System.Windows.Forms.Padding(0);
            this.iconBox1.Name = "iconBox1";
            this.iconBox1.Size = new System.Drawing.Size(135, 125);
            this.iconBox1.Stretch = true;
            this.iconBox1.TabIndex = 0;
            this.iconBox1.Text = "IconBox";
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(397, 324);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 8;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::Phoenix.Properties.Resources.LogoText;
            this.pictureBox1.Location = new System.Drawing.Point(143, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(187, 54);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // copyrightBox
            // 
            this.copyrightBox.BackColor = System.Drawing.Color.Transparent;
            this.copyrightBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.copyrightBox.Location = new System.Drawing.Point(151, 127);
            this.copyrightBox.Name = "copyrightBox";
            this.copyrightBox.Size = new System.Drawing.Size(324, 13);
            this.copyrightBox.TabIndex = 10;
            this.copyrightBox.Text = "Copyright";
            // 
            // linkLabel
            // 
            this.linkLabel.AutoSize = true;
            this.linkLabel.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel.LinkColor = System.Drawing.Color.Blue;
            this.linkLabel.Location = new System.Drawing.Point(151, 84);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Size = new System.Drawing.Size(248, 13);
            this.linkLabel.TabIndex = 11;
            this.linkLabel.TabStop = true;
            this.linkLabel.Text = "http://www.amonthia.com/wiki/index.php/Phoenix";
            this.linkLabel.Click += new System.EventHandler(this.linkLabel_Click);
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(487, 359);
            this.Controls.Add(this.linkLabel);
            this.Controls.Add(this.copyrightBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.versionBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.licenceBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.licencesListBox);
            this.Controls.Add(this.iconBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Phoenix";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Phoenix.Gui.Controls.IconBox iconBox1;
        private System.Windows.Forms.ListBox licencesListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox licenceBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label versionBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label copyrightBox;
        private System.Windows.Forms.LinkLabel linkLabel;

    }
}
