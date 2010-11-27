using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Phoenix.Configuration;
using PhoenixLauncher.Controls;
using PhoenixLauncher.Data;

namespace PhoenixLauncher
{
    public partial class MainWindow : Form
    {
        public const string PhoenixLauncherCfg = "PhoenixLauncher.xml";
        public const string HistoryFileName = "history.dat";

        private Settings settings;

        public MainWindow()
        {
            InitializeComponent();

            settings = null;
        }

        internal Settings Settings
        {
            get { return settings; }
            set
            {
                settings = value;

                try
                {
                    serverList.Settings = settings;
                    accountList.Settings = settings;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unhandled exception in MainWindow.set_Settings(). Details:\n" + e.ToString(), "Fatal Error");
                    Environment.Exit(-1);
                }
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            HistoryCache.Load(HistoryFileName);

            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.Replace('-', '/').StartsWith("/launchlast", StringComparison.InvariantCultureIgnoreCase))
                {
                    Launcher dlg = new Launcher();
                    dlg.StartPosition = FormStartPosition.CenterScreen;
                    dlg.Server = serverList.SelectedServer;
                    dlg.Account = accountList.SelectedAccount;

                    dlg.ShowDialog();

                    if (dlg.Success)
                    {
                        Close();
                    }
                }
            }
        }

        private void serverList_SelectedServerChanged(object sender, EventArgs e)
        {
            accountList.Server = serverList.SelectedServer;
            details.ServerData = accountList.Server;

            launchButton.Enabled = !serverList.SelectedServer.IsEmpty;
        }

        private void accountList_SelectedAccountChanged(object sender, EventArgs e)
        {
            details.AccountData = accountList.SelectedAccount;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.Save();
            HistoryCache.Save(HistoryFileName);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void launchButton_Click(object sender, EventArgs e)
        {
            Launcher dlg = new Launcher();
            dlg.Server = serverList.SelectedServer;
            dlg.Account = accountList.SelectedAccount;

            dlg.ShowDialog();

            if (dlg.Success)
            {
                Close();
            }
        }

        private void quickLaunchButton_Click(object sender, EventArgs e)
        {
            QuickLaunchDialog dlg = new QuickLaunchDialog();
            dlg.Server = serverList.SelectedServer;
            dlg.Account = accountList.SelectedAccount;

            dlg.ShowDialog();

            if (dlg.Started)
            {
                Close();
            }
        }
    }
}