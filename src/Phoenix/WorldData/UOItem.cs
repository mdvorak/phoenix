using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    public class UOItem : UOObject
    {
        private ItemsCollection items;
        private ItemsCollection allItems;

        public UOItem(uint serial)
            : base(serial)
        {
            items = new ItemsCollection(serial, false);
            allItems = new ItemsCollection(serial, true);
        }

        public Graphic Graphic
        {
            get { return World.GetRealItem(Serial).Graphic; }
        }

        public ushort Amount
        {
            get { return World.GetRealItem(Serial).Amount; }
        }

        public Layer Layer
        {
            get { return (Layer)World.GetRealItem(Serial).Layer; }
        }

        public Serial Container
        {
            get { return World.GetRealItem(Serial).Container; }
        }

        public bool Opened
        {
            get { return World.GetRealItem(Serial).Opened; }
        }

        public ItemsCollection Items
        {
            get { return items; }
        }

        public ItemsCollection AllItems
        {
            get { return allItems; }
        }

        public bool Move(ushort amount, ushort x, ushort y, sbyte z)
        {
            if (Exist)
                return UIManager.MoveItem(Serial, amount, x, y, z, 0);
            else
                return false;
        }

        public bool Move(ushort amount, Serial container)
        {
            return Move(amount, container, 0xFFFF, 0xFFFF);
        }

        public bool Move(ushort amount, Serial container, ushort x, ushort y)
        {
            if (Exist)
                return UIManager.MoveItem(Serial, amount, x, y, 0, container);
            else
                return false;
        }

        public bool Grab()
        {
            return Grab(0);
        }

        public bool Grab(ushort amount)
        {
            if (Aliases.RecevingContainer.IsValid && Exist)
                return UIManager.MoveItem(Serial, amount, 0xFFFF, 0xFFFF, 0, Aliases.RecevingContainer);
            else
                return false;
        }

        public bool Drop(ushort amount, int relativeX, int relativeY, int relativeZ)
        {
            if (Exist)
                return Move(amount, (ushort)(World.Player.X + relativeX), (ushort)(World.Player.Y + relativeY), (sbyte)(World.Player.Z + relativeZ));
            else
                return false;
        }

        public bool DropHere()
        {
            return DropHere(0);
        }

        public bool DropHere(ushort amount)
        {
            if (Exist)
                return Move(amount, World.Player.X, World.Player.Y, World.Player.Z);
            else
                return false;
        }

        public bool Equip()
        {
            if (Exist)
                return UO.EquipItem(Serial);
            else
                return false;
        }
    }
}
