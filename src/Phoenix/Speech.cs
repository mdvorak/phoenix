using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    public enum SpeechType : byte
    {
        Regular = 0,
        Broadcast = 1,
        Emote = 2,

        /// <summary>
        /// You see:
        /// </summary>
        Label = 6,
        Emphasis = 7,
        Whisper = 8,
        Yell = 9,
        Spell = 0xA
    }

    public enum SpeechFont : ushort
    {
        Bold = 0,
        Shadow = 1,
        BoldShadow = 2,
        Normal = 3,
        Gothic = 4,
        Italic = 5,
        SmallDark = 6,
        Colorful = 7,
        Rune = 8,
        SmallLight = 9
    }
}
