using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PhoenixLauncher.Data;

namespace PhoenixLauncher
{
    public partial class QuickLaunchDialog : Form
    {
        private bool started;

        public QuickLaunchDialog()
        {
            InitializeComponent();

            started = false;
        }

        public bool Started
        {
            get { return started; }
        }

        public Server Server
        {
            get
            {
                Server s = new Server("QuickLaunch");
                s.Address = addressBox.Value;
                s.Encryption = serverEncryptionBox.Value;
                s.ClientExe = clientExeBox.Value;
                s.UltimaDir = ultimaDirBox.Value;
                return s;
            }
            set
            {
                addressBox.Value = value.Address;
                serverEncryptionBox.Value = value.Encryption;
                clientExeBox.Value = value.ClientExe;
                ultimaDirBox.Value = value.UltimaDir;
            }
        }

        public Account Account
        {
            get
            {
                Account a = new Account(accountBox.Value);
                a.Password = passwordBox.Value;
                return a;
            }
            set
            {
                accountBox.Value = value.Name;
                passwordBox.Value = value.Password;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawLine(SystemPens.ActiveBorder, 13, 98, 197, 98);
            e.Graphics.DrawLine(SystemPens.ActiveBorder, 13, 189, 197, 189);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            started = false;
            Close();
        }

        private void launchButton_Click(object sender, EventArgs e)
        {
            Launcher dlg = new Launcher();
            dlg.Server = Server;
            dlg.Account = Account;

            dlg.ShowDialog();

            started = dlg.Success;
            Close();
        }
    }
}