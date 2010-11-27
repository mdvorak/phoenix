using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Pages.SettingCategories
{
    [SettingsCategory("Log")]
    public partial class Log : UserControl
    {
        public Log()
        {
            InitializeComponent();

            packetLoggingBox.SettingEntry = Config.PacketLog.Enable;
            clientPhoenixBox.SettingEntry = Config.PacketLog.ClientToPhoenix;
            phoenixServerBox.SettingEntry = Config.PacketLog.PhoenixToServer;
            serverPhoenixBox.SettingEntry = Config.PacketLog.ServerToPhoenix;
            phoenixClientBox.SettingEntry = Config.PacketLog.PhoenixToClient;

            packetLoggingBox.CheckedChanged += new EventHandler(packetLoggingBox_CheckedChanged);
        }

        void packetLoggingBox_CheckedChanged(object sender, EventArgs e)
        {
            clientPhoenixBox.Enabled = packetLoggingBox.Checked;
            phoenixServerBox.Enabled = packetLoggingBox.Checked;
            serverPhoenixBox.Enabled = packetLoggingBox.Checked;
            phoenixClientBox.Enabled = packetLoggingBox.Checked;
        }
    }
}
