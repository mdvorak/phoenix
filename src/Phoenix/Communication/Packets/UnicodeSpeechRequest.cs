using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication.Packets
{
    public class UnicodeSpeechRequest : VariablePacket
    {
        public const byte PacketId = 0xAD;

        private byte type;
        private ushort color;
        private ushort font;
        private string language;
        private string text;

        public UnicodeSpeechRequest(byte[] data)
            : base(data)
        {
            PacketReader reader = new PacketReader(data);
            byte id = reader.ReadByte();
            ushort blockSize = reader.ReadUInt16();

            byte type = reader.ReadByte();
            this.type = (byte)(type & 0x0F);
            color = reader.ReadUInt16();
            font = reader.ReadUInt16();
            language = reader.ReadAnsiString(4);

            if ((type & 0xC0) != 0)
            {
                int numKeywords = (Data[12] << 4) + (Data[13] >> 4);
                reader.Skip(((numKeywords * 12 + 12) + 7) / 8);

                text = reader.ReadAnsiString(Lenght - reader.Offset);
            }
            else
            {
                text = reader.ReadUnicodeString(Lenght - reader.Offset);
            }
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

        public string Language
        {
            get { return language; }
        }

        public string Text
        {
            get { return text; }
        }
    }
}
