using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Communication;

namespace Phoenix.WorldData
{
    public class UOObject
    {
        private Serial serial;

        public UOObject(Serial serial)
        {
            // TODO: vymyslet co s 0x7FFFF..
            this.serial = serial;
        }

        public Serial Serial
        {
            get { return serial; }
        }

        // TODO: virtual, na zrychleni, override v UOItem atd
        public bool Exist
        {
            get { return World.Exists(serial); }
        }

        public string Name
        {
            get { return World.GetRealObject(serial).Name; }
        }

        public UOColor Color
        {
            get { return World.GetRealObject(serial).Color; }
        }

        public ushort X
        {
            get { return World.GetRealObject(serial).X; }
        }

        public ushort Y
        {
            get { return World.GetRealObject(serial).Y; }
        }

        public sbyte Z
        {
            get { return World.GetRealObject(serial).Z; }
        }

        public byte Flags
        {
            get { return World.GetRealObject(serial).Flags; }
        }

        public string Description
        {
            get
            {
                RealObject obj = World.FindRealObject(serial);
                if (obj != null) return obj.Description;
                else return String.Format("Object {0} is not known.", serial);
            }
        }

        public int Distance
        {
            get
            {
                RealObject obj = World.FindRealObject(Serial);

                while ((obj as RealItem) != null && ((RealItem)obj).Container != 0)
                {
                    obj = World.FindRealObject(((RealItem)obj).Container);
                }

                if (obj != null && World.RealPlayer != null)
                {
                    return obj.GetDistance(World.RealPlayer);
                }
                // It is better to return MaxValue than -1, because it is more easier to use it in scripts.
                else return int.MaxValue;
            }
        }

        public event ObjectChangedEventHandler Changed
        {
            add { World.AddObjectChangedCallback(serial, value); }
            remove { World.RemoveObjectChangedCallback(serial, value); }
        }

        public void Click()
        {
            if (Exist)
            {
                Core.SendToServer(PacketBuilder.ObjectClick(serial));
            }
        }

        public void Use()
        {
            if (Exist)
            {
                Core.SendToServer(PacketBuilder.ObjectDoubleClick(serial));
                Aliases.LastObject = serial;
            }
        }

        public void WaitTarget()
        {
            if (Exist)
            {
                UIManager.WaitForTarget(serial, 0, 0, 0, 0);
            }
        }

        public override string ToString()
        {
            string name = Name;
            if (name != null && name.Length > 0)
                return name;
            else
                return serial.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return serial == ((UOObject)obj).serial;
        }

        public virtual bool Equals(UOObject obj)
        {
            if (((object)obj) == null)
                return false;
            return serial == obj.serial;
        }

        public override int GetHashCode()
        {
            return serial.GetHashCode();
        }

        public static implicit operator UInt32(UOObject obj)
        {
            if (obj == null) return Serial.Invalid;
            return obj.serial;
        }

        public static implicit operator Serial(UOObject obj)
        {
            if (obj == null) return Serial.Invalid;
            return obj.serial;
        }

        public static bool operator ==(UOObject obj1, UOObject obj2)
        {
            if (((object)obj1) == null)
                return ((object)obj2) == null;
            return obj1.Equals(obj2);
        }

        public static bool operator !=(UOObject obj1, UOObject obj2)
        {
            if (((object)obj1) == null)
                return ((object)obj2) != null;
            return !obj1.Equals(obj2);
        }
    }
}
