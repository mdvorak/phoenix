using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication.Packets
{
    public class VariablePacket : PacketBase
    {
        public VariablePacket(byte[] data)
            : base(data)
        {
        }

        public override int Lenght
        {
            get { return ByteConverter.BigEndian.ToUInt16(Data, 1); }
        }
    }
}
