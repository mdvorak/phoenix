using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    class RealObject
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (value != null && value.Contains("\0"))
                    name = value.Remove(value.IndexOf('\0'));
                else
                    name = value;
            }
        }

        public readonly uint Serial;
        public ushort Color;
        public ushort X;
        public ushort Y;
        public sbyte Z;
        public byte Flags;
        public ushort Graphic;

        public RealObject(uint serial)
        {
            name = null;
            Serial = serial & 0x7FFFFFFF;
            Color = 0;
            Z = 0;
            Y = 0;
            X = 0;
            Flags = 0;
            Graphic = 0;
        }

        public int GetDistance(RealObject obj)
        {
            return Math.Max(Math.Abs(obj.X - this.X), Math.Abs(obj.Y - this.Y));
        }

        public override string ToString()
        {
            return "0x" + Serial.ToString("X8");
        }

        public virtual string Description
        {
            get
            {
                string format = "Serial: 0x{0:X8}  ";

                if (Name != null && Name.Length > 0) format += "Name: \"{1}\"  ";

                return String.Format(format + "Position: {2}.{3}.{4}  Flags: 0x{5:X4}  Color: 0x{6:X4}",
                    Serial, Name, X, Y, Z, Flags, Color);
            }
        }
    }
}
