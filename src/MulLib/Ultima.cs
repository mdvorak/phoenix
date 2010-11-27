using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace MulLib
{
    /// <summary>
    /// General functions
    /// </summary>
    public static class Ultima
    {
        public const string RegPath = @"Software\Origin Worlds Online\Ultima Online\1.0";
        public const string ThirdDawnRegPath = @"Software\Origin Worlds Online\Ultima Online Third Dawn\1.0";

        /// <summary>
        /// Reads a path from registry "LocalMachine\Software\Origin Worlds Online\Ultima Online Third Dawn\1.0\ExePath"
        /// </summary>
        /// <returns>Path string with backslash at the end.</returns>
        public static string GetDirectory()
        {
            // Open registry key
            RegistryKey rkey = Registry.LocalMachine.OpenSubKey(ThirdDawnRegPath);
            if (rkey == null) rkey = Registry.LocalMachine.OpenSubKey(RegPath);
            if (rkey == null) throw new Exception("Cannot find key in the registry.");

            // Read mulFile from registry value
            string path = rkey.GetValue("ExePath") as string;
            if (path == null) throw new Exception("Cannot read path string from the registry.");

            // Remove executable and return
            return path.Remove(path.LastIndexOf('\\')) + "\\";
        }

        internal static System.Drawing.Rectangle GetBitmapBounds(System.Drawing.Bitmap bitmap)
        {
            return new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
        }
    }
}
