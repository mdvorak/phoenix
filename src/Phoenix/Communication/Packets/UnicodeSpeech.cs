using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication.Packets
{
    public class UnicodeSpeech : SpeechBase
    {
        public const byte PacketId = 0xAE;

        private string language;
        private string name;
        private string text;

        public UnicodeSpeech(byte[] data)
            : base(data)
        {
            language = ByteConverter.BigEndian.ToAsciiString(data, 14, 4);
            name = ByteConverter.BigEndian.ToAsciiString(data, 18, 30);
            text = ByteConverter.BigEndian.ToUnicodeString(data, 48, Lenght - 48);
        }

        public override byte Id
        {
            get { return PacketId; }
        }

        public string Language
        {
            get { return language; }
        }

        public override string Name
        {
            get { return name; }
        }

        public override string Text
        {
            get { return text; }
        }
    }
}
