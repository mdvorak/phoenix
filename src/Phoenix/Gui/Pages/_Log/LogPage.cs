using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Crownwood.Magic.Menus;

namespace Phoenix.Gui.Pages._Log
{
    public partial class LogPage : UserControl
    {
        private LogBoxWriterTraceListener listener;
        private LogFilter dlg;

        private PopupMenu popupMenu;
        private MenuCommand copyAllMenuCommand;
        private MenuCommand clearMenuCommand;
        private MenuCommand forecolorMenuCommand;
        private MenuCommand backroundMenuCommand;
        private MenuCommand wordWrapMenuCommand;
        private MenuCommand filterMenuCommand;
        private MenuCommand autoScrollCommand;

        private void InitializeMenu()
        {
            popupMenu = new PopupMenu();

            copyAllMenuCommand = new MenuCommand("Copy All", new EventHandler(copyAllMenuCommand_Click));
            clearMenuCommand = new MenuCommand("Clear", new EventHandler(clearMenuCommand_Click));
            forecolorMenuCommand = new MenuCommand("ForeColor", new EventHandler(forecolorMenuCommand_Click));
            backroundMenuCommand = new MenuCommand("BackColor", new EventHandler(backroundMenuCommand_Click));
            wordWrapMenuCommand = new MenuCommand("WordWrap", new EventHandler(wordWrapMenuCommand_Click));
            filterMenuCommand = new MenuCommand("Filter", new EventHandler(filterMenuCommand_Click));
            autoScrollCommand = new MenuCommand("AutoScroll", new EventHandler(autoScrollCommand_Click));

            popupMenu.MenuCommands.Add(copyAllMenuCommand);
            popupMenu.MenuCommands.Add(clearMenuCommand);
            popupMenu.MenuCommands.Add(new MenuCommand("-"));
            popupMenu.MenuCommands.Add(forecolorMenuCommand);
            popupMenu.MenuCommands.Add(backroundMenuCommand);
            popupMenu.MenuCommands.Add(wordWrapMenuCommand);
            popupMenu.MenuCommands.Add(new MenuCommand("-"));
            popupMenu.MenuCommands.Add(filterMenuCommand);
            popupMenu.MenuCommands.Add(autoScrollCommand);
        }

        public LogPage()
        {
            InitializeComponent();
            InitializeMenu();

            listener = new LogBoxWriterTraceListener(logBox);
            Trace.Listeners.Add(listener);

            dlg = new LogFilter();
            logBox.Categories = dlg.GetCategories();

            LoadSettings(); 

            Config.Profile.InternalSettings.Loaded += new EventHandler(Settings_Loaded);
            Config.Profile.InternalSettings.Saving += new EventHandler(Settings_Saving);
        }

        private void LoadSettings()
        {
            int rgb = Config.Profile.InternalSettings.GetAttribute(SystemColors.Window.ToArgb(), "backcolor", "Config", "Window", "LogPage");
            logBox.BackColor = System.Drawing.Color.FromArgb(rgb);

            rgb = Config.Profile.InternalSettings.GetAttribute(SystemColors.WindowText.ToArgb(), "forecolor", "Config", "Window", "LogPage");
            System.Drawing.Color foreColor = System.Drawing.Color.FromArgb(rgb);
            if (foreColor != logBox.ForeColor)
            {
                logBox.ForeColor = foreColor;
            }

            logBox.WordWrap = Config.Profile.InternalSettings.GetAttribute(false, "wordwrap", "Config", "Window", "LogPage");
            logBox.AutoScroll = Config.Profile.InternalSettings.GetAttribute(true, "autoscroll", "Config", "Window", "LogPage");

            wordWrapMenuCommand.Checked = logBox.WordWrap;
            autoScrollCommand.Checked = logBox.AutoScroll;
        }

        void Settings_Loaded(object sender, EventArgs e)
        {
            LoadSettings();
        }

        void Settings_Saving(object sender, EventArgs e)
        {
            Config.Profile.InternalSettings.SetAttribute(logBox.ForeColor.ToArgb(), "forecolor", "Config", "Window", "LogPage");
            Config.Profile.InternalSettings.SetAttribute(logBox.BackColor.ToArgb(), "backcolor", "Config", "Window", "LogPage");
            Config.Profile.InternalSettings.SetAttribute(logBox.WordWrap, "wordwrap", "Config", "Window", "LogPage");
            Config.Profile.InternalSettings.SetAttribute(logBox.AutoScroll, "autoscroll", "Config", "Window", "LogPage");
        }

        private void dummyContextMenu_Opening(object sender, CancelEventArgs e)
        {
            popupMenu.TrackPopup(Control.MousePosition);
        }

        void copyAllMenuCommand_Click(object sender, EventArgs e)
        {
            if (logBox.TextLength > 0)
            {
                DataObject o = new DataObject();
                o.SetText(logBox.Text.Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine), TextDataFormat.Text);
                o.SetText(logBox.Rtf, TextDataFormat.Rtf);
                Helper.CopyObjectToClipboardSafe(o);
            }
        }

        void clearMenuCommand_Click(object sender, EventArgs e)
        {
            logBox.Clear();
        }

        void forecolorMenuCommand_Click(object sender, EventArgs e)
        {
            colorDialog.Color = logBox.ForeColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                logBox.ForeColor = colorDialog.Color;
            }
        }

        void backroundMenuCommand_Click(object sender, EventArgs e)
        {
            colorDialog.Color = logBox.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                logBox.BackColor = colorDialog.Color;
            }
        }

        void wordWrapMenuCommand_Click(object sender, EventArgs e)
        {
            logBox.WordWrap = !logBox.WordWrap;
            wordWrapMenuCommand.Checked = logBox.WordWrap;
        }

        void filterMenuCommand_Click(object sender, EventArgs e)
        {
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                logBox.Categories = dlg.GetCategories();
            }
        }

        void autoScrollCommand_Click(object sender, EventArgs e)
        {
            logBox.AutoScroll = !logBox.AutoScroll;
            autoScrollCommand.Checked = logBox.AutoScroll;
        }
    }
}
