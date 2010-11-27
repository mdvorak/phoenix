using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using Crownwood.Magic.Menus;
using MulLib;
using Phoenix.Gui.Controls;

namespace Phoenix.Gui.Pages._Journal
{
    [PhoenixWindowTabPage("Phoenix.Properties.Resources.Page_Journal", TitleIsResource = true, Position = 2, Icon = "Phoenix.Properties.Resources.JournalIcon")]
    public partial class JournalPage : UserControl
    {
        private bool autoScroll;

        private PopupMenu popupMenu;
        private MenuCommand copyAllMenuCommand;
        private MenuCommand clearMenuCommand;
        private MenuCommand backroundMenuCommand;
        private MenuCommand wordWrapMenuCommand;
        private MenuCommand maxLenghtMenuCommand;
        private MenuCommand autoScrollCommand;

        private void InitializeMenu()
        {
            popupMenu = new PopupMenu();

            copyAllMenuCommand = new MenuCommand("Copy All", new EventHandler(copyAllMenuCommand_Click));
            clearMenuCommand = new MenuCommand("Clear", new EventHandler(clearMenuCommand_Click));
            backroundMenuCommand = new MenuCommand("BackColor", new EventHandler(backroundMenuCommand_Click));
            wordWrapMenuCommand = new MenuCommand("WordWrap", new EventHandler(wordWrapMenuCommand_Click));
            maxLenghtMenuCommand = new MenuCommand("Max Lenght", new EventHandler(maxLenghtMenuCommand_Click));
            autoScrollCommand = new MenuCommand("AutoScroll", new EventHandler(autoScrollCommand_Click));

            popupMenu.MenuCommands.Add(copyAllMenuCommand);
            popupMenu.MenuCommands.Add(clearMenuCommand);
            popupMenu.MenuCommands.Add(new MenuCommand("-"));
            popupMenu.MenuCommands.Add(backroundMenuCommand);
            popupMenu.MenuCommands.Add(wordWrapMenuCommand);
            popupMenu.MenuCommands.Add(new MenuCommand("-"));
            popupMenu.MenuCommands.Add(maxLenghtMenuCommand);
            popupMenu.MenuCommands.Add(autoScrollCommand);
        }

        public JournalPage()
        {
            InitializeComponent();
            InitializeMenu();

            LoadSettings();

            Phoenix.Logging.JournalHandler.JournalEntryAdded += new JournalEntryAddedEventHandler(JournalHandler_JournalEntryAdded);

            Config.Profile.InternalSettings.Loaded += new EventHandler(Settings_Loaded);
            Config.Profile.InternalSettings.Saving += new EventHandler(Settings_Saving);
        }

        private void LoadSettings()
        {
            int rgb = Config.Profile.InternalSettings.GetAttribute(SystemColors.Window.ToArgb(), "backcolor", "Config", "Window", "JournalPage");
            richTextBox.BackColor = System.Drawing.Color.FromArgb(rgb);

            richTextBox.WordWrap = Config.Profile.InternalSettings.GetAttribute(false, "wordwrap", "Config", "Window", "JournalPage");
            autoScroll = Config.Profile.InternalSettings.GetAttribute(true, "autoscroll", "Config", "Window", "JournalPage");

            wordWrapMenuCommand.Checked = richTextBox.WordWrap;
            autoScrollCommand.Checked = autoScroll;
        }

        void Settings_Loaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        void Settings_Saving(object sender, EventArgs e)
        {
            Config.Profile.InternalSettings.SetAttribute(richTextBox.BackColor.ToArgb(), "backcolor", "Config", "Window", "JournalPage");
            Config.Profile.InternalSettings.SetAttribute(richTextBox.WordWrap, "wordwrap", "Config", "Window", "JournalPage");
            Config.Profile.InternalSettings.SetAttribute(autoScroll, "autoscroll", "Config", "Window", "JournalPage");
        }

        private void AddEntry(JournalEntry e)
        {
            try
            {
                RichTextBoxEx.State state = richTextBox.GetState();

                try
                {
                    richTextBox.LockWindowUpdate();

                    HueEntry entry;
                    if (e.Color >= DataFiles.Hues.MinIndex && e.Color <= DataFiles.Hues.MaxIndex)
                        entry = DataFiles.Hues[e.Color];
                    else
                        entry = DataFiles.Hues[Env.DefaultFontColor];

                    richTextBox.Select(richTextBox.TextLength, 0);
                    richTextBox.SelectionColor = MulLib.UOColorConverter.ToColor(entry.Colors[24]);
                    richTextBox.AppendText(String.Format("{0} {1}\n", e.TimeStamp.ToShortTimeString(), e));

                    int lines = richTextBox.Lines.Length;
                    if (lines > Config.Profile.MaxJournalLen)
                    {
                        int line = lines - Config.Profile.MaxJournalLen + 4 + Config.Profile.MaxJournalLen / 20;
                        int deleteCharsCount = richTextBox.GetFirstCharIndexFromLine(line);

                        richTextBox.Select(0, deleteCharsCount);
                        richTextBox.SelectedText = "\n";
                    }
                }
                finally
                {
                    richTextBox.UnlockWindowUpdate();
                }

                if (IsHandleCreated)
                {
                    if (autoScroll)
                        richTextBox.ScrollToBottom();
                    else
                        richTextBox.SetState(state);
                }
                else
                    System.Diagnostics.Debug.WriteLine("JournalPage.IsHandleCreated == false", "Warning");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Unhandled error during update of JournalControl. Exception:\n" + ex.ToString(), "Gui");
            }
        }


        void JournalHandler_JournalEntryAdded(object sender, JournalEntryAddedEventArgs e)
        {
            AddEntry(e.Entry);
        }


        private void dummyMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            popupMenu.TrackPopup(Control.MousePosition);
        }

        void copyAllMenuCommand_Click(object sender, EventArgs e)
        {
            if (richTextBox.TextLength > 0)
            {
                DataObject o = new DataObject();
                o.SetText(richTextBox.Text.Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine), TextDataFormat.Text);
                o.SetText(richTextBox.Rtf, TextDataFormat.Rtf);
                Helper.CopyObjectToClipboardSafe(o);
            }
        }

        void clearMenuCommand_Click(object sender, EventArgs e)
        {
            richTextBox.Clear();
        }

        void backroundMenuCommand_Click(object sender, EventArgs e)
        {
            colorDialog.Color = richTextBox.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox.BackColor = colorDialog.Color;
            }
        }

        void wordWrapMenuCommand_Click(object sender, EventArgs e)
        {
            richTextBox.WordWrap = !richTextBox.WordWrap;
            wordWrapMenuCommand.Checked = richTextBox.WordWrap;
        }

        void maxLenghtMenuCommand_Click(object sender, EventArgs e)
        {
            MaxJournalLenghtDialog dlg = new MaxJournalLenghtDialog();
            dlg.ShowDialog();
        }

        void autoScrollCommand_Click(object sender, EventArgs e)
        {
            autoScroll = !autoScroll;
            autoScrollCommand.Checked = autoScroll;
        }

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Helper.OpenBroswer(e.LinkText);
        }
    }
}
