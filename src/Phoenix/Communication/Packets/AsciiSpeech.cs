using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication.Packets
{
    public class AsciiSpeech : SpeechBase
    {
        public const byte PacketId = 0x1C;

        private string name;
        private string text;

        public AsciiSpeech(byte[] data)
            : base(data)
        {
            name = ByteConverter.BigEndian.ToAsciiString(data, 14, 30);
            text = ByteConverter.BigEndian.ToAsciiString(data, 44, Lenght - 44);
        }

        public override byte Id
        {
            get { return PacketId; }
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
