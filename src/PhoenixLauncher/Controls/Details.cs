using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PhoenixLauncher.Data;

namespace PhoenixLauncher.Controls
{
    public partial class Details : UserControl
    {
        public Details()
        {
            InitializeComponent();
        }

        public Server ServerData
        {
            set
            {
                serverLabel.Text = value.Name;
                addressLabel.Text = value.Address;
                encryptionLabel.Text = value.Encryption;
                clientExeLabel.Text = value.ClientExe;
                ultimaDirLabel.Text = value.UltimaDir;
            }
        }

        public Account AccountData
        {
            set
            {
                accountLabel.Text = value.Name;
            }
        }
    }
}
