using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Phoenix.Communication.Packets
{
    public struct MenuOption
    {
        public ushort Artwork;
        public ushort Hue;
        public string Text;
    }

    public class Menu : VariablePacket
    {
        public const byte PacketId = 0x7C;

        private uint dialogSerial;
        private ushort menuSerial;
        private string title;
        private ReadOnlyCollection<MenuOption> options;

        public Menu(byte[] data)
            : base(data)
        {
            PacketReader reader = new PacketReader(data);
            byte id = reader.ReadByte();
            ushort blockSize = reader.ReadUInt16();

            dialogSerial = reader.ReadUInt32();
            menuSerial = reader.ReadUInt16();

            byte titleLen = reader.ReadByte();
            title = reader.ReadAnsiString(titleLen);

            byte optionCount = reader.ReadByte();
            MenuOption[] optionList = new MenuOption[optionCount];

            for (int i = 0; i < optionCount; i++) {
                optionList[i].Artwork = reader.ReadUInt16();
                optionList[i].Hue = reader.ReadUInt16();

                byte textLen = reader.ReadByte();
                optionList[i].Text = reader.ReadAnsiString(textLen);
            }

            options = Array.AsReadOnly<MenuOption>(optionList);

            // if (reader.Offset == reader.Length)
            //     System.Diagnostics.Debug.WriteLine("Menu packet resolved succesfully.", "Communication");
        }

        public override byte Id
        {
            get { return 0x7C; }
        }

        public uint DialogSerial
        {
            get { return dialogSerial; }
        }

        public ushort MenuSerial
        {
            get { return menuSerial; }
        }

        public string Title
        {
            get { return title; }
        }

        public ReadOnlyCollection<MenuOption> Options
        {
            get { return options; }
        }
    }
}
