namespace Phoenix.Gui.Controls
{
    partial class UOSelectColorDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.brightnessBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.previewIdBox = new Phoenix.Gui.Controls.NumTextBox();
            this.artImageControl = new Phoenix.Gui.Controls.ArtImageControl();
            this.label4 = new System.Windows.Forms.Label();
            this.colorNameLabel = new System.Windows.Forms.Label();
            this.extendedCheckBox = new System.Windows.Forms.CheckBox();
            this.hueSpectrum = new Phoenix.Gui.Controls.HueSpectrum();
            this.huesField = new Phoenix.Gui.Controls.HuesField();
            this.colorIndexBox = new Phoenix.Gui.Controls.NumTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 421);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Selected Color:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(499, 443);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(126, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(499, 416);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(126, 23);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            // 
            // brightnessBar
            // 
            this.brightnessBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.brightnessBar.LargeChange = 4;
            this.brightnessBar.Location = new System.Drawing.Point(494, 51);
            this.brightnessBar.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.brightnessBar.Maximum = 31;
            this.brightnessBar.Name = "brightnessBar";
            this.brightnessBar.Size = new System.Drawing.Size(138, 45);
            this.brightnessBar.TabIndex = 12;
            this.brightnessBar.TickFrequency = 4;
            this.brightnessBar.Value = 24;
            this.brightnessBar.ValueChanged += new System.EventHandler(this.brightnessBar_ValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(498, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Table Brightness:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.previewIdBox);
            this.groupBox1.Controls.Add(this.artImageControl);
            this.groupBox1.Location = new System.Drawing.Point(499, 90);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(126, 322);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preview";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Item ID:";
            // 
            // previewIdBox
            // 
            this.previewIdBox.FormatString = "X4";
            this.previewIdBox.Location = new System.Drawing.Point(56, 19);
            this.previewIdBox.Name = "previewIdBox";
            this.previewIdBox.ShowHexPrefix = true;
            this.previewIdBox.Size = new System.Drawing.Size(64, 20);
            this.previewIdBox.TabIndex = 1;
            this.previewIdBox.Text = "0x0000";
            this.previewIdBox.ValueChanged += new System.EventHandler(this.previewIdBox_ValueChanged);
            // 
            // artImageControl
            // 
            this.artImageControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.artImageControl.ArtData = null;
            this.artImageControl.BackgroundImage = null;
            this.artImageControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.artImageControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.artImageControl.Hues = null;
            this.artImageControl.ImageAlignment = Phoenix.Gui.Controls.ImageAlignment.Top;
            this.artImageControl.Location = new System.Drawing.Point(6, 48);
            this.artImageControl.Name = "artImageControl";
            this.artImageControl.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.artImageControl.Size = new System.Drawing.Size(114, 268);
            this.artImageControl.TabIndex = 0;
            this.artImageControl.Text = "artImageControl";
            this.artImageControl.UseHue = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(155, 421);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Name:";
            // 
            // colorNameLabel
            // 
            this.colorNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.colorNameLabel.AutoSize = true;
            this.colorNameLabel.Location = new System.Drawing.Point(195, 421);
            this.colorNameLabel.Name = "colorNameLabel";
            this.colorNameLabel.Size = new System.Drawing.Size(59, 13);
            this.colorNameLabel.TabIndex = 18;
            this.colorNameLabel.Text = "color name";
            // 
            // extendedCheckBox
            // 
            this.extendedCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.extendedCheckBox.AutoSize = true;
            this.extendedCheckBox.Checked = true;
            this.extendedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.extendedCheckBox.Location = new System.Drawing.Point(495, 12);
            this.extendedCheckBox.Name = "extendedCheckBox";
            this.extendedCheckBox.Size = new System.Drawing.Size(71, 17);
            this.extendedCheckBox.TabIndex = 21;
            this.extendedCheckBox.Text = "Extended";
            this.extendedCheckBox.CheckedChanged += new System.EventHandler(this.extendedCheckBox_CheckedChanged);
            // 
            // hueSpectrum
            // 
            this.hueSpectrum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hueSpectrum.BackgroundImage = null;
            this.hueSpectrum.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hueSpectrum.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.hueSpectrum.Hues = null;
            this.hueSpectrum.Location = new System.Drawing.Point(13, 442);
            this.hueSpectrum.Name = "hueSpectrum";
            this.hueSpectrum.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.hueSpectrum.Size = new System.Drawing.Size(480, 23);
            this.hueSpectrum.TabIndex = 20;
            this.hueSpectrum.Text = "hueSpectrum";
            // 
            // huesField
            // 
            this.huesField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.huesField.BackgroundImage = null;
            this.huesField.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.huesField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.huesField.Hues = null;
            this.huesField.HueSize = new System.Drawing.Size(8, 8);
            this.huesField.Location = new System.Drawing.Point(13, 12);
            this.huesField.Name = "huesField";
            this.huesField.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.huesField.SelectedColorIndex = 0;
            this.huesField.ShowAllColors = true;
            this.huesField.Size = new System.Drawing.Size(480, 400);
            this.huesField.TabIndex = 19;
            this.huesField.Text = "huesField";
            this.huesField.SelectedColorIndexChanged += new System.EventHandler(this.huesField_SelectedColorIndexChanged);
            // 
            // colorIndexBox
            // 
            this.colorIndexBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.colorIndexBox.FormatString = "X4";
            this.colorIndexBox.Location = new System.Drawing.Point(93, 418);
            this.colorIndexBox.MaxValue = 3000;
            this.colorIndexBox.Name = "colorIndexBox";
            this.colorIndexBox.ShowHexPrefix = true;
            this.colorIndexBox.Size = new System.Drawing.Size(50, 20);
            this.colorIndexBox.TabIndex = 9;
            this.colorIndexBox.Text = "0x0000";
            this.colorIndexBox.ValueChanged += new System.EventHandler(this.colorIndexBox_ValueChanged);
            // 
            // UOSelectColorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 477);
            this.Controls.Add(this.extendedCheckBox);
            this.Controls.Add(this.hueSpectrum);
            this.Controls.Add(this.huesField);
            this.Controls.Add(this.colorNameLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.brightnessBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.colorIndexBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UOSelectColorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select Color..";
            ((System.ComponentModel.ISupportInitialize)(this.brightnessBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TrackBar brightnessBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label colorNameLabel;
        private System.Windows.Forms.CheckBox extendedCheckBox;
        private System.Windows.Forms.Label label3;
        private NumTextBox previewIdBox;
        private ArtImageControl artImageControl;
        private HueSpectrum hueSpectrum;
        private HuesField huesField;
        private NumTextBox colorIndexBox;

    }
}