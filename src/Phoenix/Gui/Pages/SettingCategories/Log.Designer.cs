namespace Phoenix.Gui.Pages.SettingCategories
{
    partial class Log
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Log));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.labelLine1 = new Phoenix.Gui.Controls.LabelLine();
            this.packetLoggingBox = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.serverPhoenixBox = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.phoenixClientBox = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.phoenixServerBox = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.clientPhoenixBox = new Phoenix.Gui.Controls.SettingEntryCheckBox();
            this.SuspendLayout();
            // 
            // labelLine1
            // 
            this.labelLine1.ForeColor = System.Drawing.Color.RoyalBlue;
            resources.ApplyResources(this.labelLine1, "labelLine1");
            this.labelLine1.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.labelLine1.Name = "labelLine1";
            this.toolTip.SetToolTip(this.labelLine1, resources.GetString("labelLine1.ToolTip"));
            // 
            // packetLoggingBox
            // 
            resources.ApplyResources(this.packetLoggingBox, "packetLoggingBox");
            this.packetLoggingBox.Name = "packetLoggingBox";
            this.packetLoggingBox.SettingEntry = null;
            this.toolTip.SetToolTip(this.packetLoggingBox, resources.GetString("packetLoggingBox.ToolTip"));
            // 
            // serverPhoenixBox
            // 
            resources.ApplyResources(this.serverPhoenixBox, "serverPhoenixBox");
            this.serverPhoenixBox.Name = "serverPhoenixBox";
            this.serverPhoenixBox.SettingEntry = null;
            // 
            // phoenixClientBox
            // 
            resources.ApplyResources(this.phoenixClientBox, "phoenixClientBox");
            this.phoenixClientBox.Name = "phoenixClientBox";
            this.phoenixClientBox.SettingEntry = null;
            // 
            // phoenixServerBox
            // 
            resources.ApplyResources(this.phoenixServerBox, "phoenixServerBox");
            this.phoenixServerBox.Name = "phoenixServerBox";
            this.phoenixServerBox.SettingEntry = null;
            // 
            // clientPhoenixBox
            // 
            resources.ApplyResources(this.clientPhoenixBox, "clientPhoenixBox");
            this.clientPhoenixBox.Name = "clientPhoenixBox";
            this.clientPhoenixBox.SettingEntry = null;
            // 
            // Log
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.serverPhoenixBox);
            this.Controls.Add(this.phoenixClientBox);
            this.Controls.Add(this.phoenixServerBox);
            this.Controls.Add(this.clientPhoenixBox);
            this.Controls.Add(this.labelLine1);
            this.Controls.Add(this.packetLoggingBox);
            this.Name = "Log";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private Phoenix.Gui.Controls.SettingEntryCheckBox serverPhoenixBox;
        private Phoenix.Gui.Controls.SettingEntryCheckBox phoenixClientBox;
        private Phoenix.Gui.Controls.SettingEntryCheckBox phoenixServerBox;
        private Phoenix.Gui.Controls.SettingEntryCheckBox clientPhoenixBox;
        private Phoenix.Gui.Controls.LabelLine labelLine1;
        private Phoenix.Gui.Controls.SettingEntryCheckBox packetLoggingBox;
    }
}
