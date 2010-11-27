using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Phoenix.Configuration;
using PhoenixLauncher.Data;

namespace PhoenixLauncher.Controls
{
    public partial class ServerList : UserControl
    {
        private ISettings settings;
        public event EventHandler SelectedServerChanged;

        public ServerList()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public ISettings Settings
        {
            get { return settings; }
            set
            {
                if (settings != null) settings.Loaded -= settings_Loaded;
                settings = value;
                if (settings != null) settings.Loaded += new EventHandler(settings_Loaded);
                UpdateList();
            }
        }

        void settings_Loaded(object sender, EventArgs e)
        {
            UpdateList();
        }

        [Browsable(false)]
        public Server SelectedServer
        {
            get
            {
                if (list.SelectedItem != null)
                {
                    return new Server((string)list.SelectedItem, settings);
                }
                else
                {
                    return new Server();
                }
            }
        }

        private string LastServer
        {
            get { return settings.GetAttribute("", "LastServer", "Servers"); }
            set
            {
                if (settings != null)
                {
                    if (value == null)
                        settings.RemoveAttribute("LastServer", "Servers");
                    else
                        settings.SetAttribute(value, "LastServer", "Servers");
                }
            }
        }

        protected void OnSelectedServerChanged(EventArgs e)
        {
            if (SelectedServerChanged != null) SelectedServerChanged(this, e);
        }

        private void UpdateEmptyLabel()
        {
            emptyLabel.Location = new Point((Width - emptyLabel.Width) / 2, (Height - emptyLabel.Height) / 2);
            emptyLabel.Visible = list.Items.Count == 0;
        }

        private void UpdateList()
        {
            list.SuspendLayout();
            list.Items.Clear();

            if (settings != null)
            {
                ElementInfo[] servers = settings.EnumarateElements("Servers", "Server");

                foreach (ElementInfo info in servers)
                {
                    try
                    {
                        list.Items.Add(info.Attributes["Name"]);
                    }
                    catch (Exception) { }
                }

                list.SelectedItem = LastServer;
            }

            list.ResumeLayout();
            UpdateEmptyLabel();
            OnSelectedServerChanged(new EventArgs());
        }

        private void list_SizeChanged(object sender, EventArgs e)
        {
            UpdateEmptyLabel();
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            LastServer = (string)list.SelectedItem;
            OnSelectedServerChanged(new EventArgs());
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                editToolStripMenuItem.Enabled = true;
                removeToolStripMenuItem.Enabled = true;
            }
            else
            {
                editToolStripMenuItem.Enabled = false;
                removeToolStripMenuItem.Enabled = false;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (list.SelectedItem != null)
            {
                ServerDialog dlg = new ServerDialog(true);
                dlg.ServerData = SelectedServer;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    dlg.ServerData.Save(settings);
                }
            }

            UpdateList();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerDialog dlg = new ServerDialog(false);

        ShowDialog:
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Server server = dlg.ServerData;

                if (!settings.ElementExists("Servers", server.Element) ||
                    MessageBox.Show("Server with name \"" + server.Name + "\" already exists. Overwrite?", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    server.Save(settings);
                }
                else
                {
                    goto ShowDialog;
                }
            }

            UpdateList();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (list.SelectedItem != null)
            {
                settings.RemoveElement("Servers", new ElementInfo("Server", new AttributeInfo("Name", list.SelectedItem)));
            }

            UpdateList();
        }
    }
}
