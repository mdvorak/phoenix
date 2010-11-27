using System;
using System.Reflection;

namespace Phoenix
{
    public class ClientMessageHandlerAttribute : MessageHandlerAttributeBase
    {
        public ClientMessageHandlerAttribute(byte id)
            : base(id)
        {
        }

        protected override string Register(MemberInfo mi, object target)
        {
            Delegate d = Delegate.CreateDelegate(typeof(MessageCallback), target, (MethodInfo)mi, false);

            if (d == null)
                throw new Exception("Attribute used on incorrect method. Method must corespond to MessageCallback delegate.");

            Core.RegisterClientMessageCallback(Id, (MessageCallback)d, Priority);
            return String.Format("Client message 0x{0:X2} handler successfully registered.", Id);
        }
    }
}
