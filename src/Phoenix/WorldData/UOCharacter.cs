using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Phoenix.Communication;

namespace Phoenix.WorldData
{
    public class UOCharacter : UOObject
    {
        private LayersCollection collection;

        public UOCharacter(uint serial)
            : base(serial)
        {
            // TODO
            collection = new LayersCollection(serial, true);
        }

        public Graphic Model
        {
            get { return World.GetRealCharacter(Serial).Graphic; }
        }

        public short Hits
        {
            get { return World.GetRealCharacter(Serial).Hits; }
        }

        public short MaxHits
        {
            get { return World.GetRealCharacter(Serial).MaxHits; }
        }

        public short Stamina
        {
            get { return World.GetRealCharacter(Serial).Stamina; }
        }

        public short MaxStamina
        {
            get { return World.GetRealCharacter(Serial).MaxStamina; }
        }

        public short Mana
        {
            get { return World.GetRealCharacter(Serial).Mana; }
        }

        public short MaxMana
        {
            get { return World.GetRealCharacter(Serial).MaxMana; }
        }

        public bool Renamable
        {
            get { return World.GetRealCharacter(Serial).Renamable; }
        }

        public byte Direction
        {
            get { return (byte)(World.GetRealCharacter(Serial).Direction & 0x0F); }
        }

        public bool Running
        {
            get { return (World.GetRealCharacter(Serial).Direction & 0xC0) != 0; }
        }

        public Notoriety Notoriety
        {
            get { return (Notoriety)World.GetRealCharacter(Serial).Notoriety; }
        }

        private bool GetFlag(byte flag)
        {
            return (World.GetRealCharacter(Serial).Flags & flag) != 0;
        }

        public bool Invulnerable
        {
            get { return GetFlag(0x01); }
        }

        /// <summary>
        /// Gets wheter character is dead or not.
        /// </summary>
        public bool Dead
        {
            get { return GetFlag(0x02); }
        }

        public bool Poisoned
        {
            get { return GetFlag(0x04); }
        }

        public bool CannotMove
        {
            get { return GetFlag(0x10); }
        }

        public bool CannotTalk
        {
            get { return GetFlag(0x20); }
        }

        /// <summary>
        /// Gets character warmode.
        /// </summary>
        public virtual bool Warmode
        {
            get { return GetFlag(0x40); }
        }

        public bool Hidden
        {
            get { return GetFlag(0x80); }
        }

        public LayersCollection Layers
        {
            get { return collection; }
        }

        static readonly Regex renameRegex = new Regex(@"\A(?:[a-z0-9])+\z", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public bool Rename(string newName)
        {
            if (newName == null || newName.Length == 0) return false;
            newName = newName.Replace(" ", "");
            if (newName.Length > 29) newName = newName.Remove(29);

            if (!renameRegex.IsMatch(newName)) {
                UO.PrintError("Creature name (\"{0}\") contains some invalid characters.", newName);
                return false;
            }

            if (Exist && Renamable) {
                byte[] data = new byte[35];
                data[0] = 0x75;
                ByteConverter.BigEndian.ToBytes(Serial, data, 1);
                ByteConverter.BigEndian.ToBytesAscii(newName, data, 5);
                Core.SendToServer(data);
                return true;
            }

            return false;
        }

        public void Tell(string text)
        {
            if (Name != null && Name.Length > 0) {
                UO.Say(Name + " " + text);
            }
            else {
                UO.Print("You don't know the name of 0x{0:X8}.", Serial);
            }
        }

        public void Tell(string format, params object[] args)
        {
            Tell(String.Format(format, args));
        }

        public void Print(object o)
        {
            if (o != null)
                UO.PrintObject(Serial, o.ToString());
        }

        public void Print(string format, params object[] args)
        {
            UO.PrintObject(Serial, format, args);
        }

        public void Print(ushort color, object o)
        {
            if (o != null)
                UO.PrintObject(Serial, color, o.ToString());
        }

        public void Print(ushort color, string format, params object[] args)
        {
            UO.PrintObject(Serial, color, format, args);
        }

        /// <summary>
        /// Requests status of character from server. Exits immediatly.
        /// </summary>
        public void RequestStatus()
        {
            if (Exist) {
                Core.SendToServer(PacketBuilder.CharacterStatsRequest(Serial, PacketBuilder.StatsRequestType.Stats));
            }
        }

        /// <summary>
        /// Requests status of character from server. Exits after status has been received or timeout occured.
        /// </summary>
        /// <param name="timeout">Timeout period.</param>
        /// <returns>True when status update received; otherwise false.</returns>
        public bool RequestStatus(int timeout)
        {
            if (Exist) {
                // TODO: Could receive i.e. HitsUpdate packet before 0x11
                using (SpecializedObjectChangedEventWaiter ew = new SpecializedObjectChangedEventWaiter(Serial, ObjectChangeType.CharUpdated, StatusUpdateFilter)) {
                    Core.SendToServer(PacketBuilder.CharacterStatsRequest(Serial, PacketBuilder.StatsRequestType.Stats));
                    return ew.Wait(timeout);
                }
            }
            else return false;
        }

        private static bool StatusUpdateFilter(object eventSender, ObjectChangedEventArgs e)
        {
            return e.IsStatusUpdate;
        }
    }
}
