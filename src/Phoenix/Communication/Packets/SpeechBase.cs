using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication.Packets
{
    public abstract class SpeechBase : VariablePacket
    {
        private uint serial;
        private ushort graphic;
        private byte type;
        private ushort color;
        private ushort font;

        public SpeechBase(byte[] data)
            : base(data)
        {
            serial = ByteConverter.BigEndian.ToUInt32(data, 3);
            graphic = ByteConverter.BigEndian.ToUInt16(data, 7);
            type = ByteConverter.BigEndian.ToByte(data, 9);
            color = ByteConverter.BigEndian.ToUInt16(data, 10);
            font = ByteConverter.BigEndian.ToUInt16(data, 12);
        }

        public uint Serial
        {
            get { return serial; }
        }

        public ushort Graphic
        {
            get { return graphic; }
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

        public abstract string Name { get;}

        public abstract string Text { get;}
    }
}
