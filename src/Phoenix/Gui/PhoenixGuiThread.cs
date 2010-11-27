using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Phoenix.Gui.Controls;
using System.Diagnostics;

namespace Phoenix.Gui
{
    internal class PhoenixGuiThread : GuiThread
    {
        private PhoenixWindow window;

        public PhoenixGuiThread()
            : base(typeof(PhoenixWindow))
        {
            window = (PhoenixWindow)Window;
            System.Diagnostics.Debug.WriteLine("GUI Thread ID: " + Thread.ManagedThreadId.ToString(), "Information");
        }

        protected override void OnThreadStarted()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = Core.Culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = Core.Culture;
        }

        internal void ShowPhoenixWindow()
        {
            Invoke(new ThreadStart(window.Show));
        }

        internal void ClosePhoenixWindow()
        {
            Invoke(new ThreadStart(window.Close));
        }

        private delegate void SelectProfileProcDelegate(string selectedProfile, bool cancelEnabled);

        public void SelectProfile(string selectedProfile, bool cancelEnabled)
        {
            InvokeFast(new SelectProfileProcDelegate(SelectProfileProc), selectedProfile, cancelEnabled);
        }

        private void SelectProfileProc(string selectedProfile, bool cancelEnabled)
        {
            try {
                lock (Config.SyncRoot) {
                    using (SelectProfileDialog dlg = new SelectProfileDialog()) {
                        dlg.AddExistingProfiles(Config.Profile.EnumerateProfiles());
                        dlg.SelectedProfile = selectedProfile;
                        dlg.CancelEnabled = cancelEnabled;

                    ShowDialog:
                        if (dlg.ShowDialog() == DialogResult.OK) {

                            if (dlg.CreateProfile && Config.Profile.ProfileExists(dlg.SelectedProfile)) {
                                if (MessageBox.Show("Profile of same name already exists. Overwrite?", "Profile exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                                    Config.Profile.CreateProfile(dlg.SelectedProfile);
                                }
                                else {
                                    goto ShowDialog;
                                }
                            }
                            else {
                                Config.Profile.ChangeProfile(dlg.SelectedProfile);
                            }

                            Config.PlayerProfile = Config.Profile.ProfileName;
                            Config.Save();
                        }
                    }
                }
            }
            catch (Exception e) {
                Trace.WriteLine("Error changing profile. Exception:\n" + e.ToString(), "Phoenix");
                MessageBox.Show("Error changing profile.\nMessage: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private delegate void SetWindowTopMostDelegate(bool value);

        public void SetWindowTopMost(bool value)
        {
            InvokeFast(new SetWindowTopMostDelegate(window.SetTopMost), value);
        }

        private delegate void AddInfoGroupDelegate(Control control, string title);
        private delegate void RemoveInfoGroupDelegate(Control control);

        public void AddInfoGroup(Control control, string title)
        {
            InvokeFast(new AddInfoGroupDelegate(window.InfoPage.AddGroup), control, title);
        }

        public void RemoveInfoGroup(Control control)
        {
            InvokeFast(new RemoveInfoGroupDelegate(window.InfoPage.RemoveGroup), control);
        }

        private delegate void InsertTabDelegate(int index, Crownwood.Magic.Controls.TabPage page);
        private delegate void RemoveTabDelegate(Crownwood.Magic.Controls.TabPage page);

        public void InsertTab(int index, Crownwood.Magic.Controls.TabPage page)
        {
            InvokeFast(new InsertTabDelegate(window.TabControl.InsertTab), index, page);
        }

        public void RemoveTab(Crownwood.Magic.Controls.TabPage page)
        {
            InvokeFast(new RemoveTabDelegate(window.TabControl.RemoveTab), page);
        }

        private delegate void SettingsCategoryDelegate(CategoryData data);

        public void AddSettingsPage(CategoryData data)
        {
            InvokeFast(new SettingsCategoryDelegate(window.SettingsPage.AddCategory), data);
        }

        public void RemoveSettingsPage(CategoryData data)
        {
            InvokeFast(new SettingsCategoryDelegate(window.SettingsPage.RemoveCategory), data);
        }
    }
}
