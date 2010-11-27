using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    public enum JournalEntrySource
    {
        /// <summary>
        /// Text was sent by server.
        /// </summary>
        Server,
        /// <summary>
        /// Text was sent by Phoenix or script.
        /// </summary>
        Phoenix
    }

    public class JournalEntry
    {
        public JournalEntry(DateTime time, uint serial, string name, string text, ushort color, SpeechType type, SpeechFont font, JournalEntrySource source)
        {
            TimeStamp = time;
            Serial = serial;
            Name = name;
            Text = text;
            Color = color;
            Type = type;
            Font = font;
            Source = source;
        }

        public JournalEntry(JournalEntry e, string text)
        {
            TimeStamp = e.TimeStamp;
            Serial = e.Serial;
            Name = e.Name;
            Text = text;
            Color = e.Color;
            Type = e.Type;
            Font = e.Font;
            Source = e.Source;
        }

        /// <summary>
        /// Gets time when entry was received/created.
        /// </summary>
        public readonly DateTime TimeStamp;
        public readonly uint Serial;
        public readonly string Name;
        public readonly string Text;
        public readonly ushort Color;
        public readonly SpeechType Type;
        public readonly SpeechFont Font;
        public readonly JournalEntrySource Source;

        public override string ToString()
        {
            string name = (Type == SpeechType.Label ? "You see" : Name);
            return String.Format("{0}: {1}", name, Text);
        }
    }
}
