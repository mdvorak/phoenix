namespace Phoenix.Gui
{
    partial class PhoenixWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhoenixWindow));
            this.phoenixTabControl = new Phoenix.Gui.PhoenixTabControl();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tODOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.terminateAllStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // phoenixTabControl
            // 
            this.phoenixTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.phoenixTabControl.Location = new System.Drawing.Point(0, 0);
            this.phoenixTabControl.Name = "phoenixTabControl";
            this.phoenixTabControl.Size = new System.Drawing.Size(288, 311);
            this.phoenixTabControl.TabIndex = 0;
            this.phoenixTabControl.Text = "phoenixTabControl1";
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipTitle = "Phoenix";
            this.trayIcon.ContextMenuStrip = this.trayMenuStrip;
            this.trayIcon.Text = "Phoenix";
            this.trayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClick);
            // 
            // trayMenuStrip
            // 
            this.trayMenuStrip.Enabled = true;
            this.trayMenuStrip.GripMargin = new System.Windows.Forms.Padding(2);
            this.trayMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tODOToolStripMenuItem,
            this.showWindowToolStripMenuItem,
            this.toolStripSeparator1,
            this.terminateAllStripMenuItem});
            this.trayMenuStrip.Location = new System.Drawing.Point(25, 59);
            this.trayMenuStrip.Name = "trayMenuStrip";
            this.trayMenuStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.trayMenuStrip.Size = new System.Drawing.Size(131, 76);
            // 
            // tODOToolStripMenuItem
            // 
            this.tODOToolStripMenuItem.Enabled = false;
            this.tODOToolStripMenuItem.Name = "tODOToolStripMenuItem";
            this.tODOToolStripMenuItem.Text = "TODO";
            // 
            // showWindowToolStripMenuItem
            // 
            this.showWindowToolStripMenuItem.Name = "showWindowToolStripMenuItem";
            this.showWindowToolStripMenuItem.Text = "Show Window";
            this.showWindowToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // terminateAllStripMenuItem
            // 
            this.terminateAllStripMenuItem.Name = "terminateAllStripMenuItem";
            this.terminateAllStripMenuItem.Text = "Terminate All";
            this.terminateAllStripMenuItem.Click += new System.EventHandler(this.terminateAllStripMenuItem_Click);
            // 
            // PhoenixWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 311);
            this.CloseButton = false;
            this.Controls.Add(this.phoenixTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "PhoenixWindow";
            this.Text = "Phoenix";
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Deactivate += new System.EventHandler(this.PhoenixWindow_Deactivate);
            this.Activated += new System.EventHandler(this.PhoenixWindow_Activated);
            this.trayMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PhoenixTabControl phoenixTabControl;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip trayMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem terminateAllStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tODOToolStripMenuItem;

    }
}