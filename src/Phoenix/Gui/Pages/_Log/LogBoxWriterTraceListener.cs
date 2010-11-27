using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Phoenix.Gui.Pages._Log
{
    class LogBoxWriterTraceListener : TraceListener
    {
        private LogBox richTextBox;

        public LogBoxWriterTraceListener(LogBox richTextBox)
        {
            this.richTextBox = richTextBox;
        }

        /*
                private void TrimText()
                {
                    if (richTextBox.Lines.Length > MaxLines)
                    {
                        richTextBox.Select(0, richTextBox.GetFirstCharIndexFromLine(5));
                        richTextBox.SelectedText = "";
                    }
                }
        */

        private string FormatTime(DateTime time)
        {
            return String.Format("[{0}:{1:00}:{2:00}] ", time.Hour, time.Minute, time.Second);
        }

        public override void Write(string message, string category)
        {
            richTextBox.Write(message, category, FormatTime(DateTime.Now));
        }

        public override void WriteLine(string message, string category)
        {
            richTextBox.Write(message + "\n", category, FormatTime(DateTime.Now));
        }

        public override void Write(string message)
        {
            richTextBox.Write(message, FormatTime(DateTime.Now));
        }

        public override void WriteLine(string message)
        {
            richTextBox.Write(message + "\n", FormatTime(DateTime.Now));
        }
    }
}
