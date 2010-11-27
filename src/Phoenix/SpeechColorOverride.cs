using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    static class SpeechColorOverride
    {
        /// <summary>
        /// Called by Phoenix.Intialize()
        /// </summary>
        public static void Init()
        {
            Core.RegisterClientMessageCallback(0x03, new MessageCallback(OnSpeechRequest), CallbackPriority.Low);
            Core.RegisterClientMessageCallback(0xAD, new MessageCallback(OnSpeechRequest), CallbackPriority.Low);
        }

        static CallbackResult OnSpeechRequest(byte[] data, CallbackResult prevResult)
        {
            if (prevResult == CallbackResult.Normal && Config.Profile.OverrideSpeechColor)
            {
                byte[] newData = (byte[])data.Clone();
                ByteConverter.BigEndian.ToBytes(Config.Profile.Colors.FontColor.Value, newData, 4);
                Core.SendToServer(newData);
                return CallbackResult.Sent;
            }
            else
            {
                return CallbackResult.Normal;
            }
        }
    }
}
