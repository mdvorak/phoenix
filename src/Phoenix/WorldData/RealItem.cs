using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    class RealItem : RealObject
    {
        public ushort Amount;
        public byte Layer;
        public uint Container;
        public bool Opened;

        public RealItem(uint serial)
            : base(serial)
        {
            Amount = 0;
            Layer = 0;
            Container = 0;
            Opened = false;
        }

        public override string Description
        {
            get
            {
                return String.Format(base.Description + "  Graphic: 0x{0:X4}  Amount: {1}  Layer: {2}  Container: 0x{3:X8}",
                    Graphic, Amount, (Phoenix.WorldData.Layer)Layer, Container);
            }
        }

        internal void Detach()
        {
            RealCharacter chr = World.FindRealCharacter(Container);
            if (chr != null)
            {
                chr.Layers[Layer] = 0;
                Layer = 0;
                Container = 0;
            }
        }
    }
}
