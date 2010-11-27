using System;
using System.Text.RegularExpressions;

namespace Phoenix.WorldData
{
    public struct WorldLocation
    {
        private ushort x;
        private ushort y;
        private sbyte z;

        /// <summary>
        /// Initializes WorldLocation object with specified values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public WorldLocation(ushort x, ushort y, sbyte z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public ushort X
        {
            get { return x; }
            set { x = value; }
        }

        public ushort Y
        {
            get { return y; }
            set { y = value; }
        }

        public sbyte Z
        {
            get { return z; }
            set { z = value; }
        }

        public override int GetHashCode()
        {
            return x ^ y ^ z;
        }

        /// <summary>
        /// Returns human readable representation of object.
        /// </summary>
        /// <returns>String in X.Y.Z format.</returns>
        public override string ToString()
        {
            return String.Format("{0}.{1}.{2}", x, y, z);
        }


        private static readonly Regex regex = new Regex(@"(\d+)\s*\x2E\s*(\d+)\s*\x2E\s*(-?\d+)", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Parses string to WorldLocation.
        /// </summary>
        /// <param name="input">String in X.Y.Z format.</param>
        /// <returns>Read WorldLocation object.</returns>
        /// <exception cref="System.ArgumentNullException">Input argument is null.</exception>
        /// <exception cref="System.FormatException">Unable to parse input string.</exception>
        public static WorldLocation Parse(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            Match match = regex.Match(input);

            if(!match.Success)
                throw new FormatException("Unable to parse input string.");

            try
            {
                WorldLocation loc = new WorldLocation();
                loc.x = UInt16.Parse(match.Groups[0].Value);
                loc.y = UInt16.Parse(match.Groups[1].Value);
                loc.z = SByte.Parse(match.Groups[2].Value);
                return loc;
            }
            catch (Exception e)
            {
                throw new FormatException("Unable to parse input string.", e);
            }
        }
    }
}
