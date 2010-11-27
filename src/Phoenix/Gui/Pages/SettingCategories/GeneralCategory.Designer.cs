namespace Phoenix.Gui.Pages.SettingCategories
{
    partial class GeneralCategory
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralCategory));
            this.changeProfileButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.profileLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.disabledLimiterBox = new System.Windows.Forms.RadioButton();
            this.fps120Box = new System.Windows.Forms.RadioButton();
            this.fps500Box = new System.Windows.Forms.RadioButton();
            this.topMostCheckBox = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.stayOnTopCheckBox = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.labelLine2 = new Phoenix.Gui.Controls.LabelLine();
            this.labelLine1 = new Phoenix.Gui.Controls.LabelLine();
            this.labelLine3 = new Phoenix.Gui.Controls.LabelLine();
            this.label4 = new System.Windows.Forms.Label();
            this.minimizeToTray = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.showInTray = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.showInTaskbar = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.speechColorBox = new Phoenix.Gui.Controls.ColorNumBox();
            this.consoleColorBox = new Phoenix.Gui.Controls.ColorNumBox();
            this.overrideSpeechCheckBox = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.SuspendLayout();
            // 
            // changeProfileButton
            // 
            resources.ApplyResources(this.changeProfileButton, "changeProfileButton");
            this.changeProfileButton.Name = "changeProfileButton";
            this.changeProfileButton.Click += new System.EventHandler(this.changeProfileButton_Click);
            // 
            // saveButton
            // 
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.toolTip.SetToolTip(this.saveButton, resources.GetString("saveButton.ToolTip"));
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // profileLabel
            // 
            resources.ApplyResources(this.profileLabel, "profileLabel");
            this.profileLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.profileLabel.Name = "profileLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // disabledLimiterBox
            // 
            resources.ApplyResources(this.disabledLimiterBox, "disabledLimiterBox");
            this.disabledLimiterBox.Checked = true;
            this.disabledLimiterBox.Name = "disabledLimiterBox";
            this.disabledLimiterBox.TabStop = true;
            this.toolTip.SetToolTip(this.disabledLimiterBox, resources.GetString("disabledLimiterBox.ToolTip"));
            this.disabledLimiterBox.UseVisualStyleBackColor = true;
            this.disabledLimiterBox.CheckedChanged += new System.EventHandler(this.fps_CheckedChanged);
            // 
            // fps120Box
            // 
            resources.ApplyResources(this.fps120Box, "fps120Box");
            this.fps120Box.Name = "fps120Box";
            this.toolTip.SetToolTip(this.fps120Box, resources.GetString("fps120Box.ToolTip"));
            this.fps120Box.UseVisualStyleBackColor = true;
            this.fps120Box.CheckedChanged += new System.EventHandler(this.fps_CheckedChanged);
            // 
            // fps500Box
            // 
            resources.ApplyResources(this.fps500Box, "fps500Box");
            this.fps500Box.Name = "fps500Box";
            this.toolTip.SetToolTip(this.fps500Box, resources.GetString("fps500Box.ToolTip"));
            this.fps500Box.UseVisualStyleBackColor = true;
            this.fps500Box.CheckedChanged += new System.EventHandler(this.fps_CheckedChanged);
            // 
            // topMostCheckBox
            // 
            resources.ApplyResources(this.topMostCheckBox, "topMostCheckBox");
            this.topMostCheckBox.Name = "topMostCheckBox";
            this.topMostCheckBox.SettingEntry = null;
            this.toolTip.SetToolTip(this.topMostCheckBox, resources.GetString("topMostCheckBox.ToolTip"));
            // 
            // stayOnTopCheckBox
            // 
            resources.ApplyResources(this.stayOnTopCheckBox, "stayOnTopCheckBox");
            this.stayOnTopCheckBox.Name = "stayOnTopCheckBox";
            this.stayOnTopCheckBox.SettingEntry = null;
            this.toolTip.SetToolTip(this.stayOnTopCheckBox, resources.GetString("stayOnTopCheckBox.ToolTip"));
            // 
            // labelLine2
            // 
            resources.ApplyResources(this.labelLine2, "labelLine2");
            this.labelLine2.ForeColor = System.Drawing.Color.RoyalBlue;
            this.labelLine2.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.labelLine2.Name = "labelLine2";
            // 
            // labelLine1
            // 
            resources.ApplyResources(this.labelLine1, "labelLine1");
            this.labelLine1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.labelLine1.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.labelLine1.Name = "labelLine1";
            // 
            // labelLine3
            // 
            resources.ApplyResources(this.labelLine3, "labelLine3");
            this.labelLine3.ForeColor = System.Drawing.Color.RoyalBlue;
            this.labelLine3.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.labelLine3.Name = "labelLine3";
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.SystemColors.GrayText;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // minimizeToTray
            // 
            resources.ApplyResources(this.minimizeToTray, "minimizeToTray");
            this.minimizeToTray.Name = "minimizeToTray";
            this.minimizeToTray.SettingEntry = null;
            this.minimizeToTray.UseVisualStyleBackColor = true;
            // 
            // showInTray
            // 
            resources.ApplyResources(this.showInTray, "showInTray");
            this.showInTray.Name = "showInTray";
            this.showInTray.SettingEntry = null;
            this.showInTray.UseVisualStyleBackColor = true;
            // 
            // showInTaskbar
            // 
            resources.ApplyResources(this.showInTaskbar, "showInTaskbar");
            this.showInTaskbar.Name = "showInTaskbar";
            this.showInTaskbar.SettingEntry = null;
            this.showInTaskbar.UseVisualStyleBackColor = true;
            // 
            // speechColorBox
            // 
            resources.ApplyResources(this.speechColorBox, "speechColorBox");
            this.speechColorBox.Name = "speechColorBox";
            this.speechColorBox.Value = ((ushort)(1));
            this.speechColorBox.ValueChanged += new System.EventHandler(this.speechColorBox_ValueChanged);
            // 
            // consoleColorBox
            // 
            resources.ApplyResources(this.consoleColorBox, "consoleColorBox");
            this.consoleColorBox.Name = "consoleColorBox";
            this.consoleColorBox.Value = ((ushort)(1));
            this.consoleColorBox.ValueChanged += new System.EventHandler(this.consoleColorBox_ValueChanged);
            // 
            // overrideSpeechCheckBox
            // 
            resources.ApplyResources(this.overrideSpeechCheckBox, "overrideSpeechCheckBox");
            this.overrideSpeechCheckBox.Name = "overrideSpeechCheckBox";
            this.overrideSpeechCheckBox.SettingEntry = null;
            // 
            // GeneralCategory
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fps500Box);
            this.Controls.Add(this.fps120Box);
            this.Controls.Add(this.disabledLimiterBox);
            this.Controls.Add(this.labelLine3);
            this.Controls.Add(this.minimizeToTray);
            this.Controls.Add(this.showInTray);
            this.Controls.Add(this.showInTaskbar);
            this.Controls.Add(this.speechColorBox);
            this.Controls.Add(this.consoleColorBox);
            this.Controls.Add(this.profileLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.topMostCheckBox);
            this.Controls.Add(this.stayOnTopCheckBox);
            this.Controls.Add(this.labelLine2);
            this.Controls.Add(this.overrideSpeechCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelLine1);
            this.Controls.Add(this.changeProfileButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label4);
            this.Name = "GeneralCategory";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button changeProfileButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label profileLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip;
        private Phoenix.Gui.Controls.ColorNumBox speechColorBox;
        private Phoenix.Gui.Controls.ColorNumBox consoleColorBox;
        private Phoenix.Gui.Controls.SettingEntryCheckBox topMostCheckBox;
        private Phoenix.Gui.Controls.SettingEntryCheckBox stayOnTopCheckBox;
        private Phoenix.Gui.Controls.LabelLine labelLine2;
        private Phoenix.Gui.Controls.SettingEntryCheckBox overrideSpeechCheckBox;
        private Phoenix.Gui.Controls.LabelLine labelLine1;
        private Phoenix.Gui.Controls.SettingEntryCheckBox showInTaskbar;
        private Phoenix.Gui.Controls.SettingEntryCheckBox showInTray;
        private Phoenix.Gui.Controls.SettingEntryCheckBox minimizeToTray;
        private Phoenix.Gui.Controls.LabelLine labelLine3;
        private System.Windows.Forms.RadioButton disabledLimiterBox;
        private System.Windows.Forms.RadioButton fps120Box;
        private System.Windows.Forms.RadioButton fps500Box;
        private System.Windows.Forms.Label label4;
    }
}
