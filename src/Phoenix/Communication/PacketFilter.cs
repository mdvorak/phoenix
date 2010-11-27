using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.WorldData;
using Phoenix.Collections;

namespace Phoenix.Communication
{
    /// <summary>
    /// Checks Phoenix->Server and Phoenix->Client messages.
    /// </summary>
    static class PacketFilter
    {
        private static readonly ActionSpamLimiter speechLimiter = new ActionSpamLimiter(700, 20, 16);
        private static readonly ActionSpamLimiter actionLimiter = new ActionSpamLimiter(450, 8, 5);
        private static readonly ActionSpamLimiter attackLimiter = new ActionSpamLimiter(500, 7, 4);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Return true to eat the packet.</returns>
        public static bool OnClientMessage(byte[] data)
        {
            if (data.Length == 0 || data.Length > 512) {
                UO.PrintWarning("Outgoing packet 0x{0} dropped due to its lenght ({1} B).", data[0].ToString("X2"), data.Length);
                return true;
            }

            switch (data[0]) {
                case 0x03:
                    if (!speechLimiter.Hit(1)) {
                        UO.PrintError("Speech spam detected, message dropped.");
                        return true;
                    }

                    if (speechLimiter.IsCritical)
                        UO.PrintWarning("Warning: Speech spam detected.");

                    return OnAsciiSpeechRequest(data);

                case 0xAD:
                    if (!speechLimiter.Hit(1)) {
                        UO.PrintError("Speech spam detected, message dropped.");
                        return true;
                    }

                    if (speechLimiter.IsCritical)
                        UO.PrintWarning("Warning: Speech spam detected.");

                    return OnUnicodeSpeechRequest(data);

                case 0x08:
                    return OnItemDropRequest(data);

                case 0x75:
                    return OnRenameRequest(data);

                case 0x3B:
                    return OnBuy(data);

                case 0x06:
                case 0x12:
                case 0xBF:
                    if (!actionLimiter.Hit(1)) {
                        Trace.WriteLine("Action spam detected, message 0x" + data[0].ToString("X") + " dropped.");
                        UO.PrintError("Action spam detected, message dropped.");
                        return true;
                    }

                    if (actionLimiter.IsCritical)
                        UO.PrintWarning("Warning: Action spam detected.");

                    return false;

                case 0x05:
                    if (!attackLimiter.Hit(1)) {
                        UO.PrintError("Attack spam detected, message dropped.");
                        return true;
                    }

                    if (attackLimiter.IsCritical)
                        UO.PrintWarning("Warning: Attack spam detected.");

                    return false;
            }

            return false;
        }

        private static QueueEx<byte> walkQueue = new QueueEx<byte>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Return true to eat the packet.</returns>
        public static bool OnServerMessage(byte[] data)
        {
            switch (data[0]) {
                case 0x1C:
                case 0xAE:
                    return OnSpeech(data);

                case 0x02:
                    // Move
                    return false;
            }

            return false;
        }

        private static bool OnSpeech(byte[] data)
        {
            ushort color = ByteConverter.BigEndian.ToUInt16(data, 10);

            if (color < DataFiles.Hues.MinIndex || color > DataFiles.Hues.MaxIndex) {
                Trace.WriteLine("WARNING: Invalid color received.", "Communication");
                ByteConverter.BigEndian.ToBytes(Env.DefaultFontColor, data, 10);
            }

            return false;
        }

        private static bool OnAsciiSpeechRequest(byte[] data)
        {
            if (data.Length - 8 > Core.MaxSpeechLenght) {
                UO.PrintWarning("Outgoing packet 0x{0} dropped. Too long ({1} B) speech packet.", data[0].ToString("X2"), data.Length);
                return true;
            }

            ushort color = ByteConverter.BigEndian.ToUInt16(data, 4);
            if (color < DataFiles.Hues.MinIndex || color > DataFiles.Hues.MaxIndex) {
                UO.PrintWarning("Invalid speech color. Please change it.");
                ByteConverter.BigEndian.ToBytes(Env.DefaultFontColor, data, 4);
            }

            for (int i = 8; i < data.Length; i++) {
                if (data[i] < 32)
                    data[i] = 32;
            }

            return false;
        }

        private static bool OnUnicodeSpeechRequest(byte[] data)
        {
            if ((data.Length - 12) / 2 > Core.MaxSpeechLenght) {
                UO.PrintWarning("Outgoing packet 0x{0} dropped. Too long ({1} B) speech packet.", data[0].ToString("X2"), data.Length);
                return true;
            }

            ushort color = ByteConverter.BigEndian.ToUInt16(data, 4);
            if (color < DataFiles.Hues.MinIndex || color > DataFiles.Hues.MaxIndex) {
                UO.PrintWarning("Invalid speech color. Please change it.");
                ByteConverter.BigEndian.ToBytes(Env.DefaultFontColor, data, 4);
            }

            return false;
        }

        private static bool OnItemDropRequest(byte[] data)
        {
            uint itemSerial = ByteConverter.BigEndian.ToUInt32(data, 1);
            uint containerSerial = ByteConverter.BigEndian.ToUInt32(data, 10);

            RealItem item = World.FindRealItem(containerSerial);
            while (item != null) {
                if (item.Serial == itemSerial) {
                    ByteConverter.BigEndian.ToBytes(World.RealPlayer.Layers[0x15], data, 10); // Backpack
                    UO.PrintWarning("Phoenix saved you from container loss.");
                    return false;
                }

                item = World.FindRealItem(item.Container);
            }

            return false;
        }

        private static bool OnRenameRequest(byte[] data)
        {
            uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

            if (serial == World.PlayerSerial) {
                UO.PrintError("You cannot rename yourself!");
                //return true;
            }

            for (int i = 6; i < data.Length; i++) {
                if (data[i] != 0 && data[i] < 32) {
                    data[i] = 0;
                }
            }

            return false;
        }

        private static bool OnBuy(byte[] data)
        {
            PacketReader reader = new PacketReader(data);
            reader.Skip(7);

            byte flag = reader.ReadByte();

            if (flag == 0x02) {
                if ((reader.Length - 8) % 7 != 0) {
                    UO.PrintError("Dropped corrupted Buy packet.");
                    return true;
                }

                while (reader.Offset < reader.Length) {
                    if (reader.ReadByte() != 0x1A) {
                        UO.PrintError("Dropped corrupted Buy packet.");
                        return true;
                    }

                    // Serial
                    reader.Skip(4);
                    ushort amount = reader.ReadUInt16();

                    if (amount == 0) {
                        amount = 1;
                        ByteConverter.BigEndian.ToBytes(amount, data, reader.Offset - 2);
                    }
                }
            }
            else {
                UO.PrintError("Invalid flag in client Buy packet. Packet dropped.");
                return true;
            }

            return false;
        }
    }
}
