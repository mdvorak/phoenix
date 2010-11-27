using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    public class JournalEntryAddedEventArgs : EventArgs
    {
        private JournalEntry entry;

        public JournalEntryAddedEventArgs(JournalEntry entry)
        {
            this.entry = entry;
        }

        public JournalEntry Entry
        {
            get { return entry; }
        }
    }

    public delegate void JournalEntryAddedEventHandler(object sender, JournalEntryAddedEventArgs e);
}
