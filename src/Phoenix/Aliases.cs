using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Phoenix.Configuration;
using Phoenix.Runtime;
using System.Runtime.CompilerServices;

namespace Phoenix
{
    public static class Aliases
    {
        private static readonly Dictionary<string, Serial> objects = new Dictionary<string, Serial>();

        private static Serial lastObject = Serial.Invalid;
        private static Serial lastTarget = Serial.Invalid;
        private static Serial lastAttack = Serial.Invalid;
        private static Serial recevingContainer = Serial.Invalid;

        private static readonly DefaultPublicEvent changed = new DefaultPublicEvent();

        public static event EventHandler Changed
        {
            add { changed.AddHandler(value); }
            remove { changed.RemoveHandler(value); }
        }

        /// <summary>
        /// Called by Phoenix.Initialize()
        /// </summary>
        internal static void Init()
        {
            Core.RegisterClientMessageCallback(0x06, new MessageCallback(OnObjectDoubleClick));
            Core.RegisterClientMessageCallback(0x12, new MessageCallback(OnUse));
            Core.RegisterClientMessageCallback(0xBF, new MessageCallback(OnGeneralCommand));
            Core.RegisterClientMessageCallback(0x05, new MessageCallback(OnAttack));
            Core.RegisterClientMessageCallback(0x6C, new MessageCallback(OnClientTarget));

            Core.Disconnected += new EventHandler(Core_Disconnected);
        }

        static void Core_Disconnected(object sender, EventArgs e)
        {
            objects.Clear();
        }

        public static Serial Self
        {
            get { return WorldData.World.PlayerSerial; }
        }

        public static Serial Backpack
        {
            get
            {
                uint serial = WorldData.World.RealPlayer.Layers[0x15];

                if (serial == 0)
                    return Serial.Invalid;
                else
                    return serial;
            }
        }

        public static Serial LastObject
        {
            get { return Aliases.lastObject; }
            set
            {
                Serial old = Aliases.lastObject;
                Aliases.lastObject = value;
                OnChanged(new AliasChangedEventArgs("lastobject", value, old));
            }
        }

        public static Serial LastTarget
        {
            get { return Aliases.lastTarget; }
            set
            {
                Serial old = Aliases.lastTarget;
                Aliases.lastTarget = value;
                OnChanged(new AliasChangedEventArgs("lastobject", value, old));
            }
        }

        public static Serial LastAttack
        {
            get { return Aliases.lastAttack; }
            set
            {
                Serial old = Aliases.lastAttack;
                Aliases.lastAttack = value;
                OnChanged(new AliasChangedEventArgs("lastobject", value, old));
            }
        }

        public static Serial RecevingContainer
        {
            get { return Aliases.recevingContainer; }
            set
            {
                Serial old = Aliases.recevingContainer;
                Aliases.recevingContainer = value;
                OnChanged(new AliasChangedEventArgs("lastobject", value, old));
            }
        }

        #region Client messages handling

        private static CallbackResult OnObjectDoubleClick(byte[] data, CallbackResult prevResult)
        {
            Aliases.lastObject = ByteConverter.BigEndian.ToUInt32(data, 1);
            Trace.WriteLine("LastObject updated.", "Aliases");
            return CallbackResult.Normal;
        }

        private static CallbackResult OnUse(byte[] data, CallbackResult prevResult)
        {
            return CallbackResult.Normal;
        }

        private static CallbackResult OnGeneralCommand(byte[] data, CallbackResult prevResult)
        {
            return CallbackResult.Normal;
        }

        private static CallbackResult OnAttack(byte[] data, CallbackResult prevResult)
        {
            Aliases.lastAttack = ByteConverter.BigEndian.ToUInt32(data, 1);
            Trace.WriteLine("LastAttack updated.", "Aliases");
            return CallbackResult.Normal;
        }

        private static CallbackResult OnClientTarget(byte[] data, CallbackResult prevResult)
        {
            if (prevResult == CallbackResult.Normal) {
                Aliases.lastTarget = ByteConverter.BigEndian.ToUInt32(data, 7);
                Trace.WriteLine("LastTarget updated.", "Aliases");
            }
            return CallbackResult.Normal;
        }

        #endregion

        public static Serial GetObject(string name)
        {
            if (!Helper.CheckName(ref name, false))
                return Serial.Invalid;

            if (objects.ContainsKey(name))
                return objects[name];

            PropertyInfo property = typeof(Aliases).GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
            if (property != null) {
                return (Serial)property.GetValue(null, new object[0]);
            }
            else {
                return Serial.Invalid;
            }
        }

        public static bool ObjectExists(string name)
        {
            if (!Helper.CheckName(ref name, false))
                return false;

            if (objects.ContainsKey(name))
                return true;

            PropertyInfo property = typeof(Aliases).GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
            if (property != null) {
                object o = property.GetValue(null, new object[0]);
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// This function ins provided only for compatilibity with Injection.
        /// Objects are deleted on logout.
        /// </summary>
        /// <param name="name">Can contain only letters. It is not case sensitive.</param>
        public static void SetObject(string name, Serial value)
        {
            Helper.CheckName(ref name, true);

            Serial oldValue;
            PropertyInfo property = typeof(Aliases).GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);

            if (property != null) {
                oldValue = (Serial)property.GetValue(null, null);
                property.SetValue(null, value, new object[0]);
            }
            else {
                objects.TryGetValue(name, out oldValue);
                objects[name] = value;
            }

            Trace.WriteLine("Set alias " + name + " to " + value, "Aliases");
            OnChanged(new AliasChangedEventArgs(name, value, oldValue));
        }

        /// <summary>
        /// This function ins provided only for compatilibity with Injection.
        /// Objects are deleted on logout.
        /// </summary>
        /// <param name="name">Can contain only letters. It is not case sensitive.</param>
        public static void DeleteObject(string name)
        {
            if (!Helper.CheckName(ref name, false))
                return;

            Serial oldValue;
            PropertyInfo property = typeof(Aliases).GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);

            if (property != null) {
                oldValue = (Serial)property.GetValue(null, null);
                property.SetValue(null, Serial.Invalid, new object[0]);
            }
            else {
                if (!objects.TryGetValue(name, out oldValue))
                    return;
                objects.Remove(name);
            }

            Trace.WriteLine("Removed alias " + name, "Aliases");
            OnChanged(new AliasChangedEventArgs(name, Serial.Invalid, oldValue));
        }

        private static void OnChanged(AliasChangedEventArgs e)
        {
            changed.Invoke(null, e);
        }
    }
}
