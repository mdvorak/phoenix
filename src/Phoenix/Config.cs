using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.Configuration;

namespace Phoenix
{
    public static class Config
    {
        #region PacketLogging class

        public class PacketLogging
        {
            private SettingBoolEntry enable;
            private SettingBoolEntry clientToPhoenix;
            private SettingBoolEntry phoenixToServer;
            private SettingBoolEntry serverToPhoenix;
            private SettingBoolEntry phoenixToClient;

            private DefaultPublicEvent changed;

            internal PacketLogging(ISettings settings)
            {
                changed = new DefaultPublicEvent();

                enable = new SettingBoolEntry(settings, false, "Enable", "Config", "PacketLogging");
                clientToPhoenix = new SettingBoolEntry(settings, true, "ClientToPhoenix", "Config", "PacketLogging");
                phoenixToServer = new SettingBoolEntry(settings, false, "PhoenixToServer", "Config", "PacketLogging");
                serverToPhoenix = new SettingBoolEntry(settings, true, "ServerToPhoenix", "Config", "PacketLogging");
                phoenixToClient = new SettingBoolEntry(settings, false, "PhoenixToClient", "Config", "PacketLogging");

                enable.Changed += new EventHandler(packetLogging_Changed);
                clientToPhoenix.Changed += new EventHandler(packetLogging_Changed);
                phoenixToServer.Changed += new EventHandler(packetLogging_Changed);
                serverToPhoenix.Changed += new EventHandler(packetLogging_Changed);
                phoenixToClient.Changed += new EventHandler(packetLogging_Changed);
            }

            void packetLogging_Changed(object sender, EventArgs e)
            {
                changed.Invoke(sender, e);
            }

            public event EventHandler Changed
            {
                add { changed.AddHandler(value); }
                remove { changed.RemoveHandler(value); }
            }

            public SettingBoolEntry Enable
            {
                get { return enable; }
            }

            public SettingBoolEntry ClientToPhoenix
            {
                get { return clientToPhoenix; }
            }

            public SettingBoolEntry PhoenixToServer
            {
                get { return phoenixToServer; }
            }

            public SettingBoolEntry ServerToPhoenix
            {
                get { return serverToPhoenix; }
            }

            public SettingBoolEntry PhoenixToClient
            {
                get { return phoenixToClient; }
            }
        }

        #endregion

        internal static readonly object SyncRoot = new object();

        private static Settings settings;
        private static ProfileConfig profile;
        private static SettingsFragment userSettings;

        private static PacketLogging packetLogging;

        private static SettingInt32Entry groundFindDistance;
        private static SettingInt32Entry resyncInterval;

        static Config()
        {
            settings = new SynchronizedSettings("Phoenix");
            userSettings = new SettingsFragment(settings, "UserSettings");
            profile = null;
            packetLogging = new PacketLogging(settings);

            groundFindDistance = new SettingInt32Entry(settings, 8, "FindDistance", "Config", "World");
            resyncInterval = new SettingInt32Entry(settings, 5, "ResyncInterval", "Config");
            resyncInterval.Changed += new EventHandler(resyncInterval_Changed);
        }

        static void resyncInterval_Changed(object sender, EventArgs e)
        {
            if (resyncInterval.Value < 2)
                resyncInterval.Value = 2;
        }

        /// <summary>
        /// Called by Phoenix.Init()
        /// </summary>
        internal static void Init()
        {
            lock (Config.SyncRoot) {
                profile = new ProfileConfig(System.IO.Path.Combine(Core.Directory, "Profiles"));
            }
        }

        internal static Settings InternalSettings
        {
            get { return Config.settings; }
        }

        public static ProfileConfig Profile
        {
            get { return Config.profile; }
        }

        public static ISettings UserSettings
        {
            get { return userSettings; }
        }

        public static PacketLogging PacketLog
        {
            get { return packetLogging; }
        }

        public static SettingInt32Entry GroundFindDistance
        {
            get { return Config.groundFindDistance; }
        }

        internal static SettingInt32Entry ResyncInterval
        {
            get { return Config.resyncInterval; }
        }

        [Command("saveconfig")]
        public static void Save()
        {
            lock (Config.SyncRoot) {
                settings.Save();
                profile.Save();
                UO.Print("Configuration saved.");
                Trace.Flush();
            }
        }

        internal static string PlayerProfile
        {
            get
            {
                lock (Config.SyncRoot) {
                    if (WorldData.World.RealPlayer == null)
                        throw new InvalidOperationException("No player logged in.");

                    string playerSerial = WorldData.World.PlayerSerial.ToString("X8");
                    return Config.InternalSettings.GetAttribute(playerSerial, "profile", "Characters", new ElementInfo("Character", new AttributeInfo("serial", playerSerial)));
                }
            }
            set
            {
                lock (Config.SyncRoot) {
                    if (WorldData.World.RealPlayer == null)
                        throw new InvalidOperationException("No player logged in.");

                    string playerSerial = WorldData.World.PlayerSerial.ToString("X8");
                    Config.InternalSettings.SetAttribute(value, "profile", "Characters", new ElementInfo("Character", new AttributeInfo("serial", playerSerial)));
                }
            }
        }
    }
}
