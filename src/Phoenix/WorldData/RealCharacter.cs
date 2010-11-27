using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    class RealCharacter : RealObject
    {
        public short Hits;
        public short MaxHits;
        public short Stamina;
        public short MaxStamina;
        public short Mana;
        public short MaxMana;
        public bool Renamable;
        public byte Direction;
        public byte Notoriety;
        public short Strenght;
        public short Dexterity;
        public short Intelligence;
        public int Gold;
        public ushort Armor;
        public ushort Weight;
        public byte Status;
        public bool WarMode;

        /// <summary>
        /// I know it is not transparent, but it is much faster to access layers directly.
        /// </summary>
        private uint[] layers;

        public RealCharacter(uint serial)
            : base(serial)
        {
            Hits = -1;
            MaxHits = -1;
            Stamina = -1;
            MaxStamina = -1;
            Mana = -1;
            MaxMana = -1;
            Renamable = false;
            Direction = 0;
            Notoriety = 0;
            Strenght = -1;
            Dexterity = -1;
            Intelligence = -1;
            Gold = -1;
            Armor = 0;
            Weight = 0;

            layers = new uint[256];
        }

        public uint[] Layers
        {
            get { return layers; }
        }

        public override string Description
        {
            get
            {
                return String.Format(base.Description + "  Model: 0x{0:X4}  Renamable: {1}  Notoriety: {2}  HP: {3}/{4}", Graphic, Renamable, (Notoriety)Notoriety, Hits, MaxHits);
            }
        }
    }
}
