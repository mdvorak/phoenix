using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Configuration;

namespace Phoenix
{
    public sealed class ProfileConfig
    {
        #region ColorsConfig class

        public class ColorsConfig
        {
            private SettingUInt16Entry fontColor;
            private SettingUInt16Entry consoleColor;
            private SettingUInt16Entry infoColor;
            private SettingUInt16Entry warningColor;
            private SettingUInt16Entry errorColor;

            internal event EventHandler Changed;

            internal ColorsConfig(ISettings settings)
            {
                fontColor = new SettingUInt16Entry(settings, Env.DefaultFontColor, "font", "Environment", "Colors");
                consoleColor = new SettingUInt16Entry(settings, Env.DefaultErrorColor, "console", "Environment", "Colors");
                infoColor = new SettingUInt16Entry(settings, Env.DefaultInfoColor, "info", "Environment", "Colors");
                warningColor = new SettingUInt16Entry(settings, Env.DefaultWarningColor, "warning", "Environment", "Colors");
                errorColor = new SettingUInt16Entry(settings, Env.DefaultErrorColor, "error", "Environment", "Colors");

                fontColor.Changed += new EventHandler(color_Changed);
                consoleColor.Changed += new EventHandler(color_Changed);
                infoColor.Changed += new EventHandler(color_Changed);
                warningColor.Changed += new EventHandler(color_Changed);
                errorColor.Changed += new EventHandler(color_Changed);
            }

            void color_Changed(object sender, EventArgs e)
            {
                SettingUInt16Entry color = (SettingUInt16Entry)sender;
                if (color.Value < DataFiles.Hues.MinIndex || color.Value > DataFiles.Hues.MaxIndex) {
                    color.Reset();
                }
                else {
                    SyncEvent.Invoke(Changed, sender, e);
                }
            }

            public SettingUInt16Entry FontColor
            {
                get { return fontColor; }
            }

            public SettingUInt16Entry ConsoleColor
            {
                get { return consoleColor; }
            }

            public SettingUInt16Entry InfoColor
            {
                get { return infoColor; }
            }

            public SettingUInt16Entry WarningColor
            {
                get { return warningColor; }
            }

            public SettingUInt16Entry ErrorColor
            {
                get { return errorColor; }
            }
        }

        #endregion

        #region WindowConfig class

        public class WindowConfig
        {
            private SettingBoolEntry stayOnTop;
            private SettingBoolEntry topMost;
            private SettingPointEntry position;
            private SettingSizeEntry size;
            private SettingBoolEntry showInTaskbar;
            private SettingBoolEntry showInTray;
            private SettingBoolEntry minimizeToTray;

            public WindowConfig(ISettings settings)
            {
                stayOnTop = new SettingBoolEntry(settings, false, "StayOnTop", "Config", "Window");
                topMost = new SettingBoolEntry(settings, false, "TopMost", "Config", "Window");
                position = new SettingPointEntry(settings, new System.Drawing.Point(256, 256), "Position", "Config", "Window");
                size = new SettingSizeEntry(settings, new System.Drawing.Size(319, 392), "Size", "Config", "Window");
                showInTaskbar = new SettingBoolEntry(settings, true, "ShowInTaskbar", "Config", "Window");
                showInTray = new SettingBoolEntry(settings, false, "ShowInTray", "Config", "Window");
                minimizeToTray = new SettingBoolEntry(settings, false, "MinimizeToTray", "Config", "Window");
            }

            public SettingBoolEntry StayOnTop
            {
                get { return stayOnTop; }
            }

            public SettingBoolEntry TopMost
            {
                get { return topMost; }
            }

            public SettingPointEntry Position
            {
                get { return position; }
            }

            public SettingSizeEntry Size
            {
                get { return size; }
            }

            public SettingBoolEntry ShowInTaskbar
            {
                get { return showInTaskbar; }
            }

            public SettingBoolEntry ShowInTray
            {
                get { return showInTray; }
            }

            public SettingBoolEntry MinimizeToTray
            {
                get { return minimizeToTray; }
            }
        }

        #endregion

        private SettingsLoader loader;
        private Settings settings;
        private string profileName;
        private SettingsFragment userSettings;

        private DefaultPublicEvent profileChanged;

        private SettingInt32Entry maxJournalLen;
        private SettingBoolEntry overrideSpeechColor;
        private ColorsConfig colors;
        private WindowConfig window;

        private SettingInt32Entry fpsLimit;

        internal ProfileConfig(string profilesFolder)
        {
            profileChanged = new DefaultPublicEvent();

            loader = new SettingsLoader(profilesFolder);
            profileName = null;
            settings = new SynchronizedSettings("Profile");
            userSettings = new SettingsFragment(settings, "UserSettings");

            maxJournalLen = new SettingInt32Entry(settings, 500, "MaxJournalLen", "Config");
            overrideSpeechColor = new SettingBoolEntry(settings, false, "OverrideSpeechColor", "Config");

            colors = new ColorsConfig(settings);
            window = new WindowConfig(settings);

            fpsLimit = new SettingInt32Entry(settings, 0, "fps", "Config", "FpsLimiter");
        }

        public event EventHandler ProfileChanged
        {
            add { profileChanged.AddHandler(value); }
            remove { profileChanged.RemoveHandler(value); }
        }

        public ISettings UserSettings
        {
            get { return userSettings; }
        }

        internal Settings InternalSettings
        {
            get { return settings; }
        }

        #region Profile maintaince

        public string ProfileName
        {
            get { return profileName; }
        }

        internal void Save()
        {
            settings.Save();
        }

        internal bool ProfileExists(string profile)
        {
            lock (Config.SyncRoot) {
                string path = loader.GetProfilePath(profile);
                return System.IO.File.Exists(path);
            }
        }

        internal void CreateProfile(string profile)
        {
            lock (Config.SyncRoot) {
                string path = loader.GetProfilePath(profile);

                if (settings.Path != path) {
                    settings.Save();
                    settings.Path = path;
                    profileName = profile;

                    OnProfileChanged(EventArgs.Empty);
                }
            }
        }

        internal void ChangeProfile(string profile)
        {
            lock (Config.SyncRoot) {
                string path = loader.GetProfilePath(profile);

                if (settings.Path != path) {
                    settings.Save();
                    settings.Path = path;
                    settings.Load();
                    profileName = profile;
                    OnProfileChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Called in Phoenix.Initialize() only.
        /// </summary>
        internal void LoadDefault()
        {
            lock (Config.SyncRoot) {
                profileName = "Default";
                settings.Path = loader.GetProfilePath(profileName);
                settings.Load();
            }
        }

        internal string[] EnumerateProfiles()
        {
            lock (Config.SyncRoot) {
                return loader.Enumerate();
            }
        }

        private void OnProfileChanged(EventArgs e)
        {
            profileChanged.Invoke(this, e);
        }

        #endregion

        public SettingInt32Entry MaxJournalLen
        {
            get { return maxJournalLen; }
        }

        public SettingBoolEntry OverrideSpeechColor
        {
            get { return overrideSpeechColor; }
        }

        public ColorsConfig Colors
        {
            get { return colors; }
        }

        public WindowConfig Window
        {
            get { return window; }
        }

        public SettingInt32Entry FpsLimit
        {
            get { return fpsLimit; }
        }
    }
}
