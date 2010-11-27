using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Phoenix.Collections;
using System.Security.Permissions;

namespace Phoenix
{
    public static class Journal
    {
        /// <summary>
        /// Maximum number of lines in thread journal.
        /// </summary>
        /// <remarks>Added because of long-time running scripts.</remarks>
        public const int MaxLenght = 500;

        public class Instance
        {
            internal readonly List<JournalEntry> History = new List<JournalEntry>(128);
            internal readonly object SyncRoot = new object();
            internal readonly Thread Thread = Thread.CurrentThread;
            internal int LineOffset = 0;

            internal Instance()
            {
            }

            internal void Clear()
            {
                lock (SyncRoot) {
                    History.Clear();
                    LineOffset = 0;
                }
            }
        }

        private static Instance defaultInstance = new Instance();
        private static DictionaryEx<int, Instance> instanceList;

        private static PublicEvent<JournalEntryAddedEventHandler, JournalEntryAddedEventArgs> entryAddedEvent =
            new PublicEvent<JournalEntryAddedEventHandler, JournalEntryAddedEventArgs>();

        /// <summary>
        /// Called by Phoenix.Init();
        /// </summary>
        internal static void Init()
        {
            instanceList = new DictionaryEx<int, Instance>().CreateSynchronized();
            defaultInstance = new Instance();
            Phoenix.Logging.JournalHandler.JournalEntryAdded += new JournalEntryAddedEventHandler(JournalHandler_JournalEntryAdded);
        }

        internal static void ThreadInit()
        {
            int id = Thread.CurrentThread.ManagedThreadId;

            if (!instanceList.ContainsKey(id)) {
                instanceList.Add(id, new Instance());
            }
        }

        private static Instance CurrentInstance
        {
            get
            {
                Instance instance;
                if (instanceList.TryGetValue(Thread.CurrentThread.ManagedThreadId, out instance))
                    return instance;
                else
                    return defaultInstance;
            }
        }

        private static void UpdateInstance(Instance instance, JournalEntryAddedEventArgs e)
        {
            lock (instance.SyncRoot) {
                instance.History.Add(e.Entry);

                if (instance.History.Count >= MaxLenght) {
                    instance.History.RemoveAt(0);
                    instance.LineOffset++;
                }
            }
        }

        static void JournalHandler_JournalEntryAdded(object sender, JournalEntryAddedEventArgs e)
        {
            UpdateInstance(defaultInstance, e);

            List<int> removeList = new List<int>();

            lock (instanceList.SyncRoot) {
                foreach (KeyValuePair<int, Instance> pair in instanceList) {
                    if (pair.Value.Thread.IsAlive) {
                        UpdateInstance(pair.Value, e);
                    }
                    else {
                        removeList.Add(pair.Key);
                    }
                }
            }

            for (int i = 0; i < removeList.Count; i++) {
                instanceList.Remove(removeList[i]);
            }

            try {
                entryAddedEvent.Invoke(null, e);
            }
            catch (Exception ex) {
                System.Diagnostics.Trace.WriteLine("Unhandled exception in Journal.LineAdded event. Details:\n" + ex.ToString(), "Error");
            }
        }

        /// <summary>
        /// Raised when new entry from server is added to journal.
        /// </summary>
        public static event JournalEntryAddedEventHandler EntryAdded
        {
            add { entryAddedEvent.AddHandler(value); }
            remove { entryAddedEvent.RemoveHandler(value); }
        }

        /// <summary>
        /// Clears history for current thread.
        /// </summary>
        public static void Clear()
        {
            CurrentInstance.Clear();
        }

        /// <summary>
        /// Clears history for all threads.
        /// </summary>
        public static void ClearAll()
        {
            lock (instanceList.SyncRoot) {
                foreach (KeyValuePair<int, Instance> pair in instanceList) {
                    pair.Value.Clear();
                }
            }
        }

        /// <summary>
        /// Returns whether journal contains specified text or not. Test is not case sensitive.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Contains(string text)
        {
            return Contains(true, text);
        }

