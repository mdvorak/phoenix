using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Pages.InfoGroups
{
    [InfoGroup("Server")]
    public partial class ServerInfo : UserControl
    {
        public ServerInfo()
        {
            InitializeComponent();

            LoginInfo.Changed += new EventHandler(LoginInfo_Changed);
            Core.Disconnected += new EventHandler(Core_Disconnected);

            encryptionBox.Text = Core.LaunchData.ServerEncName;
            Reset();
        }

        void Core_Disconnected(object sender, EventArgs e)
        {
            Reset();
        }

        void LoginInfo_Changed(object sender, EventArgs e)
        {
            addressBox.Text = LoginInfo.Address.ToString();
            shardBox.Text = LoginInfo.Shard;
        }

        private void Reset()
        {
            addressBox.Text = Core.LaunchData.Address;
            shardBox.Text = "";
        }
    }
}
