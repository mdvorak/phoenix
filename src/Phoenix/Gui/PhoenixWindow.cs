using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Crownwood.Magic;
using Crownwood.Magic.Docking;
using Crownwood.Magic.Menus;
using Phoenix.Runtime;
using Phoenix.Properties;

namespace Phoenix.Gui
{
    partial class PhoenixWindow : Phoenix.Gui.Controls.FormEx
    {
        private Phoenix.Gui.Pages.InfoPage infoPage;
        private Phoenix.Gui.Pages.SettingsPage settingsPage;
        private Phoenix.Gui.Pages._Log.LogPage logPage;
        private bool supressTopmost;

        public PhoenixWindow()
        {
            InitializeComponent();

            trayIcon.Icon = this.Icon;

            infoPage = new Phoenix.Gui.Pages.InfoPage();
            settingsPage = new Phoenix.Gui.Pages.SettingsPage();
            logPage = new Phoenix.Gui.Pages._Log.LogPage();

            phoenixTabControl.InsertTab(0, new Crownwood.Magic.Controls.TabPage(Resources.Page_Info, infoPage, Resources.InfoIcon));
            phoenixTabControl.InsertTab(1, new Crownwood.Magic.Controls.TabPage(Resources.Page_Settings, settingsPage, Properties.Resources.SettingsIcon));
            phoenixTabControl.InsertTab(3, new Crownwood.Magic.Controls.TabPage(Resources.Page_Log, logPage, Properties.Resources.LogIcon));

            Config.Profile.Window.TopMost.Changed += new EventHandler(TopMost_Changed);

            Config.Profile.Window.Position.Changed += new EventHandler(WindowPosition_Changed);
            Config.Profile.Window.Position.Saving += new EventHandler(WindowPosition_Saving);

            Config.Profile.Window.Size.Changed += new EventHandler(WindowSize_Changed);
            Config.Profile.Window.Size.Saving += new EventHandler(WindowSize_Saving);

            Config.Profile.Window.ShowInTaskbar.Changed += new EventHandler(ShowInTaskbar_Changed);
            Config.Profile.Window.ShowInTray.Changed += new EventHandler(ShowInTray_Changed);
            Config.Profile.Window.MinimizeToTray.Changed += new EventHandler(MinimizeToTray_Changed);

            Location = Config.Profile.Window.Position;
            Size = Config.Profile.Window.Size;

            Text = Core.VersionString;
        }

        public bool SupressTopMost
        {
            get { return supressTopmost; }
            set { supressTopmost = value; }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        void TopMost_Changed(object sender, EventArgs e)
        {
            TopMost = Config.Profile.Window.TopMost;
        }

        internal PhoenixTabControl TabControl
        {
            get { return phoenixTabControl; }
        }

        internal Phoenix.Gui.Pages.InfoPage InfoPage
        {
            get { return infoPage; }
        }

        internal Phoenix.Gui.Pages.SettingsPage SettingsPage
        {
            get { return settingsPage; }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            trayIcon.Text = Text;
        }

        void MinimizeToTray_Changed(object sender, EventArgs e)
        {
            ShowInTaskbar = Config.Profile.Window.ShowInTaskbar &&
                !(WindowState == FormWindowState.Minimized && Config.Profile.Window.MinimizeToTray);

            trayIcon.Visible = Config.Profile.Window.ShowInTray ||
                               (WindowState == FormWindowState.Minimized && Config.Profile.Window.MinimizeToTray);
        }

        void ShowInTray_Changed(object sender, EventArgs e)
        {
            trayIcon.Visible = Config.Profile.Window.ShowInTray ||
                               (WindowState == FormWindowState.Minimized && Config.Profile.Window.MinimizeToTray);
        }

        void ShowInTaskbar_Changed(object sender, EventArgs e)
        {
            ShowInTaskbar = Config.Profile.Window.ShowInTaskbar &&
                            !(WindowState == FormWindowState.Minimized && Config.Profile.Window.MinimizeToTray);
        }

        protected override void OnWindowStateChanged(EventArgs e)
        {
            // If ShowInTaskbar is false, window is minimized to left bottom corner.
            ShowInTaskbar = Config.Profile.Window.ShowInTaskbar || Config.Profile.Window.MinimizeToTray || Config.Profile.Window.ShowInTray;

            base.OnWindowStateChanged(e);

            ShowInTaskbar = Config.Profile.Window.ShowInTaskbar &&
                            !(WindowState == FormWindowState.Minimized && Config.Profile.Window.MinimizeToTray);

            trayIcon.Visible = Config.Profile.Window.ShowInTray ||
                               (WindowState == FormWindowState.Minimized && Config.Profile.Window.MinimizeToTray);
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) {
                WindowState = FormWindowState.Normal;
            }
        }

        private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void terminateAllStripMenuItem_Click(object sender, EventArgs e)
        {
            Phoenix.Runtime.RuntimeCore.Executions.TerminateAll();
        }

        private void PhoenixWindow_Activated(object sender, EventArgs e)
        {
            if (supressTopmost)
                return;

            SetTopMost(Config.Profile.Window.StayOnTop || Config.Profile.Window.TopMost);
        }

        private void PhoenixWindow_Deactivate(object sender, EventArgs e)
        {
            if (supressTopmost)
                return;

            SetTopMost(Config.Profile.Window.TopMost);
        }

        void WindowPosition_Saving(object sender, EventArgs e)
        {
            if (!IsDisposed && SafeIsHandleCreated) {
                Debug.Assert(SafeLocation.X > -32000 && SafeLocation.Y > -32000);
                Config.Profile.Window.Position.Value = SafeLocation;
            }
        }

        void WindowPosition_Changed(object sender, EventArgs e)
        {
            if (!IsDisposed) {
                Location = Config.Profile.Window.Position;
            }
        }

        void WindowSize_Saving(object sender, EventArgs e)
        {
            if (!IsDisposed && SafeIsHandleCreated) {
                Debug.Assert(SafeSize != SystemInformation.MinimizedWindowSize);
                Config.Profile.Window.Size.Value = SafeSize;
            }
        }

        void WindowSize_Changed(object sender, EventArgs e)
        {
            if (!IsDisposed) {
                Size = Config.Profile.Window.Size;
            }
        }
    }
}