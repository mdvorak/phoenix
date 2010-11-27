using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.Configuration;

namespace Phoenix.Communication
{
    enum PacketDirection
    {
        ServerToPhoenix,
        PhoenixToClient,
        ClientToPhoenix,
        PhoenixToServer
    }

    static class PacketLogging
    {
        private static string BuildLine(byte[] data, int index, out int readBytes)
        {
            const int LineLenght = 25;

            StringBuilder hex = new StringBuilder();
            string chars = "";

            readBytes = 0;
            for (int i = 0; i < LineLenght; i++)
            {
                if (index + i < data.Length)
                {
                    hex.Append(data[index + i].ToString("X2") + " ");

                    if (Char.IsLetterOrDigit((Char)data[index + i]))
                        chars += (Char)data[index + i];
                    else
                        chars += ".";

                    readBytes++;
                }
                else
                {
                    hex.Append("   ");
                    chars += " ";
                }
            }

            return hex.ToString() + chars;
        }

        public static string BuildString(byte[] data)
        {
            string str = String.Format("Packet id: 0x{0}; {1} bytes:\n", data[0].ToString("X2"), data.Length);

            int index = 0;
            while (index < data.Length)
            {
                int readBytes;
                str += BuildLine(data, index, out readBytes) + "\n";
                index += readBytes;
            }

            return str;
        }

        public static void Write(PacketDirection dir, byte[] data)
        {
            if (Config.PacketLog.Enable)
            {
                string header = "";

                switch (dir)
                {
                    case PacketDirection.ServerToPhoenix:
                        if (!Config.PacketLog.ServerToPhoenix) return;
                        header = "Server->Phoenix";
                        break;

                    case PacketDirection.PhoenixToServer:
                        if (!Config.PacketLog.PhoenixToServer) return;
                        header = "Phoenix->Server";
                        break;

                    case PacketDirection.ClientToPhoenix:
                        if (!Config.PacketLog.ClientToPhoenix) return;
                        header = "Client->Phoenix";
                        break;

                    case PacketDirection.PhoenixToClient:
                        if (!Config.PacketLog.PhoenixToClient) return;
                        header = "Phoenix->Client";
                        break;
                }

                string packet = BuildString(data).Trim().Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine);

                Trace.WriteLine(packet, header);
            }
        }
    }
}
