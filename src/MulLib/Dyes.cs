using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MulLib
{
    /// <summary>
    /// Contains methods for dying bitmaps in uo style.
    /// </summary>
    public static class Dyes
    {
        /// <summary>
        /// Dyes bitmap gray pixels.
        /// </summary>
        /// <param name="hue">Colors spectrum that will be used.</param>
        /// <param name="bitmap">Bitmap that will be edited.</param>
        public static void RecolorPartial(HueEntry hue, Bitmap bitmap)
        {
            if (hue == null)
                throw new ArgumentNullException("hue");

            if (bitmap == null)
                return;

            if (bitmap.PixelFormat != PixelFormat.Format16bppArgb1555)
                throw new ArgumentException("Invalid bitmap pixel format.", "source");

            BitmapData data = bitmap.LockBits(Ultima.GetBitmapBounds(bitmap), ImageLockMode.ReadWrite, PixelFormat.Format16bppArgb1555);
            Debug.Assert(data.Stride % 2 == 0, "data.Stride % 2 == 0");

            RecolorPartialInternal(hue, data.Scan0, data.Stride * data.Height / 2);

            bitmap.UnlockBits(data);
        }

        private static unsafe void RecolorPartialInternal(HueEntry hue, IntPtr dataPtr, int lenght)
        {
            ushort* pData = (ushort*)dataPtr.ToPointer();

            for (int i = 0; i < lenght; i++) {
                ushort c = (ushort)((*pData) & 0x1F);
                if (c == (((*pData) >> 5) & 0x1F) && c == (((*pData) >> 10) & 0x1F)) {
                    *pData = (ushort)(((*pData) & 0x8000) | hue.Colors[c]);
                }
                pData++;
            }
        }

        /// <summary>
        /// Dyes whole bitmap. Non grey pixels are first converted to grey and then dyed.
        /// </summary>
        /// <param name="hue">Colors spectrum that will be used.</param>
        /// <param name="bitmap">Bitmap that will be edited.</param>
        public static void RecolorFull(HueEntry hue, Bitmap bitmap)
        {
            if (hue == null)
                throw new ArgumentNullException("hue");

            if (bitmap == null)
                return;

            if (bitmap.PixelFormat != PixelFormat.Format16bppArgb1555)
                throw new ArgumentException("Invalid bitmap pixel format.", "source");

            BitmapData data = bitmap.LockBits(Ultima.GetBitmapBounds(bitmap), ImageLockMode.ReadWrite, PixelFormat.Format16bppArgb1555);
            Debug.Assert(data.Stride % 2 == 0, "data.Stride % 2 == 0");

            RecolorFullInternal(hue, data.Scan0, data.Stride * data.Height / 2);

            bitmap.UnlockBits(data);
        }

        private static unsafe void RecolorFullInternal(HueEntry hue, IntPtr dataPtr, int lenght)
        {
            ushort* pData = (ushort*)dataPtr.ToPointer();

            for (int i = 0; i < lenght; i++) {
                ushort c = (ushort)((((*pData) & 0x1F) + (((*pData) >> 5) & 0x1F) + (((*pData) >> 10) & 0x1F)) / 3);

                *pData = (ushort)(((*pData) & 0x8000) | hue.Colors[c & 0x1F]);

                pData++;
            }
        }
    }
}
