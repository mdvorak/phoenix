namespace Phoenix.Gui.Pages
{
    partial class InfoPage
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
            this.groupsPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // groupsPanel
            // 
            this.groupsPanel.AutoSize = true;
            this.groupsPanel.Location = new System.Drawing.Point(0, 0);
            this.groupsPanel.Name = "groupsPanel";
            this.groupsPanel.Size = new System.Drawing.Size(211, 209);
            this.groupsPanel.TabIndex = 0;
            // 
            // InfoPage
            // 
            this.AutoScroll = true;
            this.Controls.Add(this.groupsPanel);
            this.Name = "InfoPage";
            this.Size = new System.Drawing.Size(211, 209);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel groupsPanel;
    }
}
