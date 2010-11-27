using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Communication;
using Phoenix.Communication.Packets;
using System.Diagnostics;

namespace Phoenix.Macros
{
    public static class PacketTranslator
    {
        private static byte[] knownPackets = new byte[] { 0x06, 0x6C, 0x12, AsciiSpeechRequest.PacketId, UnicodeSpeechRequest.PacketId, 0x7D };
        private static Menu lastMenu = null;

        static PacketTranslator()
        {
            // TODO: Do it another way 
            Core.RegisterServerMessageCallback(Menu.PacketId, new MessageCallback(OnObjectPicker));
        }

        public static byte[] GetKnownPackets()
        {
            return knownPackets;
        }

        public static IMacroCommand Translate(byte[] data)
        {
            switch (data[0]) {
                case 0x06:
                    return OnDoubleClick(data);

                case 0x6C:
                    return OnTarget(data);

                case 0x12:
                    return OnAction(data);

                case AsciiSpeechRequest.PacketId:
                    return OnAsciiSpeechRequest(data);

                case UnicodeSpeechRequest.PacketId:
                    return OnUnicodeSpeechRequest(data);

                case 0x7D:
                    return OnObjectPicked(data);

                default:
                    throw new ArgumentException(String.Format("Unknow packet (0x{0:X2}).", data[0]));
            }
        }

        private static IMacroCommand OnDoubleClick(byte[] data)
        {
            uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);
            return new UseObjectCommand(serial);
        }

        private static IMacroCommand OnTarget(byte[] data)
        {
            TargetData ti = TargetData.FromData(data);
            return new WaitTargetCommand(ti);
        }

        private static IMacroCommand OnAction(byte[] data)
        {
            PacketReader reader = new PacketReader(data);

            reader.Skip(3);

            byte action = reader.ReadByte();
            string cmd = reader.ReadAnsiString(reader.Length - 4);

            switch (action) {
                case 0x24: // UseSkill
                    Debug.Assert(cmd.EndsWith(" 0"));
                    return new UseSkillMacroCommand(Byte.Parse(cmd.Remove(cmd.Length - 2)));

                case 0x56: // Cast
                    return new CastMacroCommand(Byte.Parse(cmd));

                case 0x58: // OpenDoor
                    return new OpenDoorMacroCommand();

                default:
                    return null;
            }
        }

        private static IMacroCommand OnAsciiSpeechRequest(byte[] data)
        {
            AsciiSpeechRequest packet = new AsciiSpeechRequest(data);
            return new SpeechMacroCommand(packet.Color, packet.Text);
        }

        private static IMacroCommand OnUnicodeSpeechRequest(byte[] data)
        {
            UnicodeSpeechRequest packet = new UnicodeSpeechRequest(data);
            return new SpeechMacroCommand(packet.Color, packet.Text);
        }

        private static CallbackResult OnObjectPicker(byte[] data, CallbackResult prevResult)
        {
            lastMenu = new Menu(data);
            return CallbackResult.Normal;
        }

        private static IMacroCommand OnObjectPicked(byte[] data)
        {
            try {
                if (lastMenu != null) {
                    uint dlgSerial = ByteConverter.BigEndian.ToUInt32(data, 1);

                    if (dlgSerial == lastMenu.DialogSerial) {
                        int choice = ByteConverter.BigEndian.ToUInt16(data, 7) - 1;

                        if (choice < 0) {
                            return new WaitMenuMacroCommand(new MenuSelection(lastMenu.Title, null));
                        }
                        else if (choice < lastMenu.Options.Count) {
                            return new WaitMenuMacroCommand(new MenuSelection(lastMenu.Title, lastMenu.Options[choice].Text));
                        }
                    }
                }

                UO.Print(Env.ErrorColor, "Error recording macro: Unable to resolve MenuSelect.");
                return null;
            }
            finally {
                lastMenu = null;
            }
        }
    }
}
