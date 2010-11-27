using System;

namespace Phoenix
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class MessageHandlerAttributeBase : RuntimeAttribute
    {
        private byte id;
        private CallbackPriority priority;

        public MessageHandlerAttributeBase(byte id)
        {
            this.id = id;
            priority = CallbackPriority.Normal;
        }

        public byte Id
        {
            get { return id; }
        }

        public CallbackPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }
    }
}
