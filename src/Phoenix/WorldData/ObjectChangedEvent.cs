using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    public enum ObjectChangeType
    {
        Unknown = 0,
        CharUpdated = 1,
        ItemUpdated = 2,
        Removed = 4,
        ItemOpened = 8,
        /// <summary>
        /// Needs a lot of performance testing.
        /// </summary>
        SubItemUpdated = 16,
        /// <summary>
        /// Needs a lot of performance testing.
        /// </summary>
        SubItemRemoved = 32,
        NewItem = 64
    }

    public class ObjectChangedEventArgs : EventArgs
    {
        private Serial serial;
        private Serial itemSerial;
        private ObjectChangeType type;
        private bool isStatusUpdate;

        public ObjectChangedEventArgs(Serial serial, ObjectChangeType type)
        {
            this.serial = serial;
            this.type = type;
            this.itemSerial = Serial.Invalid;
        }

        public ObjectChangedEventArgs(Serial serial, ObjectChangeType type, bool isStatusUpdate)
        {
            this.serial = serial;
            this.type = type;
            this.itemSerial = Serial.Invalid;
            this.isStatusUpdate = isStatusUpdate;
        }

        public ObjectChangedEventArgs(Serial serial, Serial itemSerial, ObjectChangeType type)
        {
            this.serial = serial;
            this.type = type;
            this.itemSerial = itemSerial;
        }

        /// <summary>
        /// Serial of "caller".
        /// </summary>
        public Serial Serial
        {
            get { return serial; }
        }

        /// <summary>
        /// Serial of changed sub item or Serial.Invalid.
        /// </summary>
        public Serial ItemSerial
        {
            get { return itemSerial; }
        }

        public ObjectChangeType Type
        {
            get { return type; }
        }

        public bool IsStatusUpdate
        {
            get { return isStatusUpdate; }
        }
    }

    public delegate void ObjectChangedEventHandler(object sender, ObjectChangedEventArgs e);

    internal class ObjectChangedPublicEvent : PublicEvent<ObjectChangedEventHandler, ObjectChangedEventArgs>
    {
    }
}
