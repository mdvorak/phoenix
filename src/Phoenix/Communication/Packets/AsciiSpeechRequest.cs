using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication.Packets
{
    public class AsciiSpeechRequest : VariablePacket
    {
        public const byte PacketId = 0x03;


        private byte type;
        private ushort color;
        private ushort font;
        private string text;

        public AsciiSpeechRequest(byte[] data)
            : base(data)
        {
            type = ByteConverter.BigEndian.ToByte(data, 3);
            color = ByteConverter.BigEndian.ToUInt16(data, 4);
            font = ByteConverter.BigEndian.ToUInt16(data, 6);
            text = ByteConverter.BigEndian.ToAsciiString(data, 8, Lenght - 8);
        }

        public override byte Id
        {
            get { return PacketId; }
        }

        public ushort Type
        {
            get { return type; }
        }

        public ushort Color
        {
            get { return color; }
        }

        public ushort Font
        {
            get { return font; }
        }

        public string Text
        {
            get { return text; }
        }
    }
}
