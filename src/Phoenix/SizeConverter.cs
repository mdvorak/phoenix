using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    /// <summary>
    /// Converts bytes count to specified string format.
    /// </summary>
    public static class SizeConverter
    {
        /// <summary>
        /// 1B
        /// </summary>
        public const long Byte = 1;
        /// <summary>
        /// kB in bytes.
        /// </summary>
        public const long Kilobyte = Byte * 1024;
        /// <summary>
        /// MB in bytes.
        /// </summary>
        public const long Megabyte = Kilobyte * 1024;
        /// <summary>
        /// GB in bytes.
        /// </summary>
        public const long Gigabyte = Megabyte * 1024;
        /// <summary>
        /// TB in bytes.
        /// </summary>
        public const long Terabyte = Gigabyte * 1024;

        /// <summary>
        /// Returns size in bytes.
        /// </summary>
        /// <param name="size">Size in bytes.</param>
        /// <returns>String representing specified size in requested format.</returns>
        public static string ToBytes(long size)
        {
            return String.Format("{0} B", size);
        }

        /// <summary>
        /// Returns size in kilobytes.
        /// </summary>
        /// <param name="size">Size in bytes.</param>
        /// <returns>String representing specified size in requested format.</returns>
        public static string ToKiloBytes(long size)
        {
            return String.Format("{0} kB", ((float)size / Kilobyte).ToString("#.##"));
        }

        /// <summary>
        /// Returns size in megabytes.
        /// </summary>
        /// <param name="size">Size in bytes.</param>
        /// <returns>String representing specified size in requested format.</returns>
        public static string ToMegaBytes(long size)
        {
            return String.Format("{0} MB", ((float)size / Megabyte).ToString("#.##"));
        }

        /// <summary>
        /// Returns size in gigabytes.
        /// </summary>
        /// <param name="size">Size in bytes.</param>
        /// <returns>String representing specified size in requested format.</returns>
        public static string ToGigaBytes(long size)
        {
            return String.Format("{0} GB", ((float)size / Gigabyte).ToString("#.##"));
        }

        /// <summary>
        /// Returns size in terabytes.
        /// </summary>
        /// <param name="size">Size in bytes.</param>
        /// <returns>String representing specified size in requested format.</returns>
        public static string ToTeraBytes(long size)
        {
            return String.Format("{0} TB", ((float)size / Terabyte).ToString("#.##"));
        }

        /// <summary>
        /// Returns size in optimal units.
        /// </summary>
        /// <param name="size">Size in bytes.</param>
        /// <returns>String representing specified size in optimal format.</returns>
        public static string ToOptimal(long size)
        {
            if (size > Terabyte)
                return ToTeraBytes(size);
            else if (size > Gigabyte)
                return ToGigaBytes(size);
            else if (size > Megabyte)
                return ToMegaBytes(size);
            else if (size > Kilobyte)
                return ToKiloBytes(size);
            else
                return ToBytes(size);
        }
    }
}
