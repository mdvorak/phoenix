using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Phoenix.Logging
{
    public class TextFileTraceListener : TextWriterTraceListener
    {
        public TextFileTraceListener()
            : base()
        {
            Initialize();
        }

        public TextFileTraceListener(Stream stream)
            : base(stream)
        {
            Initialize();
        }

        public TextFileTraceListener(string fileName)
            : base(fileName)
        {
            Initialize();
        }

        public TextFileTraceListener(TextWriter writer)
            : base(writer)
        {
            Initialize();
        }

        public TextFileTraceListener(Stream stream, string name)
            : base(stream, name)
        {
            Initialize();
        }

        public TextFileTraceListener(string fileName, string name)
            : base(fileName, name)
        {
            Initialize();
        }

        public TextFileTraceListener(TextWriter writer, string name)
            : base(writer, name)
        {
            Initialize();
        }

        private void Initialize()
        {
            WriteLine("*** Logging started at " + DateTime.Now.ToString() + " ***");
        }

        private string FormatTime(DateTime time)
        {
            return String.Format("[{0}:{1:00}:{2:00}] ", time.Hour, time.Minute, time.Second);
        }

        public override void Write(string message)
        {
            base.Write(FormatTime(DateTime.Now) + message);
        }

        public override void WriteLine(string message)
        {
            base.WriteLine(FormatTime(DateTime.Now) + message);
        }
    }
}
