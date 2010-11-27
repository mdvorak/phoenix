using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Pages.SettingCategories
{
    public partial class GeneralCategory : UserControl
    {
        public GeneralCategory()
        {
            InitializeComponent();

            Config.Profile.ProfileChanged += new EventHandler(Profile_ProfileChanged);

            stayOnTopCheckBox.SettingEntry = Config.Profile.Window.StayOnTop;
            topMostCheckBox.SettingEntry = Config.Profile.Window.TopMost;

            showInTaskbar.SettingEntry = Config.Profile.Window.ShowInTaskbar;
            showInTray.SettingEntry = Config.Profile.Window.ShowInTray;
            minimizeToTray.SettingEntry = Config.Profile.Window.MinimizeToTray;

            Config.Profile.Colors.FontColor.Changed += new EventHandler(FontColor_Changed);
            Config.Profile.Colors.ConsoleColor.Changed += new EventHandler(ConsoleColor_Changed);

            overrideSpeechCheckBox.SettingEntry = Config.Profile.OverrideSpeechColor;

            speechColorBox.Value = Config.Profile.Colors.FontColor;
            consoleColorBox.Value = Config.Profile.Colors.ConsoleColor;

            Config.Profile.FpsLimit.Changed += new EventHandler(FrameLimit_Changed);
        }

        void Profile_ProfileChanged(object sender, EventArgs e)
        {
            profileLabel.Text = Config.Profile.ProfileName;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Config.Save();
        }

        private void changeProfileButton_Click(object sender, EventArgs e)
        {
            Core.SelectProfile(Config.Profile.ProfileName, true);
        }

        private void speechColorBox_ValueChanged(object sender, EventArgs e)
        {
            Config.Profile.Colors.FontColor.Value = speechColorBox.Value;
        }

        private void consoleColorBox_ValueChanged(object sender, EventArgs e)
        {
            Config.Profile.Colors.ConsoleColor.Value = consoleColorBox.Value;
        }

        void FontColor_Changed(object sender, EventArgs e)
        {
            speechColorBox.Value = Config.Profile.Colors.FontColor;
        }

        void ConsoleColor_Changed(object sender, EventArgs e)
        {
            consoleColorBox.Value = Config.Profile.Colors.ConsoleColor;
        }

        private void fps_CheckedChanged(object sender, EventArgs e)
        {
            int fps = Config.Profile.FpsLimit;

            if (disabledLimiterBox.Checked)
                fps = 0;
            else if (fps120Box.Checked)
                fps = 120;
            else if (fps500Box.Checked)
                fps = 500;

            Config.Profile.FpsLimit.Value = fps;
        }

        void FrameLimit_Changed(object sender, EventArgs e)
        {
            disabledLimiterBox.Checked = false;
            fps120Box.Checked = false;
            fps500Box.Checked = false;

            switch (Config.Profile.FpsLimit.Value)
            {
                case -1:
                case 0:
                    disabledLimiterBox.Checked = true;
                    break;

                case 120:
                    fps120Box.Checked = true;
                    break;

                case 500:
                    fps500Box.Checked = true;
                    break;
            }
        }
    }
}
