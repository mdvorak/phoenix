using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.Communication;

namespace Phoenix.WorldData
{
    #region SkillValue class

    public enum SkillLock : byte
    {
        Gain = 0x00,
        Lose = 0x01,
        Lock = 0x02
    }

    public struct SkillValue
    {
        public ushort ID;
        public ushort Value;
        public ushort RealValue;
        public SkillLock Lock;

        /// <summary>
        /// If 0xFFFF skill max value isn't supplied.
        /// </summary>
        public ushort MaxValue;

        public SkillValue(ushort id, ushort value, ushort realValue, SkillLock lockStatus, ushort maxValue)
        {
            ID = id;
            Value = value;
            RealValue = realValue;
            Lock = lockStatus;
            MaxValue = maxValue;
        }

        public override int GetHashCode()
        {
            return ID ^ Value ^ RealValue ^ MaxValue ^ (int)Lock;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == GetType())
                return Equals((SkillValue)obj);
            else
                return false;
        }

        public bool Equals(SkillValue value)
        {
            return value.ID == ID && value.Value == Value && value.RealValue == RealValue && value.Lock == Lock && value.MaxValue == MaxValue;
        }

        public override string ToString()
        {
            return String.Format("ID={0} Value={1} RealValue={2} Lock={3} MaxValue={4}", ID, Value, RealValue, Lock, MaxValue);
        }
    }

    #endregion

    #region SkillChanged event

    public class SkillChangedEventArgs : EventArgs
    {
        private SkillValue value;
        private SkillValue oldValue;

        public SkillChangedEventArgs(SkillValue value, SkillValue oldValue)
        {
            this.value = value;
            this.oldValue = oldValue;
        }

        public SkillValue Value
        {
            get { return value; }
        }

        public SkillValue OldValue
        {
            get { return oldValue; }
        }
    }

    public delegate void SkillChangedEventHandler(object sender, SkillChangedEventArgs e);

    #endregion

    public class PlayerSkills
    {
        #region Handler

        class SkillChangedPublicEvent : PublicEvent<SkillChangedEventHandler, SkillChangedEventArgs>
        {
        }

        private static readonly object syncRoot = new object();
        private static Dictionary<ushort, SkillValue> skillList = new Dictionary<ushort, SkillValue>();
        private static SkillChangedPublicEvent skillChanged = new SkillChangedPublicEvent();
        private static DefaultPublicEvent skillsCleared = new DefaultPublicEvent();

        /// <summary>
        /// Called only in Core.Initialize().
        /// </summary>
        internal static void Init()
        {
            Core.RegisterServerMessageCallback(0x3A, new MessageCallback(OnSkillsUpdate));

            Core.LoginComplete += new EventHandler(Core_LoginComplete);
            Core.Disconnected += new EventHandler(Core_Disconnected);
        }

        static void Core_LoginComplete(object sender, EventArgs e)
        {

        }

        static void Core_Disconnected(object sender, EventArgs e)
        {
            lock (syncRoot) {
                skillList.Clear();
                OnSkillsCleared(EventArgs.Empty);
            }
        }

        static CallbackResult OnSkillsUpdate(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                PacketReader reader = new PacketReader(data);
                byte id = reader.ReadByte();
                if (id != 0x3A) throw new Exception("Invalid packet passed to OnSkillsUpdate.");

                ushort packetLenght = reader.ReadUInt16();
                byte type = reader.ReadByte();

                bool skillcup = (type == 0x02) || (type == 0x03) || (type == 0xDF);
                bool loop = (type != 0xDF && type != 0xFF);
                int @base = (loop ? 1 : 0); // For some strange reason, list has base 1 for skillid, and update 0.

                ushort lastID;

                // Loop only one time if Type is 0xFF, otherwise loop until SkillID is zero.
                while (reader.Offset < reader.Length && (lastID = reader.ReadUInt16()) > 0) {
                    SkillValue value = new SkillValue();
                    value.ID = (ushort)(lastID - @base); 
                    value.Value = reader.ReadUInt16();
                    value.RealValue = reader.ReadUInt16();
                    value.Lock = (SkillLock)reader.ReadByte();
                    value.MaxValue = skillcup ? reader.ReadUInt16() : (ushort)0xFFFF; // TODO

                    Trace.WriteLine(String.Format("Received skill {0} update to {1}", value.ID, value.RealValue), "World");
                    OnSkillChanged(value);

                    // Stop iteration
                    if (!loop)
                        break;
                }

                return CallbackResult.Normal;
            }
        }

        private static void OnSkillChanged(SkillValue value)
        {
            try {
                SkillValue oldValue;
                skillList.TryGetValue(value.ID, out oldValue);

                if (!value.Equals(oldValue)) {
                    Trace.WriteLine("New value for skill", "World");
                    skillList[value.ID] = value;

                    skillChanged.Invoke(null, new SkillChangedEventArgs(value, oldValue));
                }
                else
                    Trace.WriteLine("No new value for skill", "World");
            }
            catch (Exception e) {
                Trace.WriteLine("Unhandled exception in PlayerSkills.SkillChanged event. Exception:\n" + e.ToString(), "World");
            }
        }

        private static void OnSkillsCleared(EventArgs e)
        {
            try {
                skillsCleared.Invoke(null, e);
            }
            catch (Exception ex) {
                Trace.WriteLine("Unhandled exception in PlayerSkills.SkillsCleared event. Exception:\n" + ex.ToString(), "World");
            }
        }

        #endregion

        #region Public properties and methods

        public PlayerSkills()
        {

        }

        public SkillValue this[ushort id]
        {
            get
            {
                SkillValue value;
                PlayerSkills.skillList.TryGetValue(id, out value);
                return value;
            }
        }

        public SkillValue this[string name]
        {
            get
            {
                name = name.ToLowerInvariant();

                for (int i = 0; i < DataFiles.Skills.Count; i++) {
                    if (DataFiles.Skills[i].Name.ToLowerInvariant().Contains(name)) {
                        SkillValue value;
                        PlayerSkills.skillList.TryGetValue((ushort)i, out value);
                        return value;
                    }
                }

                return new SkillValue();
            }
        }

        public static event SkillChangedEventHandler SkillChanged
        {
            add { PlayerSkills.skillChanged.AddHandler(value); }
            remove { PlayerSkills.skillChanged.RemoveHandler(value); }
        }

        public static event EventHandler SkillsCleared
        {
            add { PlayerSkills.skillsCleared.AddHandler(value); }
            remove { PlayerSkills.skillsCleared.RemoveHandler(value); }
        }

        #endregion
    }
}
