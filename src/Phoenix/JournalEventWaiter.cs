using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    public class JournalEventWaiter : EventWaiter<JournalEntryAddedEventArgs>
    {
        private string[] searchedText;
        private bool ignoreCase;

        public JournalEventWaiter(bool ignoreCase, params string[] searchedText)
        {
            this.ignoreCase = ignoreCase;

            if (ignoreCase)
            {
                this.searchedText = new string[searchedText.Length];

                for (int i = 0; i < searchedText.Length; i++)
                {
                    this.searchedText[i] = searchedText[i].ToLowerInvariant();
                }
            }
            else
            {
                this.searchedText = searchedText;
            }

            Journal.EntryAdded += new JournalEntryAddedEventHandler(this.Handler);
        }

        protected override bool OnEventArgsTest(object eventSender, JournalEntryAddedEventArgs eventArgs)
        {
            string line = eventArgs.Entry.ToString();
            if (ignoreCase)
                line = line.ToLowerInvariant();

            for (int i = 0; i < searchedText.Length; i++)
            {
                if (line.Contains(searchedText[i]))
                {
                    Journal.EntryAdded -= new JournalEntryAddedEventHandler(this.Handler);
                    return true;
                }
            }

            return false;
        }

        public override void Dispose()
        {
            Journal.EntryAdded -= new JournalEntryAddedEventHandler(this.Handler);
            searchedText = null;

            base.Dispose();
        }
    }
}
