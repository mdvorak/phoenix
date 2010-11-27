using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UOEncryption
{
    public enum LoginEncryptionType
    {
        /// <summary>
        /// Non-ecprypted communication
        /// </summary>
        None,

        /// <summary>
        /// Pre 1.26.35 clients.
        /// </summary>
        Old,

        /// <summary>
        /// 1.26.36 client only.
        /// </summary>
        Rare,

        /// <summary>
        /// >= 1.26.36 clients.
        /// </summary>
        New
    }

    public enum GameEncryptionType
    {
        /// <summary>
        /// Non-ecprypted communication
        /// </summary>
        None,

        /// <summary>
        /// Pre 2.0.0 clients. Using Blowfish
        /// </summary>
        Old,

        /// <summary>
        /// 2.0.0-2.0.3 clients. Using Blowfish+Twofish
        /// </summary>
        Rare,

        /// <summary>
        /// >= 2.0.4 clients. Using Twofish.
        /// </summary>
        New
    }

    public struct EncryptionVersion : IComparable<EncryptionVersion>
    {
        private int major;
        private int minor;
        private int minor2;

        public EncryptionVersion(int major, int minor, int minor2)
        {
            this.major = major;
            this.minor = minor;
            this.minor2 = minor2;
        }

        public int Major
        {
            get { return major; }
            set { major = value; }
        }

        public int Minor
        {
            get { return minor; }
            set { minor = value; }
        }

        public int Minor2
        {
            get { return minor2; }
            set { minor2 = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EncryptionVersion && CompareTo((EncryptionVersion)obj) == 0)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return major ^ minor ^ minor2;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}.{2}", major, minor, minor2);
        }

        public int CompareTo(EncryptionVersion other)
        {
            if (major == other.major)
            {
                if (minor == other.minor)
                {
                    return minor2 - other.minor2;
                }
                else
                {
                    return minor - other.minor;
                }
            }
            else
            {
                return major - other.major;
            }
        }

        public GameEncryptionType GameEncryptionType
        {
            get
            {
                if (major == 0 && minor == 0 && minor2 == 0)
                    return GameEncryptionType.None;
                else if (this < new EncryptionVersion(2, 0, 0))
                    return GameEncryptionType.Old;
                else if (this >= new EncryptionVersion(2, 0, 4))
                    return GameEncryptionType.New;
                else
                    return GameEncryptionType.Rare;
            }
        }

        public LoginEncryptionType LoginEncryptionType
        {
            get
            {
                if (major == 0 && minor == 0 && minor2 == 0)
                    return LoginEncryptionType.None;
                else if (this < new EncryptionVersion(1, 25, 36))
                    return LoginEncryptionType.Old;
                else if (this > new EncryptionVersion(1, 25, 36))
                    return LoginEncryptionType.New;
                else
                    return LoginEncryptionType.Rare;
            }
        }

        public static EncryptionVersion Parse(string version)
        {
            Regex regex = new Regex(@"\d+");

            MatchCollection matches = regex.Matches(version);
            if (matches.Count != 3) throw new FormatException("Invalid version string format.");

            EncryptionVersion v = new EncryptionVersion();
            v.major = Int32.Parse(matches[0].Value);
            v.minor = Int32.Parse(matches[1].Value);
            v.minor2 = Int32.Parse(matches[2].Value);

            return v;
        }

        public static EncryptionVersion FromGame(GameEncryptionType game)
        {
            switch (game)
            {
                case GameEncryptionType.None:
                    return new EncryptionVersion();

                case GameEncryptionType.Old:
                    return new EncryptionVersion(1, 26, 4);

                case GameEncryptionType.Rare:
                    return new EncryptionVersion(2, 0, 0);

                case GameEncryptionType.New:
                    return new EncryptionVersion(2, 0, 4);

                default:
                    throw new Exception("Internal error.");
            }
        }

        public static EncryptionVersion FromLogin(LoginEncryptionType login)
        {
            switch (login)
            {
                case LoginEncryptionType.None:
                    return new EncryptionVersion();

                case LoginEncryptionType.Old:
                    return new EncryptionVersion(1, 25, 35);

                case LoginEncryptionType.Rare:
                    return new EncryptionVersion(1, 25, 36);

                case LoginEncryptionType.New:
                    return new EncryptionVersion(1, 25, 37);

                default:
                    throw new Exception("Internal error.");
            }
        }

        public static bool operator ==(EncryptionVersion v1, EncryptionVersion v2)
        {
            return v1.CompareTo(v2) == 0;
        }

        public static bool operator !=(EncryptionVersion v1, EncryptionVersion v2)
        {
            return v1.CompareTo(v2) != 0;
        }

        public static bool operator >(EncryptionVersion v1, EncryptionVersion v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator >=(EncryptionVersion v1, EncryptionVersion v2)
        {
            return v1.CompareTo(v2) >= 0;
        }

        public static bool operator <(EncryptionVersion v1, EncryptionVersion v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator <=(EncryptionVersion v1, EncryptionVersion v2)
        {
            return v1.CompareTo(v2) <= 0;
        }
    }
}
