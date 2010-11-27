using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Phoenix.Communication.Packets;

namespace Phoenix.Logging
{
    internal static class JournalHandler
    {
        private static readonly object syncRoot;
        private static Queue<JournalEntry> newEntryQueue;
        private static AutoResetEvent newEntryEvent;
        private static Thread procThread;

        public static event JournalEntryAddedEventHandler JournalEntryAdded;

        static JournalHandler()
        {
            syncRoot = new object();
            newEntryQueue = new Queue<JournalEntry>();
            newEntryEvent = new AutoResetEvent(false);
            procThread = new Thread(new ThreadStart(EventProc));
            procThread.Name = "JournalHandlerEventProc";
            procThread.IsBackground = true;
            procThread.Start();
        }

        /// <summary>
        /// Called by Phoenix.Initialize()
        /// </summary>
        internal static void Init()
        {
            Core.RegisterServerMessageCallback(0x1C, new MessageCallback(OnAsciiSpeech));
            Core.RegisterServerMessageCallback(0xAE, new MessageCallback(OnUnicodeSpeech));

            Core.LoginComplete += new EventHandler(Core_LoginComplete);
            Core.Disconnected += new EventHandler(Core_Disconnected);
        }

        private static void OnJournalEntryAdded(JournalEntryAddedEventArgs e)
        {
            SyncEvent.Invoke(JournalEntryAdded, null, e);
        }

        private static void EventProc()
        {
            while (true)
            {
                newEntryEvent.WaitOne();

                try
                {
                    while (newEntryQueue.Count > 0)
                    {
                        JournalEntry e = null;
                        lock (syncRoot)
                        {
                            Debug.Assert(newEntryQueue.Peek() != null, "null in newEntryQueue.");
                            e = newEntryQueue.Dequeue();
                        }

                        if (e != null)
                        {
                            Trace.WriteLine(String.Format("0x{0:X8} 0x{1:X4} ", e.Serial, e.Color) + e.ToString(), "Journal");
                            OnJournalEntryAdded(new JournalEntryAddedEventArgs(e));
                        }
                    }
                }
                catch { }
            }
        }

        static void Core_LoginComplete(object sender, EventArgs e)
        {
            string name = WorldData.World.RealPlayer.Name;
            if (name == null) name = WorldData.World.RealPlayer.ToString();

            String msg = String.Format("Session {0} started at {1}.", name, DateTime.Now);
            Print(new JournalEntry(DateTime.Now, 0, "Journal", msg, 1, SpeechType.Regular, SpeechFont.Normal, JournalEntrySource.Phoenix));
        }

        static void Core_Disconnected(object sender, EventArgs e)
        {
            String msg = String.Format("Session {0} ended at {1}.\n", WorldData.World.RealPlayer, DateTime.Now);
            Print(new JournalEntry(DateTime.Now, 0, "Journal", msg, 1, SpeechType.Regular, SpeechFont.Normal, JournalEntrySource.Phoenix));
        }

        private static CallbackResult OnAsciiSpeech(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);
            Print(new JournalEntry(DateTime.Now, packet.Serial, packet.Name, packet.Text, packet.Color, (SpeechType)packet.Type, (SpeechFont)packet.Font, JournalEntrySource.Server));
            return CallbackResult.Normal;
        }

        private static CallbackResult OnUnicodeSpeech(byte[] data, CallbackResult prevResult)
        {
            UnicodeSpeech packet = new UnicodeSpeech(data);
            Print(new JournalEntry(DateTime.Now, packet.Serial, packet.Name, packet.Text, packet.Color, (SpeechType)packet.Type, (SpeechFont)packet.Font, JournalEntrySource.Server));
            return CallbackResult.Normal;
        }

        internal static void Print(JournalEntry entry)
        {
            lock (syncRoot)
            {
                newEntryQueue.Enqueue(entry);
                newEntryEvent.Set();
            }
        }
    }
}
