using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.Communication;
using Phoenix.Configuration;
using Phoenix.WorldData;

namespace Phoenix
{
    partial class UO
    {
        /// <summary>
        /// Znaci, zdali se na prikaz <see cref="Print"/> pouziva unicode.
        /// </summary>
        [ThreadStatic]
        public static bool UseUnicodePrint;

        /// <summary>
        /// Prints the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        public static void Print(object o)
        {
            if (o != null)
                PrintInternal("Phoenix", o.ToString(), Env.ConsoleColor, SpeechFont.Normal, SpeechType.Regular);
        }

        /// <summary>
        /// Prints the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        [Command]
        public static void Print(string text)
        {
            PrintInternal("Phoenix", text, Env.ConsoleColor, SpeechFont.Normal, SpeechType.Regular);
        }

        /// <summary>
        /// Prints the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="text">The text.</param>
        [Command]
        public static void Print(ushort color, string text)
        {
            PrintInternal("Phoenix", text, color, SpeechFont.Normal, SpeechType.Regular);
        }

        /// <summary>
        /// Prints the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void Print(string format, params object[] args)
        {
            string text = String.Format(format, args);
            PrintInternal("Phoenix", text, Env.ConsoleColor, SpeechFont.Normal, SpeechType.Regular);
        }

        /// <summary>
        /// Prints the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void Print(ushort color, string format, params object[] args)
        {
            string text = String.Format(format, args);
            PrintInternal("Phoenix", text, color, SpeechFont.Normal, SpeechType.Regular);
        }

        /// <summary>
        /// Prints the specified font.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="color">The color.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void Print(SpeechFont font, ushort color, string format, params object[] args)
        {
            string text = String.Format(format, args);
            PrintInternal("Phoenix", text, color, font, SpeechType.Regular);
        }

        /// <summary>
        /// Prints the information.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void PrintInformation(string format, params object[] args)
        {
            string text = String.Format(format, args);
            PrintInternal("Information", text, Env.InfoColor, SpeechFont.Normal, SpeechType.Regular);
        }

        /// <summary>
        /// Prints the warning.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void PrintWarning(string format, params object[] args)
        {
            string text = String.Format(format, args);
            PrintInternal("Warning", text, Env.WarningColor, SpeechFont.Normal, SpeechType.Regular);
        }

        /// <summary>
        /// Prints the error.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void PrintError(string format, params object[] args)
        {
            string text = String.Format(format, args);
            PrintInternal("Error", text, Env.ErrorColor, SpeechFont.Normal, SpeechType.Regular);
        }

        private static void PrintInternal(string speaker, string text, ushort color, SpeechFont font, SpeechType type)
        {
            if (text != null) {
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++) {
                    byte[] data;

                    if (UseUnicodePrint)
                        data = PacketBuilder.CharacterSpeechUnicode(0xFFFFFFFF, 0xFFFF, speaker, type, font, color, lines[i]);
                    else
                        data = PacketBuilder.CharacterSpeechAscii(0xFFFFFFFF, 0xFFFF, speaker, type, font, color, lines[i]);

                    Core.SendToClient(data, true);
                }
            }
        }

        /// <summary>
        /// Says the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        [Command("say")]
        public static void Say(string text)
        {
            SayInternal(SpeechType.Regular, Env.FontColor, text);
        }

        /// <summary>
        /// Says the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="text">The text.</param>
        [Command("say")]
        public static void Say(ushort color, string text)
        {
            SayInternal(SpeechType.Regular, color, text);
        }

        /// <summary>
        /// Says the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void Say(string format, params object[] args)
        {
            Say(SpeechType.Regular, Env.FontColor, format, args);
        }

        /// <summary>
        /// Says the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void Say(ushort color, string format, params object[] args)
        {
            Say(SpeechType.Regular, color, format, args);
        }

        /// <summary>
        /// Says the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="color">The color.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void Say(SpeechType type, ushort color, string format, params object[] args)
        {
            string text = String.Format(format, args);
            SayInternal(type, color, text);
        }

        /// <summary>
        /// Says the internal.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="color">The color.</param>
        /// <param name="text">The text.</param>
        private static void SayInternal(SpeechType type, ushort color, string text)
        {
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++) {
                string line = "";

                for (int x = 0; x < lines[i].Length; x++) {
                    if (lines[i][x] >= ' ')
                        line += lines[i][x];
                }

                byte[] data = PacketBuilder.SpeechRequestUnicode(type, SpeechFont.Normal, color, line);
                Core.SendToServer(data, true);
            }
        }

        /// <summary>
        /// Prints the object.
        /// </summary>
        /// <param name="serial">The serial.</param>
        /// <param name="text">The text.</param>
        [Command("printo")]
        public static void PrintObject(Serial serial, string text)
        {
            PrintObjectInternal(serial, Env.FontColor, text);
        }

        /// <summary>
        /// Prints the object.
        /// </summary>
        /// <param name="serial">The serial.</param>
        /// <param name="color">The color.</param>
        /// <param name="text">The text.</param>
        [Command("printo")]
        public static void PrintObject(Serial serial, ushort color, string text)
        {
            PrintObjectInternal(serial, color, text);
        }

        /// <summary>
        /// Prints the object.
        /// </summary>
        /// <param name="serial">The serial.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void PrintObject(Serial serial, string format, params object[] args)
        {
            PrintObject(serial, Env.FontColor, String.Format(format, args));
        }

        /// <summary>
        /// Prints text above object of specified id (like if it said it but you are the only one who see it).
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="color"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <remarks>When object doesn't exist in object it acts like Print.</remarks>
        public static void PrintObject(Serial serial, ushort color, string format, params object[] args)
        {
            PrintObjectInternal(serial, color, String.Format(format, args));
        }

        private static void PrintObjectInternal(Serial serial, ushort color, string text)
        {
            RealObject ro = World.GetRealObject(serial);
            string name = ro.Name != null ? ro.Name : "";

            string[] lines = text.Replace("\r", "").Split('\n');
            for (int i = 0; i < lines.Length; i++) {
                byte[] data = PacketBuilder.CharacterSpeechUnicode(serial, ro.Graphic, name, SpeechType.Regular, SpeechFont.Normal, color, lines[i]);
                Core.SendToClient(data, true);
            }
        }
    }
}
