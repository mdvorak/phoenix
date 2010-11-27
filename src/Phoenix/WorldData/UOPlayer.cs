using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Communication;

namespace Phoenix.WorldData
{
    public class UOPlayer : UOCharacter
    {
        private PlayerSkills skills = new PlayerSkills();

        public UOPlayer(uint serial)
            : base(serial)
        {
        }

        public short Strenght
        {
            get { return World.GetRealCharacter(Serial).Strenght; }
        }

        public short Dexterity
        {
            get { return World.GetRealCharacter(Serial).Dexterity; }
        }

        public short Intelligence
        {
            get { return World.GetRealCharacter(Serial).Intelligence; }
        }

        public int Gold
        {
            get { return World.GetRealCharacter(Serial).Gold; }
        }

        public ushort Armor
        {
            get { return World.GetRealCharacter(Serial).Armor; }
        }

        public ushort Weight
        {
            get { return World.GetRealCharacter(Serial).Weight; }
        }

        /// <summary>
        /// Gets maximum weight that can player carry. But dont trust it :)
        /// </summary>
        public ushort MaxWeight
        {
            get
            {
                RealCharacter obj = World.GetRealCharacter(Serial);
                if (obj != null)
                    return (ushort)(((obj.Strenght * 7) / 2) + 65);
                else
                    return 0;
            }
        }

        public UOItem Backpack
        {
            get { return Layers[Layer.Backpack]; }
        }

        public PlayerSkills Skills
        {
            get { return skills; }
        }

        public void ChangeWarmode(WarmodeChange change)
        {
            if (change >= WarmodeChange.Switch)
                change = Warmode ? WarmodeChange.Peace : WarmodeChange.War;

            Core.SendToServer(PacketBuilder.Warmode((byte)change));
        }

        public override bool Warmode
        {
            get { return World.GetRealCharacter(Serial).WarMode; }
        }
    }
}
