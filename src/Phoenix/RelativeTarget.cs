using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;

namespace Phoenix
{
    public class RelativeTarget : IClientTarget
    {
        private byte flags;
        private int relativeX;
        private int relativeY;
        private int relativeZ;
        private ushort graphic;

        public RelativeTarget(int relativeX, int relativeY, int relativeZ, ushort graphic)
        {
            this.relativeX = relativeX;
            this.relativeY = relativeY;
            this.relativeZ = relativeZ;
        }

        public uint Serial
        {
            get { return 0; }
        }

        public byte Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public TargetType Type
        {
            get { return (graphic > 0) ? TargetType.Object : TargetType.Ground; }
        }

        public ushort X
        {
            get { return (ushort)(World.RealPlayer.X + relativeX); }
            set { relativeX = value - World.RealPlayer.X; }
        }

        public ushort Y
        {
            get { return (ushort)(World.RealPlayer.Y + relativeY); }
            set { relativeY = value - World.RealPlayer.Y; }
        }

        public sbyte Z
        {
            get { return (sbyte)(World.RealPlayer.Z + relativeZ); }
            set { relativeZ = value - World.RealPlayer.Z; }
        }

        public ushort Graphic
        {
            get { return graphic; }
            set { graphic = value; }
        }

        public virtual object Clone()
        {
            RelativeTarget ti = new RelativeTarget(relativeX, relativeY, relativeZ, Graphic);
            ti.flags = flags;
            return ti;
        }

        public static RelativeTarget FromAbsolute(ushort absoluteX, ushort absoluteY, sbyte absoluteZ, ushort graphic)
        {
            RealCharacter player = World.RealPlayer;
            System.Diagnostics.Debug.Assert(player.Serial != World.InvalidSerial);
            return new RelativeTarget(absoluteX - player.X, absoluteY - player.Y, absoluteZ - player.Z, graphic);
        }
    }
}
