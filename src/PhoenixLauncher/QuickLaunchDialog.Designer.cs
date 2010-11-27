namespace PhoenixLauncher
{
    partial class QuickLaunchDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuickLaunchDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.launchButton = new System.Windows.Forms.Button();
            this.ultimaDirBox = new PhoenixLauncher.Controls.UltimaDirBox();
            this.clientExeBox = new PhoenixLauncher.Controls.ClientExeBox();
            this.passwordBox = new PhoenixLauncher.Controls.PasswordBox();
            this.accountBox = new PhoenixLauncher.Controls.AccountBox();
            this.serverEncryptionBox = new PhoenixLauncher.Controls.ServerEncryptionBox();
            this.addressBox = new PhoenixLauncher.Controls.AddressBox();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // launchButton
            // 
            resources.ApplyResources(this.launchButton, "launchButton");
            this.launchButton.Name = "launchButton";
            this.launchButton.Click += new System.EventHandler(this.launchButton_Click);
            // 
            // ultimaDirBox
            // 
            resources.ApplyResources(this.ultimaDirBox, "ultimaDirBox");
            this.ultimaDirBox.MaximumSize = new System.Drawing.Size(352, 36);
            this.ultimaDirBox.MinimumSize = new System.Drawing.Size(50, 36);
            this.ultimaDirBox.Name = "ultimaDirBox";
            this.ultimaDirBox.Value = "";
            // 
            // clientExeBox
            // 
            resources.ApplyResources(this.clientExeBox, "clientExeBox");
            this.clientExeBox.MaximumSize = new System.Drawing.Size(352, 36);
            this.clientExeBox.MinimumSize = new System.Drawing.Size(50, 36);
            this.clientExeBox.Name = "clientExeBox";
            this.clientExeBox.Value = "";
            // 
            // passwordBox
            // 
            resources.ApplyResources(this.passwordBox, "passwordBox");
            this.passwordBox.MaximumSize = new System.Drawing.Size(352, 36);
            this.passwordBox.MinimumSize = new System.Drawing.Size(50, 36);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.Value = "";
            // 
            // accountBox
            // 
            resources.ApplyResources(this.accountBox, "accountBox");
            this.accountBox.MaximumSize = new System.Drawing.Size(352, 36);
            this.accountBox.MinimumSize = new System.Drawing.Size(50, 36);
            this.accountBox.Name = "accountBox";
            this.accountBox.Value = "";
            // 
            // serverEncryptionBox
            // 
            resources.ApplyResources(this.serverEncryptionBox, "serverEncryptionBox");
            this.serverEncryptionBox.MaximumSize = new System.Drawing.Size(352, 37);
            this.serverEncryptionBox.MinimumSize = new System.Drawing.Size(50, 37);
            this.serverEncryptionBox.Name = "serverEncryptionBox";
            this.serverEncryptionBox.Value = null;
            // 
            // addressBox
            // 
            this.addressBox.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            resources.ApplyResources(this.addressBox, "addressBox");
            this.addressBox.MaximumSize = new System.Drawing.Size(352, 36);
            this.addressBox.MinimumSize = new System.Drawing.Size(50, 36);
            this.addressBox.Name = "addressBox";
            this.addressBox.Value = "";
            // 
            // QuickLaunchDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.launchButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.ultimaDirBox);
            this.Controls.Add(this.clientExeBox);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.accountBox);
            this.Controls.Add(this.serverEncryptionBox);
            this.Controls.Add(this.addressBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuickLaunchDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private PhoenixLauncher.Controls.AddressBox addressBox;
        private PhoenixLauncher.Controls.ServerEncryptionBox serverEncryptionBox;
        private PhoenixLauncher.Controls.AccountBox accountBox;
        private PhoenixLauncher.Controls.PasswordBox passwordBox;
        private PhoenixLauncher.Controls.ClientExeBox clientExeBox;
        private PhoenixLauncher.Controls.UltimaDirBox ultimaDirBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button launchButton;
    }
}