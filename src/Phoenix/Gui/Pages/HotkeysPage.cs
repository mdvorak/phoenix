using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Gui.Pages
{
    [PhoenixWindowTabPage("Phoenix.Properties.Resources.Page_Hotkeys", TitleIsResource = true, Position = 4, Icon = "Phoenix.Properties.Resources.HotkeysIcon")]
    public partial class HotkeysPage : UserControl
    {
        private const string CommandSeparator = " | ";
        private string selectedShortcut;

        public HotkeysPage()
        {
            InitializeComponent();

            Config.Profile.InternalSettings.Loaded += new EventHandler(Settings_Loaded);
            Config.Profile.InternalSettings.Saving += new EventHandler(Settings_Saving);

            RuntimeCore.Hotkeys.Added += new HotkeyShortcutChangedEventHandler(Hotkeys_Added);
            RuntimeCore.Hotkeys.Removed += new HotkeyShortcutChangedEventHandler(Hotkeys_Removed);
            RuntimeCore.Hotkeys.Updated += new HotkeyShortcutChangedEventHandler(Hotkeys_Updated);
            RuntimeCore.Hotkeys.Cleared += new EventHandler(Hotkeys_Cleared);
        }

        void Settings_Loaded(object sender, EventArgs e)
        {
            try {
                int splitter = Config.Profile.InternalSettings.GetAttribute(-1, "splitter", "Config", "Window", "HotkeysPage");

                if (splitter < 0)
                    System.Diagnostics.Trace.WriteLine("Invalid SplitterDistance for HotkeysPage.", "Gui");
                else
                    splitContainer.SplitterDistance = splitContainer.ClientSize.Height - splitter;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Unable to load splitter distance from settings. Exception:\n" + ex.ToString(), "Gui");
            }
        }

        void Settings_Saving(object sender, EventArgs e)
        {
            Config.Profile.InternalSettings.SetAttribute(splitContainer.ClientSize.Height - splitContainer.SplitterDistance, "splitter", "Config", "Window", "HotkeysPage");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RuntimeCore.Hotkeys.Reload();
        }

        void Hotkeys_Added(object sender, HotkeyShortcutChangedEventArgs e)
        {
            ListViewItem item = new ListViewItem(e.Shortcut.ToString());
            item.Tag = e.Shortcut;
            item.SubItems.Add(RuntimeCore.Hotkeys.Get(e.Shortcut).Replace("\n", CommandSeparator));
            listView.Items.Add(item);

            if (IsHandleCreated) {
                foreach (ListViewItem selectedItem in listView.SelectedItems) {
                    selectedItem.Selected = false;
                }

                item.Selected = true;
                item.EnsureVisible();
            }
            else {
                System.Diagnostics.Debug.WriteLine("HotkeysPage.IsHandleCreated == false", "Warning");
            }

            RefreshSelected();
        }

        void Hotkeys_Removed(object sender, HotkeyShortcutChangedEventArgs e)
        {
            foreach (ListViewItem item in listView.Items) {
                if (item.Tag.Equals(e.Shortcut)) {
                    listView.Items.Remove(item);
                    break;
                }
            }

            RefreshSelected();
        }

        void Hotkeys_Updated(object sender, HotkeyShortcutChangedEventArgs e)
        {
            foreach (ListViewItem item in listView.Items) {
                if (item.Tag.Equals(e.Shortcut)) {
                    item.SubItems[1].Text = RuntimeCore.Hotkeys.Get(e.Shortcut).Replace("\n", CommandSeparator);
                    break;
                }
            }

            RefreshSelected();
        }

        void Hotkeys_Cleared(object sender, EventArgs e)
        {
            listView.Items.Clear();
            RefreshSelected();
        }

        private void RefreshSelected()
        {
            try {
                if (listView.SelectedItems.Count > 0) {
                    updateButton.Enabled = true;
                    deleteButton.Enabled = true;
                    commandLineBox.Enabled = true;

                    selectedShortcut = (string)listView.SelectedItems[0].Tag;
                    commandLineBox.Text = RuntimeCore.Hotkeys.Get(selectedShortcut);
                }
                else {
                    updateButton.Enabled = false;
                    deleteButton.Enabled = false;
                    commandLineBox.Enabled = false;

                    selectedShortcut = "None";
                    commandLineBox.Text = "";
                }

                commandLineBox.Modified = false;
            }
            catch (Exception e) {
                System.Diagnostics.Trace.WriteLine("Error in HotkeysPage.RefreshSelected(). Exception:\n" + e.ToString(), "Gui");
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelected();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try {
                AddHotkeyDialog dlg = new AddHotkeyDialog();

                // We dont wanna hide the dialog..
                ((PhoenixWindow)ParentForm).SupressTopMost = true;
                ParentForm.TopMost = false;

                if (dlg.ShowDialog() == DialogResult.OK) {
                    RuntimeCore.Hotkeys.Add(dlg.Shortcut, "");
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error");
            }
            finally {
                ((PhoenixWindow)ParentForm).SupressTopMost = false;
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            try {
                RuntimeCore.Hotkeys.Update(selectedShortcut, commandLineBox.Text);
                commandLineBox.Modified = false;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try {
                RuntimeCore.Hotkeys.Remove(selectedShortcut);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void listView_SizeChanged(object sender, EventArgs e)
        {
            commandColumn.Width = listView.ClientSize.Width - shortcutColumn.Width - SystemInformation.Border3DSize.Width * 3;
        }

        private void commandLineBox_ModifiedChanged(object sender, EventArgs e)
        {
            updateButton.ForeColor = commandLineBox.Modified ? Color.Red : SystemColors.ControlText;
        }
    }
}
