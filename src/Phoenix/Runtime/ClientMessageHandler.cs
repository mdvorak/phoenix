using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.Communication.Packets;

namespace Phoenix.Runtime
{
    internal static class ClientMessageHandler
    {
        public static void Init()
        {
            Core.RegisterClientMessageCallback(AsciiSpeechRequest.PacketId, new MessageCallback(OnAsciiSpeechRequest), CallbackPriority.Highest);
            Core.RegisterClientMessageCallback(UnicodeSpeechRequest.PacketId, new MessageCallback(OnUnicodeSpeechRequest), CallbackPriority.Highest);
        }

        public static void ProcessCommand(string text)
        {
            try
            {
                TextCommand cmd = TextCommand.Parse(text);
                RuntimeCore.Executions.Execute(RuntimeCore.CommandList[cmd.Command], cmd.Arguments);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Runtime");
                UO.PrintError("Error: " + e.Message);
            }
        }

        private static CallbackResult OnAsciiSpeechRequest(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeechRequest packet = new AsciiSpeechRequest(data);

            if (!Core.LoggedIn || packet.Text.Length == 0)
                return CallbackResult.Eat;

            if (packet.Text[0] == ',')
            {
                ProcessCommand(packet.Text);
                return CallbackResult.Eat;
            }
            else
            {
                return CallbackResult.Normal;
            }
        }

        private static CallbackResult OnUnicodeSpeechRequest(byte[] data, CallbackResult prevResult)
        {
            UnicodeSpeechRequest packet = new UnicodeSpeechRequest(data);

            if (!Core.LoggedIn || packet.Text.Length == 0)
                return CallbackResult.Eat;

            if (packet.Text[0] == ',')
            {
                ProcessCommand(packet.Text);
                return CallbackResult.Eat;
            }
            else
            {
                return CallbackResult.Normal;
            }
        }
    }
}
