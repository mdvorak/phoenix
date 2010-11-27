namespace Phoenix.Gui
{
    partial class SelectProfileDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectProfileDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.profileListBox = new System.Windows.Forms.ListBox();
            this.newProfileTextBox = new System.Windows.Forms.TextBox();
            this.selectRadioButton = new System.Windows.Forms.RadioButton();
            this.createRadioButton = new System.Windows.Forms.RadioButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(237, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "Profile is set of configuration, i.e. hotkeys, colors, window position etc.";
            // 
            // profileListBox
            // 
            this.profileListBox.Enabled = false;
            this.profileListBox.FormattingEnabled = true;
            this.profileListBox.Location = new System.Drawing.Point(93, 70);
            this.profileListBox.Name = "profileListBox";
            this.profileListBox.Size = new System.Drawing.Size(156, 95);
            this.profileListBox.TabIndex = 7;
            this.toolTip.SetToolTip(this.profileListBox, "List of existing profiles.");
            this.profileListBox.SelectedIndexChanged += new System.EventHandler(this.profileListBox_SelectedIndexChanged);
            // 
            // newProfileTextBox
            // 
            this.newProfileTextBox.Location = new System.Drawing.Point(93, 44);
            this.newProfileTextBox.MaxLength = 32;
            this.newProfileTextBox.Name = "newProfileTextBox";
            this.newProfileTextBox.Size = new System.Drawing.Size(156, 20);
            this.newProfileTextBox.TabIndex = 6;
            this.newProfileTextBox.Text = "New Profile Name";
            this.toolTip.SetToolTip(this.newProfileTextBox, "Name of new profile.");
            this.newProfileTextBox.TextChanged += new System.EventHandler(this.newProfileTextBox_TextChanged);
            // 
            // selectRadioButton
            // 
            this.selectRadioButton.AutoSize = true;
            this.selectRadioButton.Enabled = false;
            this.selectRadioButton.Location = new System.Drawing.Point(12, 68);
            this.selectRadioButton.Name = "selectRadioButton";
            this.selectRadioButton.Size = new System.Drawing.Size(55, 17);
            this.selectRadioButton.TabIndex = 5;
            this.selectRadioButton.Text = "Select";
            this.selectRadioButton.CheckedChanged += new System.EventHandler(this.selectRadioButton_CheckedChanged);
            // 
            // createRadioButton
            // 
            this.createRadioButton.AutoSize = true;
            this.createRadioButton.Checked = true;
            this.createRadioButton.Location = new System.Drawing.Point(12, 45);
            this.createRadioButton.Name = "createRadioButton";
            this.createRadioButton.Size = new System.Drawing.Size(79, 17);
            this.createRadioButton.TabIndex = 4;
            this.createRadioButton.TabStop = true;
            this.createRadioButton.Text = "Create new";
            this.toolTip.SetToolTip(this.createRadioButton, "Create new profile of specified name. If profile already exists, you will be prom" +
                    "pted if you want to overwrite it.\r\nNew profile is copy of current profile.");
            this.createRadioButton.CheckedChanged += new System.EventHandler(this.createRadioButton_CheckedChanged);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 6000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // okButton
            // 
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(133, 171);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Enabled = false;
            this.cancelButton.Location = new System.Drawing.Point(52, 171);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // SelectProfileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 206);
            this.CloseButton = false;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.profileListBox);
            this.Controls.Add(this.newProfileTextBox);
            this.Controls.Add(this.selectRadioButton);
            this.Controls.Add(this.createRadioButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectProfileDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Profile..";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SelectProfileDialog_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox profileListBox;
        private System.Windows.Forms.TextBox newProfileTextBox;
        private System.Windows.Forms.RadioButton selectRadioButton;
        private System.Windows.Forms.RadioButton createRadioButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}