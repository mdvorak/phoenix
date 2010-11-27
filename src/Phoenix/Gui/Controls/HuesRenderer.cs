using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MulLib;


namespace Phoenix.Gui.Controls
{
    public static class HuesRenderer
    {
        private static int Range(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        public static void DrawField(Bitmap bitmap, Hues hues, int startHueIndex, int endHueIndex, Size hueSize, int brightness)
        {
            // Check arguments
            if (bitmap == null) throw new ArgumentNullException("bitmap");
            if (hues == null) throw new ArgumentNullException("hues");

            if (bitmap.PixelFormat != PixelFormat.Format16bppArgb1555)
                throw new ArgumentException("Invalid bitmap pixel format.", "bitmap");

            if (startHueIndex != 0)
                startHueIndex = HuesRenderer.Range(startHueIndex, hues.MinIndex, hues.MaxIndex);
            if (endHueIndex != 0)
                endHueIndex = HuesRenderer.Range(endHueIndex, hues.MinIndex, hues.MaxIndex);
            hueSize.Width = HuesRenderer.Range(hueSize.Width, 1, 64);
            hueSize.Height = HuesRenderer.Range(hueSize.Height, 1, 64);
            brightness = HuesRenderer.Range(brightness, 0, 31);

            int columns = bitmap.Size.Width / hueSize.Width;
            int rows = bitmap.Size.Height / hueSize.Height;

            BitmapData data = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

            for (int y = 0; y < rows; y++)
            {
                short[] line = new short[bitmap.Size.Width];

                for (int x = 0; x < columns; x++)
                {
                    int id = startHueIndex + x * rows + y + 1;

                    if (id <= hues.MaxIndex && (endHueIndex == 0 || id <= endHueIndex))
                    {
                        ushort color = hues.Get(id).Colors[brightness];

                        for (int i = 0; i < hueSize.Width; i++)
                        {
                            line[x * hueSize.Width + i] = (short)(color | 0x8000);
                        }
                    }
                }

                for (int i = 0; i < hueSize.Height; i++)
                {
                    int offset = (int)data.Scan0 + (y * hueSize.Height + i) * data.Stride;
                    Marshal.Copy(line, 0, (IntPtr)offset, line.Length);
                }
            }

            bitmap.UnlockBits(data);
        }

        public static void DrawSpectrum(Bitmap bitmap, Hues hues, int index)
        {
            if (bitmap == null) throw new ArgumentNullException("bitmap");
            if (hues == null) throw new ArgumentNullException("hues");

            if (index < hues.MinIndex || index > hues.MaxIndex)
                throw new ArgumentOutOfRangeException("index");

            HueEntry hue = hues.Get(index);

            BitmapData data = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

            float colorWidth = (float)bitmap.Width / 32.0f;
            short[] line = new short[bitmap.Width];

            for (int x = 0; x < bitmap.Width; x++)
            {
                int id = (int)(x / colorWidth);
                if (id < 32)
                {
                    line[x] = (short)(hue.Colors[id] | 0x8000);
                }
            }

            for (int y = 0; y < bitmap.Height; y++)
            {
                int offset = (int)data.Scan0 + y * data.Stride;
                Marshal.Copy(line, 0, (IntPtr)offset, line.Length);
            }

            bitmap.UnlockBits(data);
        }
    }
}
