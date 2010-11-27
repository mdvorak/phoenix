using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    /// <summary>
    /// Phoenix environmental variables.
    /// </summary>
    public static class Env
    {
        public const ushort DefaultFontColor = 0x02B2;
        public const ushort DefaultConsoleColor = 0x3BF;
        public const ushort DefaultInfoColor = 0x9E;
        public const ushort DefaultWarningColor = 0x30;
        public const ushort DefaultErrorColor = 0x21;

        public static ushort FontColor
        {
            get { return Config.Profile.Colors.FontColor.Value; }
            set { Config.Profile.Colors.FontColor.Value = value; }
        }

        public static ushort ConsoleColor
        {
            get { return Config.Profile.Colors.ConsoleColor.Value; }
            set { Config.Profile.Colors.ConsoleColor.Value = value; }
        }

        public static ushort InfoColor
        {
            get { return Config.Profile.Colors.InfoColor.Value; }
            set { Config.Profile.Colors.InfoColor.Value = value; }
        }

        public static ushort WarningColor
        {
            get { return Config.Profile.Colors.WarningColor.Value; }
            set { Config.Profile.Colors.WarningColor.Value = value; }
        }

        public static ushort ErrorColor
        {
            get { return Config.Profile.Colors.ErrorColor.Value; }
            set { Config.Profile.Colors.ErrorColor.Value = value; }
        }
    }
}
