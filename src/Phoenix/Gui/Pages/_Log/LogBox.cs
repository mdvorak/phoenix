using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Phoenix.Gui.Controls;

namespace Phoenix.Gui.Pages._Log
{
    [ToolboxItem(false)]
    class LogBox : RichTextBoxEx
    {
        protected struct LogEntry
        {
            public string Category;
            public string Text;
            public string TimeStamp;

            public LogEntry(string category, string text, string timestamp)
            {
                Category = category;
                Text = text;
                TimeStamp = timestamp;
            }
        }

        private readonly object syncRoot = new object();
        private IntPtr safeHandle = IntPtr.Zero;
        private Queue<LogEntry> updateQueue = new Queue<LogEntry>();
        private Dictionary<string, LogCategoryInfo> categories = new Dictionary<string, LogCategoryInfo>();
        private bool autoScroll;

        /// <summary>
        /// Default c..tor
        /// </summary>
        public LogBox()
        {
            categories.Add("Other", new LogCategoryInfo(true, System.Drawing.Color.Black));
            autoScroll = true;
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool AutoScroll
        {
            get { return autoScroll; }
            set { autoScroll = value; }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            safeHandle = Handle;

            base.OnHandleCreated(e);

            if (updateQueue.Count > 0) {
                Native.PostMessage(safeHandle, Native.WM_UPDATECONTENTS, 0, 0);
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            safeHandle = IntPtr.Zero;

            base.OnHandleDestroyed(e);
        }

        protected bool IsSafeHandleCreated
        {
            get { return safeHandle != IntPtr.Zero; }
        }

        public Dictionary<string, LogCategoryInfo> Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        public void Write(string message, string timestamp)
        {
            lock (syncRoot) {
                updateQueue.Enqueue(new LogEntry(null, message, timestamp));
            }

            if (IsSafeHandleCreated) {
                Native.PostMessage(safeHandle, Native.WM_UPDATECONTENTS, 0, 0);
            }
        }

        public void Write(string message, string category, string timestamp)
        {
            lock (syncRoot) {
                updateQueue.Enqueue(new LogEntry(category, message, timestamp));
            }

            if (IsSafeHandleCreated) {
                Native.PostMessage(safeHandle, Native.WM_UPDATECONTENTS, 0, 0);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Native.WM_UPDATECONTENTS) {
                while (updateQueue.Count > 0) {
                    LogEntry entry;

                    lock (syncRoot) {
                        entry = updateQueue.Dequeue();
                    }

                    OnUpdateContents(entry);
                }

                m.Result = IntPtr.Zero;
            }
            else {
                base.WndProc(ref m);
            }
        }

        protected virtual void OnUpdateContents(LogEntry entry)
        {
            if (entry.Text != null) {
                State state = GetState();

                try {
                    LockWindowUpdate();

                    if (entry.Category != null) {
                        LogCategoryInfo info;

                        if (categories.ContainsKey(entry.Category))
                            info = categories[entry.Category];
                        else
                            info = categories["Other"];

                        if (info.Enabled) {
                            SetColor(TextLength, 0, ForeColor);
                            AppendText(entry.TimeStamp);

                            SetColor(TextLength, 0, info.Color);
                            AppendText(entry.Category);

                            SetColor(TextLength, 0, ForeColor);
                            AppendText(": " + entry.Text);
                        }
                    }
                    else {
                        SetColor(TextLength, 0, ForeColor);
                        AppendText(entry.TimeStamp);
                        AppendText(entry.Text);
                    }
                }
                finally {
                    UnlockWindowUpdate();
                }

                // Handle scrolling
                if (autoScroll)
                    ScrollToBottom();
                else
                    SetState(state);
            }
        }
    }
}
