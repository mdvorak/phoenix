using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Communication;

namespace Phoenix
{
    public class StaticTarget : IClientTarget
    {
        private byte flags;
        private uint serial;
        private ushort x;
        private ushort y;
        private sbyte z;
        private ushort graphic;

        public StaticTarget()
        {
        }

        public StaticTarget(uint serial, ushort x, ushort y, sbyte z, ushort graphic)
        {
            this.serial = serial;
            this.x = x;
            this.y = y;
            this.z = z;
            this.graphic = graphic;
        }

        public uint Serial
        {
            get { return serial; }
            set { serial = value; }
        }

        public byte Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public TargetType Type
        {
            get { return (serial > 0 || graphic > 0) ? TargetType.Object : TargetType.Ground; }
        }

        public ushort X
        {
            get { return x; }
            set { x = value; }
        }

        public ushort Y
        {
            get { return y; }
            set { y = value; }
        }

        public sbyte Z
        {
            get { return z; }
            set { z = value; }
        }

        public ushort Graphic
        {
            get { return graphic; }
            set { graphic = value; }
        }

        public virtual object Clone()
        {
            StaticTarget ti = new StaticTarget();
            ti.Flags = Flags;
            ti.serial = serial;
            ti.x = x;
            ti.y = y;
            ti.z = z;
            ti.graphic = graphic;
            return ti;
        }
    }
}
