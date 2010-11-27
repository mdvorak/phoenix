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
    public partial class AccountList : UserControl
    {
        private ISettings settings;
        private Server server;
        public event EventHandler SelectedAccountChanged;

        public AccountList()
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
        public Server Server
        {
            get { return server; }
            set
            {
                server = value;
                UpdateList();
            }
        }

        [Browsable(false)]
        public Account SelectedAccount
        {
            get
            {
                if (list.SelectedItem != null)
                {
                    return new Account((string)list.SelectedItem, settings, server);
                }
                else
                {
                    return new Account();
                }
            }
        }

        private string LastAccount
        {
            get { return settings.GetAttribute("", "LastAccount", "Servers", server.Element); }
            set
            {
                if (value == null)
                    settings.RemoveAttribute("LastAccount", "Servers", server.Element);
                else
                    settings.SetAttribute(value, "LastAccount", "Servers", server.Element);
            }
        }

        protected void OnSelectedAccountChanged(EventArgs e)
        {
            if (SelectedAccountChanged != null) SelectedAccountChanged(this, e);
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

            if (settings != null && !server.IsEmpty)
            {
                ElementInfo[] accounts = settings.EnumarateElements("Servers", server.Element, "Account");

                foreach (ElementInfo info in accounts)
                {
                    try
                    {
                        list.Items.Add(info.Attributes["Name"]);
                    }
                    catch (Exception) { }
                }

                list.SelectedItem = LastAccount;
            }

            list.ResumeLayout();
            UpdateEmptyLabel();
            OnSelectedAccountChanged(new EventArgs());
        }

        private void list_SizeChanged(object sender, EventArgs e)
        {
            UpdateEmptyLabel();
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            LastAccount = (string)list.SelectedItem;
            OnSelectedAccountChanged(new EventArgs());
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

            addToolStripMenuItem.Enabled = !server.IsEmpty;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (list.SelectedItem != null)
            {
                AccountDialog dlg = new AccountDialog(true);
                dlg.AccountData = SelectedAccount;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    dlg.AccountData.Save(settings, server);
                }
            }

            UpdateList();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountDialog dlg = new AccountDialog(false);

        ShowDialog:
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Account account = dlg.AccountData;

                if (!settings.ElementExists("Servers", server.Element, account.Element) ||
                    MessageBox.Show("Account \"" + account.Name + "\" already exists. Overwrite?", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    account.Save(settings, server);
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
                settings.RemoveElement("Servers", server.Element, new ElementInfo("Account", new AttributeInfo("Name", SelectedAccount)));
            }

            UpdateList();
        }
    }
}
