using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication.Packets
{
    public sealed class TargetData : ICloneable
    {
        public byte Type;
        public uint TargetId;
        public byte Flags;
        public uint Serial;
        public ushort X;
        public ushort Y;
        public sbyte Z;
        public ushort Graphic;

        internal byte[] ToData()
        {
            return PacketBuilder.Target(Type, TargetId, Flags, Serial, X, Y, Z, Graphic);
        }

        public object Clone()
        {
            TargetData td = new TargetData();
            td.Type = Type;
            td.TargetId = TargetId;
            td.Flags = Flags;
            td.Serial = Serial;
            td.X = X;
            td.Y = Y;
            td.Z = Z;
            td.Graphic = Graphic;
            return td;
        }

        internal static byte[] ToData(IClientTarget target, uint targetId)
        {
            return PacketBuilder.Target((byte)target.Type, targetId, target.Flags, target.Serial, target.X, target.Y, target.Z, target.Graphic);
        }

        internal static TargetData FromData(byte[] data)
        {
            if (data[0] != 0x6C) throw new ArgumentException("Invalid packet.");

            TargetData targetData = new TargetData();
            targetData.Type = data[1];
            targetData.TargetId = ByteConverter.BigEndian.ToUInt32(data, 2);
            targetData.Flags = data[6];
            targetData.Serial = ByteConverter.BigEndian.ToUInt32(data, 7);
            targetData.X = ByteConverter.BigEndian.ToUInt16(data, 11);
            targetData.Y = ByteConverter.BigEndian.ToUInt16(data, 13);
            targetData.Z = ByteConverter.BigEndian.ToSByte(data, 16);
            targetData.Graphic = ByteConverter.BigEndian.ToUInt16(data, 17);

            return targetData;
        }
    }
}
