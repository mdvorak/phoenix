using System;
using System.Reflection;

namespace Phoenix
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// This sample shows how to use ServerMessageHandlerAttribute.
    /// It is not required to be used on static methods.
    /// <code>
    /// public static class SomeClass
    /// {
    ///    [ServerMessageHandler(0x2C)]
    ///    public static CallbackResult OnDeath(byte[] data, CallbackResult prevResult)
    ///    {
    ///        UO.Print("You are DEAD!");
    ///        return CallbackResult.Normal;
    ///    }
    /// }
    /// </code>
    /// </example>
    public class ServerMessageHandlerAttribute : MessageHandlerAttributeBase
    {
        public ServerMessageHandlerAttribute(byte id)
            : base(id)
        {
        }

        protected override string Register(MemberInfo mi, object target)
        {
            Delegate d = Delegate.CreateDelegate(typeof(MessageCallback), target, (MethodInfo)mi, false);

            if (d == null)
                throw new Exception("Attribute used on incorrect method. Method must corespond to MessageCallback delegate.");

            Core.RegisterServerMessageCallback(Id, (MessageCallback)d, Priority);
            return String.Format("Server message 0x{0:X2} handler successfully registered.", Id);
        }
    }
}
