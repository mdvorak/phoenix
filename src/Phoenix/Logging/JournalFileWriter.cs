using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Phoenix.WorldData;
using System.Threading;
using System.Diagnostics;

namespace Phoenix
{
    internal class JournalFileWriter : IDisposable
    {
        private readonly object syncRoot = new object();
        private TextWriter writer;
        private Timer flushTimer;

        public JournalFileWriter(string file)
        {
            writer = File.AppendText(file);
            flushTimer = new Timer(new TimerCallback(FlushCallback), null, 5000, 5000);

            Core.Disconnected += new EventHandler(Core_Disconnected);
            Logging.JournalHandler.JournalEntryAdded += new JournalEntryAddedEventHandler(JournalHandler_JournalEntryAdded);
        }

        public bool Disposed
        {
            get { return writer == null; }
        }

        void FlushCallback(object unused)
        {
            try {
                lock (syncRoot) {
                    if (writer != null)
                        writer.Flush();
                }
            }
            catch (Exception e) {
                Trace.WriteLine("Unhandled error during JournalFileWriter flush:\n" + e.ToString(), "Warning");
            }
        }

        void Core_Disconnected(object sender, EventArgs e)
        {
            Flush();
        }

        void JournalHandler_JournalEntryAdded(object sender, JournalEntryAddedEventArgs e)
        {
            ushort color = 0;
            if (DataFiles.Hues != null && e.Entry.Color > DataFiles.Hues.MinIndex && e.Entry.Color < DataFiles.Hues.MaxIndex) {
                color = DataFiles.Hues[e.Entry.Color].Colors[24];
            }

            string line = String.Format("[{0:X}.{1:X}:{2:00}:{3:00}:{4:00} {5:X4}] {6}",
                e.Entry.TimeStamp.Year - 2000, e.Entry.TimeStamp.DayOfYear,
                e.Entry.TimeStamp.Hour, e.Entry.TimeStamp.Minute, e.Entry.TimeStamp.Second, color, e.Entry);

            lock (syncRoot) {
                if (writer != null)
                    writer.WriteLine(line);
            }
        }

        public void Flush()
        {
            lock (syncRoot) {
                writer.Flush();
            }
        }

        public void Close()
        {
            lock (syncRoot) {
                flushTimer.Dispose();

                Core.Disconnected -= Core_Disconnected;
                Logging.JournalHandler.JournalEntryAdded -= JournalHandler_JournalEntryAdded;

                writer.Close();
                writer = null;
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }
    }
}
