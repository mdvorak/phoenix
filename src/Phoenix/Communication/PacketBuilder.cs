using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication
{
    public static class PacketBuilder
    {
        public static byte[] LoginRequestShardList(string name, string password)
        {
            byte[] packet = new byte[62];

            packet[0] = 0x80;
            ByteConverter.BigEndian.ToBytesAscii(name, packet, 1, 30);
            ByteConverter.BigEndian.ToBytesAscii(password, packet, 31, 30);
            packet[61] = 0xFF;

            return packet;
        }

        public static byte[] ServerLoginRequest(uint seed, string name, string password)
        {
            byte[] packet = new byte[65];

            packet[0] = 0x91;
            ByteConverter.LittleEndian.ToBytes(seed, packet, 1);
            ByteConverter.BigEndian.ToBytesAscii(name, packet, 5, 30);
            ByteConverter.BigEndian.ToBytesAscii(password, packet, 35, 30);

            return packet;
        }

        public static byte[] CharacterSpeechUnicode(uint serial, ushort model, string name, SpeechType type, SpeechFont font, ushort color, string text)
        {
            // For security reasons
            if (text.Length > Core.MaxSpeechLenght)
                text = text.Remove(Core.MaxSpeechLenght);

            PacketWriter writer = new PacketWriter(0xAE);

            writer.WriteBlockSize();
            writer.Write(serial);
            writer.Write(model);
            writer.Write((byte)type);
            writer.Write(color);
            writer.Write((ushort)font);
            writer.WriteAsciiString("CSY", 4);
            writer.WriteAsciiString(name, 30);
            writer.WriteUnicodeString(text);

            return writer.GetBytes();
        }

        public static byte[] CharacterSpeechAscii(uint serial, ushort model, string name, SpeechType type, SpeechFont font, ushort color, string text)
        {
            // For security reasons
            if (text.Length > Core.MaxSpeechLenght)
                text = text.Remove(Core.MaxSpeechLenght);

            PacketWriter writer = new PacketWriter(0x1C);

            writer.WriteBlockSize();
            writer.Write(serial);
            writer.Write(model);
            writer.Write((byte)type);
            writer.Write(color);
            writer.Write((ushort)font);
            writer.WriteAsciiString(name, 30);
            writer.WriteAsciiString(text);

            return writer.GetBytes();
        }

        public static byte[] SpeechRequestAscii(SpeechType type, SpeechFont font, ushort color, string text)
        {
            // For security reasons
            if (text.Length > Core.MaxSpeechLenght)
                text = text.Remove(Core.MaxSpeechLenght);

            PacketWriter writer = new PacketWriter(0x03);

            writer.WriteBlockSize();
            writer.Write((byte)type);
            writer.Write(color);
            writer.Write((ushort)font);
            writer.WriteAsciiString(text);

            return writer.GetBytes();
        }

        public static byte[] SpeechRequestUnicode(SpeechType type, SpeechFont font, ushort color, string text)
        {
            // For security reasons
            if (text.Length > Core.MaxSpeechLenght)
                text = text.Remove(Core.MaxSpeechLenght);

            PacketWriter writer = new PacketWriter(0xAD);

            writer.WriteBlockSize();
            writer.Write((byte)type);
            writer.Write(color);
            writer.Write((ushort)font);
            writer.WriteAsciiString("CSY", 4);
            writer.WriteUnicodeString(text);

            return writer.GetBytes();
        }

        public static byte[] WalkRequestSucceeded(byte sequence)
        {
            byte[] data = new byte[3];
            data[0] = 0x22;
            data[1] = sequence;
            return data;
        }

        public static byte[] WalkRequestFailed(byte sequence, ushort x, ushort y, byte z, byte direction)
        {
            byte[] data = new byte[8];
            data[0] = 0x21;
            data[1] = sequence;
            ByteConverter.BigEndian.ToBytes(x, data, 2);
            ByteConverter.BigEndian.ToBytes(y, data, 4);
            data[6] = direction;
            data[7] = (byte)z;
            return data;
        }

        public static byte[] ItemPickupRequest(uint serial, ushort amount)
        {
            byte[] data = new byte[7];
            data[0] = 0x07;
            ByteConverter.BigEndian.ToBytes(serial, data, 1);
            ByteConverter.BigEndian.ToBytes(amount, data, 5);
            return data;
        }

        public static byte[] ItemDropRequest(uint serial, ushort x, ushort y, sbyte z, uint container)
        {
            if (container == 0) container = uint.MaxValue;

            byte[] data = new byte[14];
            data[0] = 0x08;
            ByteConverter.BigEndian.ToBytes(serial, data, 1);
            ByteConverter.BigEndian.ToBytes(x, data, 5);
            ByteConverter.BigEndian.ToBytes(y, data, 7);
            ByteConverter.BigEndian.ToBytes(z, data, 9);
            ByteConverter.BigEndian.ToBytes(container, data, 10);
            return data;
        }

        public static byte[] ItemEquipRequest(uint serial, uint character, byte layer)
        {
            byte[] data = new byte[10];
            data[0] = 0x13;
            ByteConverter.BigEndian.ToBytes(serial, data, 1);
            data[5] = layer;
            ByteConverter.BigEndian.ToBytes(character, data, 6);
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="targetId"></param>
        /// <param name="flag">0x00 - Normal; 0x01 - Criminal Action; 0x02 - Unknown; 0x03 - Cancel Target (server-side)</param>
        /// <param name="serial"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="graphic"></param>
        /// <returns></returns>
        public static byte[] Target(byte type, uint targetId, byte flag, uint serial, ushort x, ushort y, sbyte z, ushort graphic)
        {
            byte[] data = new byte[19];
            data[0] = 0x6C;
            data[1] = type;
            ByteConverter.BigEndian.ToBytes(targetId, data, 2);
            data[6] = flag;
            ByteConverter.BigEndian.ToBytes(serial, data, 7);
            ByteConverter.BigEndian.ToBytes(x, data, 11);
            ByteConverter.BigEndian.ToBytes(y, data, 13);
            ByteConverter.BigEndian.ToBytes(z, data, 16);
            ByteConverter.BigEndian.ToBytes(graphic, data, 17);
            return data;
        }

        public static byte[] PickupItemFailed(byte msg)
        {
            byte[] data = new byte[2];
            data[0] = 0x27;
            data[1] = Math.Max(msg, (byte)6);
            return data;
        }

        /// <summary>
        /// This is the client's response to the Object Picker packet.
        /// </summary>
        /// <param name="dialog">The serial of the dialog to which this is responding.</param>
        /// <param name="menu">The serial of the menu to which this is responding.</param>
        /// <param name="option">The 1-based index of the option that was selected.</param>
        /// <param name="art">The artwork number of the selected item.</param>
        /// <param name="hue">The hue of the selected item.</param>
        /// <returns>Assembled packet data.</returns>
        public static byte[] ObjectPicked(uint dialog, ushort menu, ushort option, ushort art, ushort hue)
        {
            byte[] data = new byte[13];
            data[0] = 0x7D;
            ByteConverter.BigEndian.ToBytes(dialog, data, 1);
            ByteConverter.BigEndian.ToBytes(menu, data, 5);
            ByteConverter.BigEndian.ToBytes(option, data, 7);
            ByteConverter.BigEndian.ToBytes(art, data, 9);
            ByteConverter.BigEndian.ToBytes(hue, data, 11);
            return data;
        }

        public static byte[] ObjectDoubleClick(uint serial)
        {
            byte[] data = new byte[5];
            data[0] = 0x06;
            ByteConverter.BigEndian.ToBytes(serial, data, 1);
            return data;
        }

        public static byte[] ObjectClick(uint serial)
        {
            byte[] data = new byte[5];
            data[0] = 0x09;
            ByteConverter.BigEndian.ToBytes(serial, data, 1);
            return data;
        }

        public static byte[] OpenDoor()
        {
            byte[] data = new byte[5];
            data[0] = 0x12;
            data[1] = 0x00;
            data[2] = 0x05;
            data[3] = 0x58;
            data[4] = 0x00;
            return data;
        }

        public static byte[] CastSpell(byte spell)
        {
            PacketWriter writer = new PacketWriter(0x12);
            writer.WriteBlockSize();
            writer.Write((byte)0x56);
            writer.WriteAsciiString(spell.ToString());
            return writer.GetBytes();
        }

        public static byte[] UseSkill(ushort skill)
        {
            PacketWriter writer = new PacketWriter(0x12);
            writer.WriteBlockSize();
            writer.Write((byte)0x24);
            writer.WriteAsciiString(String.Format("{0} 0", skill));
            return writer.GetBytes();
        }

        /// <summary>
        /// Used only for "bow" and "salute" animations.
        /// </summary>
        public static byte[] PerformAnimation(string animationName)
        {
            PacketWriter writer = new PacketWriter(0x12);
            writer.WriteBlockSize();
            writer.Write((byte)0xC7);
            writer.WriteAsciiString(animationName.ToLowerInvariant());
            return writer.GetBytes();
        }

        /// <summary>
        /// Packet will not contain passwords and starting locations. Flags will be set to 0x00.
        /// </summary>
        /// <param name="characters">List of characters.</param>
        public static byte[] CharacterList(params string[] characters)
        {
            PacketWriter writer = new PacketWriter(0xA9);

            writer.WriteBlockSize();
            writer.Write((byte)5);

            for (int i = 0; i < 5; i++) {
                if (i < characters.Length) {
                    writer.WriteAsciiString(characters[i], 30);
                    writer.WriteBytes(0, 30);
                }
                else {
                    writer.WriteBytes(0, 60);
                }
            }

            writer.WriteBytes(0, 2);

            return writer.GetBytes();
        }

        /// <summary>
        /// Character Skills/Stats Request (10 bytes).
        /// </summary>
        /// <param name="serial">UID the client is requesting information about.</param>
        /// <param name="type">4 for stats/status bar, 5 for skills.</param>
        /// <returns></returns>
        public static byte[] CharacterSkillsStatsRequest(uint serial, byte type)
        {
            byte[] data = new byte[10];
            data[0] = 0x34;
            ByteConverter.BigEndian.ToBytes(0xedededed, data, 1);
            data[5] = type;
            ByteConverter.BigEndian.ToBytes(serial, data, 6);
            return data;
        }

        public static byte[] AttackRequest(uint serial)
        {
            byte[] data = new byte[5];
            data[0] = 0x05;
            ByteConverter.BigEndian.ToBytes(serial, data, 1);
            return data;
        }

        public static byte[] ObjectRemove(uint serial)
        {
            byte[] data = new byte[5];
            data[0] = 0x1D;
            ByteConverter.BigEndian.ToBytes(serial, data, 1);
            return data;
        }

        public enum StatsRequestType : byte
        {
            Stats = 4,
            Skills = 5
        }

        public static byte[] CharacterStatsRequest(uint serial, StatsRequestType type)
        {
            byte[] data = new byte[10];
            data[0] = 0x34;
            ByteConverter.BigEndian.ToBytes((uint)0xedededed, data, 1);
            data[5] = (byte)type;
            ByteConverter.BigEndian.ToBytes(serial, data, 6);
            return data;
        }

        public static byte[] Warmode(byte state)
        {
            byte[] data = new byte[5];
            data[0] = 0x72;
            data[1] = state;
            data[2] = 0x32;
            return data;
        }
    }
}
