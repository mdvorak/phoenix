using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MulLib
{
    /// <summary>
    /// Contains basic functions to converting colors from/to Ultima Online format (A1R5G5B5).
    /// </summary>
    public static class UOColorConverter
    {
        /// <summary>
        /// Converts specified A1R5G5B5 color to A8R8G8B8 format.
        /// </summary>
        /// <param name="color">16-bit color to convert.</param>
        /// <returns>ARGB color.</returns>
        public static int ToArgb(ushort color)
        {
            int argb = (int)(((((color) >> 0) & 0x1F) << 3) | (((((color) >> 5) & 0x1F) << 3) << 8) | (((((color) >> 10) & 0x1F) << 3) << 16));

            if ((color & 0x8000) != 0)
                argb |= (0xFF << 24);

            return argb;
        }

        /// <summary>
        /// Converts specified A8R8G8B8 color to A1R5G5B5 format.
        /// </summary>
        /// <param name="argb">32-bit color to convert.</param>
        /// <returns>16-bit color.</returns>
        public static ushort FromArgb(int argb)
        {
            ushort color = 0;
            if ((argb >> 24) != 0) color = 0x8000;

            color |= ((ushort)(((((((argb) >> 16) & 0xFF) >> 3) & 0x1F) << 10) | ((((((argb) >> 8) & 0xFF) >> 3) & 0x1F) << 5) | (((((argb) & 0xFF) >> 3) & 0x1F) << 0)));
            return color;
        }

        /// <summary>
        /// Converts specified A1R5G5B5 color to Color object.
        /// </summary>
        /// <param name="color">16-bit color to convert.</param>
        /// <returns>New instance of Color object.</returns>
        public static Color ToColor(ushort color)
        {
            return Color.FromArgb(ToArgb(color));
        }

        /// <summary>
        /// Converts specified Color to A1R5G5B5 color format.
        /// </summary>
        /// <param name="color">Color to convert.</param>
        /// <returns>16-bit A1R5G5B5 color.</returns>
        public static ushort FromColor(Color color)
        {
            return FromArgb(color.ToArgb());
        }
    }
}
