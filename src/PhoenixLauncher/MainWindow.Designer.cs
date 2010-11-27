namespace PhoenixLauncher
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.exitButton = new System.Windows.Forms.Button();
            this.launchButton = new System.Windows.Forms.Button();
            this.quickLaunchButton = new System.Windows.Forms.Button();
            this.details = new PhoenixLauncher.Controls.Details();
            this.accountList = new PhoenixLauncher.Controls.AccountList();
            this.serverList = new PhoenixLauncher.Controls.ServerList();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Server list:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(179, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Account list:";
            // 
            // exitButton
            // 
            this.exitButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.exitButton.Location = new System.Drawing.Point(12, 295);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 5;
            this.exitButton.Text = "Exit";
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // launchButton
            // 
            this.launchButton.Enabled = false;
            this.launchButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.launchButton.Location = new System.Drawing.Point(263, 295);
            this.launchButton.Name = "launchButton";
            this.launchButton.Size = new System.Drawing.Size(75, 23);
            this.launchButton.TabIndex = 6;
            this.launchButton.Text = "Launch";
            this.launchButton.Click += new System.EventHandler(this.launchButton_Click);
            // 
            // quickLaunchButton
            // 
            this.quickLaunchButton.Location = new System.Drawing.Point(178, 295);
            this.quickLaunchButton.Name = "quickLaunchButton";
            this.quickLaunchButton.Size = new System.Drawing.Size(79, 23);
            this.quickLaunchButton.TabIndex = 8;
            this.quickLaunchButton.Text = "QuickLaunch";
            this.quickLaunchButton.Click += new System.EventHandler(this.quickLaunchButton_Click);
            // 
            // details
            // 
            this.details.Location = new System.Drawing.Point(13, 152);
            this.details.Name = "details";
            this.details.Size = new System.Drawing.Size(325, 137);
            this.details.TabIndex = 4;
            // 
            // accountList
            // 
            this.accountList.Location = new System.Drawing.Point(178, 25);
            this.accountList.Name = "accountList";
            this.accountList.Settings = null;
            this.accountList.Size = new System.Drawing.Size(160, 121);
            this.accountList.TabIndex = 1;
            this.accountList.SelectedAccountChanged += new System.EventHandler(this.accountList_SelectedAccountChanged);
            // 
            // serverList
            // 
            this.serverList.Location = new System.Drawing.Point(12, 25);
            this.serverList.Name = "serverList";
            this.serverList.Settings = null;
            this.serverList.Size = new System.Drawing.Size(160, 121);
            this.serverList.TabIndex = 0;
            this.serverList.SelectedServerChanged += new System.EventHandler(this.serverList_SelectedServerChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 330);
            this.Controls.Add(this.accountList);
            this.Controls.Add(this.serverList);
            this.Controls.Add(this.quickLaunchButton);
            this.Controls.Add(this.launchButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.details);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phoenix Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button launchButton;
        private System.Windows.Forms.Button quickLaunchButton;
        private PhoenixLauncher.Controls.Details details;
        private PhoenixLauncher.Controls.AccountList accountList;
        private PhoenixLauncher.Controls.ServerList serverList;

    }
}