        /// <summary>
        /// Returns whether journal contains specified text or not.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ignoreCase">Specifies whether test will be case sensitive or not.</param>
        /// <returns></returns>
        public static bool Contains(bool ignoreCase, string text)
        {
            Instance instance = CurrentInstance;

            if (ignoreCase)
                text = text.ToLowerInvariant();

            lock (instance.SyncRoot) {
                for (int i = 0; i < instance.History.Count; i++) {
                    string line = instance.History[i].ToString();
                    if (ignoreCase)
                        line = line.ToLowerInvariant();

                    if (line.Contains(text)) {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Returns whether journal contains specified text or not. Test is not case sensitive.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Contains(params string[] text)
        {
            return Contains(true, text);
        }

        /// <summary>
        /// Returns whether journal contains specified text or not.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ignoreCase">Specifies whether test will be case sensitive or not.</param>
        /// <returns></returns>
        public static bool Contains(bool ignoreCase, params string[] text)
        {
            Instance instance = CurrentInstance;

            string[] searchedText;

            if (ignoreCase) {
                searchedText = new string[text.Length];

                for (int i = 0; i < text.Length; i++) {
                    if (text[i] != null)
                        searchedText[i] = text[i].ToLowerInvariant();
                }
            }
            else {
                searchedText = text;
            }

            lock (instance.SyncRoot) {
                for (int i = 0; i < instance.History.Count; i++) {
                    string line = instance.History[i].ToString();
                    if (ignoreCase)
                        line = line.ToLowerInvariant();

                    for (int a = 0; a < searchedText.Length; a++) {
                        if (searchedText[a] != null && line.Contains(searchedText[a])) {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public static int Find(string text)
        {
            return Find(true, text);
        }

        public static int Find(bool ignoreCase, string text)
        {
            Instance instance = CurrentInstance;

            if (ignoreCase)
                text = text.ToLowerInvariant();

            lock (instance.SyncRoot) {
                for (int i = 0; i < instance.History.Count; i++) {
                    string line = instance.History[i].ToString();
                    if (ignoreCase)
                        line = line.ToLowerInvariant();

                    if (line.Contains(text)) {
                        return instance.LineOffset + i;
                    }
                }

                return -1;
            }
        }

        public static int Find(params string[] text)
        {
            return Find(true, text);
        }

        public static int Find(bool ignoreCase, params string[] text)
        {
            Instance instance = CurrentInstance;

            string[] searchedText;

            if (ignoreCase) {
                searchedText = new string[text.Length];

                for (int i = 0; i < text.Length; i++) {
                    if (text[i] != null)
                        searchedText[i] = text[i].ToLowerInvariant();
                }
            }
            else {
                searchedText = text;
            }

            lock (instance.SyncRoot) {
                for (int i = 0; i < instance.History.Count; i++) {
                    string line = instance.History[i].ToString();
                    if (ignoreCase)
                        line = line.ToLowerInvariant();

                    for (int a = 0; a < searchedText.Length; a++) {
                        if (searchedText[a] != null && line.Contains(searchedText[a])) {
                            return instance.LineOffset + i;
                        }
                    }
                }

                return -1;
            }
        }

        public static string GetLineText(int line)
        {
            Instance instance = CurrentInstance;

            lock (instance.SyncRoot) {
                int histLine = line - instance.LineOffset;

                if (histLine >= 0 && histLine < instance.History.Count) {
                    return instance.History[histLine].Text;
                }
                else {
                    return "";
                }
            }
        }

        public static JournalEntry GetLine(int line)
        {
            Instance instance = CurrentInstance;

            lock (instance.SyncRoot) {
                int histLine = line - instance.LineOffset;

                if (histLine >= 0 && histLine < instance.History.Count) {
                    return instance.History[histLine];
                }
                else {
                    return new JournalEntry(new DateTime(), 0, "", "", 0, SpeechType.Regular, SpeechFont.Normal, JournalEntrySource.Phoenix);
                }
            }
        }

        public static bool SetLineText(int line, string text)
        {
            Instance instance = CurrentInstance;

            lock (instance.SyncRoot) {
                int histLine = line - instance.LineOffset;

                if (histLine >= 0 && histLine < instance.History.Count) {
                    instance.History[histLine] = new JournalEntry(instance.History[histLine], text);
                    return true;
                }
                else return false;
            }
        }

        public static bool SetLine(int line, JournalEntry e)
        {
            Instance instance = CurrentInstance;

            lock (instance.SyncRoot) {
                int histLine = line - instance.LineOffset;

                if (histLine >= 0 && histLine < instance.History.Count) {
                    instance.History[histLine] = e;
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// Waits until one of requested strings does not appear in journal.
        /// </summary>
        /// <param name="ignoreCase">If true character case is ignored.</param>
        /// <param name="timeout">Time in milliseconds before function times out. Use -1 to wait for infinite.</param>
        /// <param name="text">List of text that script is waiting for.</param>
        /// <returns>Returns true if text appeared in journal. False when timeout.</returns>
        public static bool WaitForText(bool ignoreCase, int timeout, params string[] text)
        {
            using (JournalEventWaiter obj = new JournalEventWaiter(ignoreCase, text)) {
                if (Contains(ignoreCase, text))
                    return true;

                return obj.Wait(timeout);
            }
        }

        public static void WaitForText(bool ignoreCase, params string[] text)
        {
            WaitForText(ignoreCase, Timeout.Infinite, text);
        }

        [Command("waitjournal")]
        public static void WaitForText(params string[] text)
        {
            WaitForText(true, Timeout.Infinite, text);
        }
    }
}
