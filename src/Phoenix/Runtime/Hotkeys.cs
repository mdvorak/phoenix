using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Phoenix.Configuration;

namespace Phoenix.Runtime
{
    public class HotkeyShortcutChangedEventArgs : EventArgs
    {
        private string shortcut;

        public HotkeyShortcutChangedEventArgs(string shortcut)
        {
            this.shortcut = shortcut;
        }

        public string Shortcut
        {
            get { return shortcut; }
        }
    }

    public delegate void HotkeyShortcutChangedEventHandler(object sender, HotkeyShortcutChangedEventArgs e);

    internal class HotkeyShortcutChangedPublicEvent : PublicEvent<HotkeyShortcutChangedEventHandler, HotkeyShortcutChangedEventArgs>
    {
    }

    public class Hotkeys
    {
        private readonly object syncRoot;
        private Dictionary<string, CommandList> hotkeys;
        private HotkeyShortcutChangedPublicEvent added;
        private HotkeyShortcutChangedPublicEvent removed;
        private HotkeyShortcutChangedPublicEvent updated;
        private DefaultPublicEvent cleared;

        public Hotkeys()
        {
            syncRoot = new object();
            hotkeys = new Dictionary<string, CommandList>();

            added = new HotkeyShortcutChangedPublicEvent();
            removed = new HotkeyShortcutChangedPublicEvent();
            updated = new HotkeyShortcutChangedPublicEvent();
            cleared = new DefaultPublicEvent();
        }

        internal void Init()
        {
            Config.Profile.InternalSettings.Loaded += new EventHandler(Settings_Loaded);
            Config.Profile.InternalSettings.Saving += new EventHandler(Settings_Saving);
        }

        public event HotkeyShortcutChangedEventHandler Added
        {
            add { added.AddHandler(value); }
            remove { added.RemoveHandler(value); }
        }

        public event HotkeyShortcutChangedEventHandler Removed
        {
            add { removed.AddHandler(value); }
            remove { removed.RemoveHandler(value); }
        }

        public event HotkeyShortcutChangedEventHandler Updated
        {
            add { updated.AddHandler(value); }
            remove { updated.RemoveHandler(value); }
        }

        public event EventHandler Cleared
        {
            add { cleared.AddHandler(value); }
            remove { cleared.RemoveHandler(value); }
        }

        void Settings_Loaded(object sender, EventArgs e)
        {
            Reload();
        }

        internal void Reload()
        {
            lock (syncRoot) {
                hotkeys.Clear();
                cleared.Invoke(this, EventArgs.Empty);

                ElementInfo[] elements = Config.Profile.InternalSettings.EnumarateElements("Hotkeys", "Hotkey");

                foreach (ElementInfo ei in elements) {
                    try {
                        if (ei.Value != null) {
                            Add(ei.Attributes["shortcut"].ToString(), ei.Value);
                        }
                    }
                    catch (Exception e) {
                        Trace.WriteLine("Error reading hotkey from config. Message: " + e.Message, "Hotkeys");
                    }
                }

                Debug.WriteLine("Reloaded.", "Hotkeys");
            }
        }

        void Settings_Saving(object sender, EventArgs e)
        {
            lock (syncRoot) {
                Config.Profile.InternalSettings.RemoveElement("Hotkeys");

                foreach (KeyValuePair<string, CommandList> pair in hotkeys) {
                    Config.Profile.InternalSettings.SetElement(pair.Value.Text, "Hotkeys", new ElementInfo("Hotkey", new AttributeInfo("shortcut", pair.Key)));
                }
            }
        }

        public bool Contains(string shortcut)
        {
            lock (syncRoot) {
                return hotkeys.ContainsKey(shortcut);
            }
        }

        public string Get(string shortcut)
        {
            lock (syncRoot) {
                if (!hotkeys.ContainsKey(shortcut))
                    throw new Exception("Hotkey doesn't exist.");

                return hotkeys[shortcut].Text;
            }
        }

        /// <param name="commands">Multiple commands can be separated by '\n'.</param>
        public void Update(string shortcut, string commands)
        {
            if (commands == null)
                throw new ArgumentNullException("commands");

            lock (syncRoot) {
                if (!hotkeys.ContainsKey(shortcut))
                    throw new Exception("Hotkey doesn't exist.");

                hotkeys[shortcut] = new CommandList(commands);
            }

            updated.Invoke(this, new HotkeyShortcutChangedEventArgs(shortcut));
        }

        /// <param name="commands">Multiple commands can be separated by '\n'.</param>
        public void Add(string shortcut, string commands)
        {
            if (shortcut == null || shortcut.Trim().Length == 0 || shortcut.Trim() == "None")
                throw new ArgumentException("No shortcut specified.", "shortcut");

            if (commands == null)
                throw new ArgumentNullException("commands");

            shortcut = shortcut.Trim();

            lock (syncRoot) {
                if (hotkeys.ContainsKey(shortcut))
                    throw new Exception("Hotkey already exists.");

                hotkeys.Add(shortcut, new CommandList(commands));
            }

            added.Invoke(this, new HotkeyShortcutChangedEventArgs(shortcut));
        }

        public void Remove(string shortcut)
        {
            lock (syncRoot) {
                if (!hotkeys.ContainsKey(shortcut))
                    throw new Exception("Hotkey doesn't exist.");

                hotkeys.Remove(shortcut);
            }

            removed.Invoke(this, new HotkeyShortcutChangedEventArgs(shortcut));
        }

        public void Run(string shortcut)
        {
            CommandList cmdList;

            lock (syncRoot) {
                hotkeys.TryGetValue(shortcut, out cmdList);
            }

            if (cmdList != null) {
                cmdList.Run();
            }
            else {
                throw new RuntimeException("Hotkey " + shortcut.ToString() + " not found.");
            }
        }

        [Command("hotkey")]
        public void Exec(string shortcut)
        {
            lock (syncRoot) {
                CommandList cmdList;

                if (hotkeys.TryGetValue(shortcut, out cmdList)) {
                    ThreadStarter.StartBackround(new WorkerDelegate(Worker), shortcut, cmdList);
                }
                else {
                    UO.PrintError("Hotkey {0} not found.", shortcut);
                }
            }
        }

        private delegate void WorkerDelegate(string shortcut, CommandList cmdList);
        private static void Worker(string shortcut, CommandList cmdList)
        {
            try {
                Debug.WriteLine("Hotkey execution " + shortcut + " started.", "Runtime");

                cmdList.Run();
            }
            catch (ThreadAbortException) {
                UO.PrintInformation("Hotkey {0} execution terminated.", shortcut);
            }
            catch (ScriptErrorException e) {
                Trace.WriteLine("Unhandled exception:\n" + e.ToString(), "Script");
                UO.PrintError(e.Message);
            }
            catch (Exception e) {
                Trace.WriteLine("Unhandled error during hotkey execution. Exception:" + Environment.NewLine + e.ToString(), "Runtime");
                UO.PrintError("Hotkey error: {0}", e.Message);
            }
            finally {
                Debug.WriteLine("Hotkey execution " + shortcut.ToString() + " finished.", "Runtime");
            }
        }
    }
}
