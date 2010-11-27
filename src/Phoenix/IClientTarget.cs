using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    public enum TargetType : byte
    {
        Object = 0,
        Ground = 1
    }

    public interface IClientTarget : ICloneable
    {
        byte Flags { get; }
        TargetType Type { get; }
        uint Serial { get; }
        ushort X { get; }
        ushort Y { get; }
        sbyte Z { get; }
        ushort Graphic { get; }
    }
}
