namespace PhoenixLauncher.Controls
{
    partial class ServerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ultimaDirBox = new PhoenixLauncher.Controls.UltimaDirBox();
            this.clientExeBox = new PhoenixLauncher.Controls.ClientExeBox();
            this.serverEncryptionBox = new PhoenixLauncher.Controls.ServerEncryptionBox();
            this.addressBox = new PhoenixLauncher.Controls.AddressBox();
            this.nameBox = new PhoenixLauncher.Controls.NameBox();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // ultimaDirBox
            // 
            resources.ApplyResources(this.ultimaDirBox, "ultimaDirBox");
            this.ultimaDirBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ultimaDirBox.MaximumSize = new System.Drawing.Size(352, 36);
            this.ultimaDirBox.MinimumSize = new System.Drawing.Size(50, 36);
            this.ultimaDirBox.Name = "ultimaDirBox";
            this.ultimaDirBox.Value = "";
            // 
            // clientExeBox
            // 
            resources.ApplyResources(this.clientExeBox, "clientExeBox");
            this.clientExeBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.clientExeBox.MaximumSize = new System.Drawing.Size(352, 36);
            this.clientExeBox.MinimumSize = new System.Drawing.Size(50, 36);
            this.clientExeBox.Name = "clientExeBox";
            this.clientExeBox.Value = "";
            // 
            // serverEncryptionBox
            // 
            resources.ApplyResources(this.serverEncryptionBox, "serverEncryptionBox");
            this.serverEncryptionBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.serverEncryptionBox.MaximumSize = new System.Drawing.Size(352, 37);
            this.serverEncryptionBox.MinimumSize = new System.Drawing.Size(50, 37);
            this.serverEncryptionBox.Name = "serverEncryptionBox";
            this.serverEncryptionBox.Value = null;
            // 
            // addressBox
            // 
            resources.ApplyResources(this.addressBox, "addressBox");
            this.addressBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addressBox.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.addressBox.MaximumSize = new System.Drawing.Size(352, 36);
            this.addressBox.MinimumSize = new System.Drawing.Size(50, 36);
            this.addressBox.Name = "addressBox";
            this.addressBox.Value = "";
            // 
            // nameBox
            // 
            resources.ApplyResources(this.nameBox, "nameBox");
            this.nameBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.nameBox.MaximumSize = new System.Drawing.Size(352, 36);
            this.nameBox.MinimumSize = new System.Drawing.Size(50, 36);
            this.nameBox.Name = "nameBox";
            this.nameBox.Value = "";
            // 
            // ServerDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultimaDirBox);
            this.Controls.Add(this.clientExeBox);
            this.Controls.Add(this.serverEncryptionBox);
            this.Controls.Add(this.addressBox);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerDialog";
            this.ShowInTaskbar = false;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ServerDialog_Paint);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ServerDialog_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private PhoenixLauncher.Controls.UltimaDirBox ultimaDirBox;
        private PhoenixLauncher.Controls.ClientExeBox clientExeBox;
        private PhoenixLauncher.Controls.ServerEncryptionBox serverEncryptionBox;
        private PhoenixLauncher.Controls.AddressBox addressBox;
        private PhoenixLauncher.Controls.NameBox nameBox;
    }
}