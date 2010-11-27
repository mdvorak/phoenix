using System;
using System.Text;

#pragma warning disable 3021

namespace Phoenix
{
    public abstract class ByteConverter
    {
        #region InverseConverter class

        private class InverseConverter : ByteConverter
        {
            public InverseConverter()
                : base(BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode)
            {
            }

            public override sbyte ToSByte(byte[] data, int index)
            {
                return (sbyte)data[index];
            }

            public override byte ToByte(byte[] data, int index)
            {
                return data[index];
            }

            public override short ToInt16(byte[] data, int index)
            {
                return (short)(data[index + 1] | (data[index] << 8));
            }

            public override ushort ToUInt16(byte[] data, int index)
            {
                return (ushort)(data[index + 1] | (data[index] << 8));
            }

            public override int ToInt32(byte[] data, int index)
            {
                return (int)(data[index + 3] | (data[index + 2] << 8) | (data[index + 1] << 16) | (data[index] << 24));
            }

            public override uint ToUInt32(byte[] data, int index)
            {
                return (uint)(data[index + 3] | (data[index + 2] << 8) | (data[index + 1] << 16) | (data[index] << 24));
            }

            public override long ToInt64(byte[] data, int index)
            {
                return (long)(data[index + 7] | (data[index + 6] << 8) | (data[index + 5] << 16) | (data[index + 4] << 24) |
                              (data[index + 3] << 32) | (data[index + 2] << 40) | (data[index + 1] << 48) | (data[index] << 56));
            }

            public override ulong ToUInt64(byte[] data, int index)
            {
                return (ulong)(data[index + 7] | (data[index + 6] << 8) | (data[index + 5] << 16) | (data[index + 4] << 24) |
                              (data[index + 3] << 32) | (data[index + 2] << 40) | (data[index + 1] << 48) | (data[index] << 56));
            }

            public override string ToUnicodeString(byte[] data, int index, int lenght)
            {
                string str = unicodeEncoding.GetString(data, index, lenght);
                if (str.Contains("\0")) str = str.Remove(str.IndexOf('\0'));
                return str;
            }

            public override byte[] GetBytes(sbyte value)
            {
                byte[] data = new byte[1];
                data[0] = (byte)value;
                return data;
            }

            public override byte[] GetBytes(byte value)
            {
                byte[] data = new byte[1];
                data[0] = value;
                return data;
            }

            public override byte[] GetBytes(short value)
            {
                byte[] data = new byte[2];
                data[0] = (byte)(value >> 8);
                data[1] = (byte)(value);
                return data;
            }

            public override byte[] GetBytes(ushort value)
            {
                byte[] data = new byte[2];
                data[0] = (byte)(value >> 8);
                data[1] = (byte)(value);
                return data;
            }

            public override byte[] GetBytes(int value)
            {
                byte[] data = new byte[4];
                data[0] = (byte)(value >> 24);
                data[1] = (byte)(value >> 16);
                data[2] = (byte)(value >> 8);
                data[3] = (byte)(value);
                return data;
            }

            public override byte[] GetBytes(uint value)
            {
                byte[] data = new byte[4];
                data[0] = (byte)(value >> 24);
                data[1] = (byte)(value >> 16);
                data[2] = (byte)(value >> 8);
                data[3] = (byte)(value);
                return data;
            }

            public override byte[] GetBytes(long value)
            {
                byte[] data = new byte[8];
                data[0] = (byte)(value >> 56);
                data[1] = (byte)(value >> 48);
                data[2] = (byte)(value >> 40);
                data[3] = (byte)(value >> 32);
                data[4] = (byte)(value >> 24);
                data[5] = (byte)(value >> 16);
                data[6] = (byte)(value >> 8);
                data[7] = (byte)(value);
                return data;
            }

            public override byte[] GetBytes(ulong value)
            {
                byte[] data = new byte[8];
                data[0] = (byte)(value >> 56);
                data[1] = (byte)(value >> 48);
                data[2] = (byte)(value >> 40);
                data[3] = (byte)(value >> 32);
                data[4] = (byte)(value >> 24);
                data[5] = (byte)(value >> 16);
                data[6] = (byte)(value >> 8);
                data[7] = (byte)(value);
                return data;
            }

            public override byte[] GetBytesUnicode(string value)
            {
                return unicodeEncoding.GetBytes(value);
            }

            public override int ToBytes(sbyte value, byte[] data, int index)
            {
                data[index] = (byte)(value);
                return 1;
            }

            public override int ToBytes(byte value, byte[] data, int index)
            {
                data[index] = value;
                return 1;
            }

            public override int ToBytes(short value, byte[] data, int index)
            {
                data[index + 0] = (byte)(value >> 8);
                data[index + 1] = (byte)(value);
                return 2;
            }

            public override int ToBytes(ushort value, byte[] data, int index)
            {
                data[index + 0] = (byte)(value >> 8);
                data[index + 1] = (byte)(value);
                return 2;
            }

            public override int ToBytes(int value, byte[] data, int index)
            {
                data[index + 0] = (byte)(value >> 24);
                data[index + 1] = (byte)(value >> 16);
                data[index + 2] = (byte)(value >> 8);
                data[index + 3] = (byte)(value);
                return 4;
            }

            public override int ToBytes(uint value, byte[] data, int index)
            {
                data[index + 0] = (byte)(value >> 24);
                data[index + 1] = (byte)(value >> 16);
                data[index + 2] = (byte)(value >> 8);
                data[index + 3] = (byte)(value);
                return 4;
            }

            public override int ToBytes(long value, byte[] data, int index)
            {
                data[index + 0] = (byte)(value >> 56);
                data[index + 1] = (byte)(value >> 48);
                data[index + 2] = (byte)(value >> 40);
                data[index + 3] = (byte)(value >> 32);
                data[index + 4] = (byte)(value >> 24);
                data[index + 5] = (byte)(value >> 16);
                data[index + 6] = (byte)(value >> 8);
                data[index + 7] = (byte)(value);
                return 8;
            }

            public override int ToBytes(ulong value, byte[] data, int index)
            {
                data[index + 0] = (byte)(value >> 56);
                data[index + 1] = (byte)(value >> 48);
                data[index + 2] = (byte)(value >> 40);
                data[index + 3] = (byte)(value >> 32);
                data[index + 4] = (byte)(value >> 24);
                data[index + 5] = (byte)(value >> 16);
                data[index + 6] = (byte)(value >> 8);
                data[index + 7] = (byte)(value);
                return 8;
            }
        }

        #endregion

        #region NormalConverter class

        private class NormalConverter : ByteConverter
        {
            public NormalConverter()
                : base(BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode)
            {
            }

            public override sbyte ToSByte(byte[] data, int index)
            {
                return (sbyte)data[index];
            }

            public override byte ToByte(byte[] data, int index)
            {
                return data[index];
            }

            public override short ToInt16(byte[] data, int index)
            {
                return (short)(data[index] | (data[index + 1] << 8));
            }

            public override ushort ToUInt16(byte[] data, int index)
            {
                return (ushort)(data[index] | (data[index + 1] << 8));
            }

            public override int ToInt32(byte[] data, int index)
            {
                return (int)(data[index] | (data[index + 1] << 8) | (data[index + 2] << 16) | (data[index + 3] << 24));
            }

            public override uint ToUInt32(byte[] data, int index)
            {
                return (uint)(data[index] | (data[index + 1] << 8) | (data[index + 2] << 16) | (data[index + 3] << 24));
            }

            public override long ToInt64(byte[] data, int index)
            {
                return (long)(data[index] | (data[index + 1] << 8) | (data[index + 2] << 16) | (data[index + 3] << 24) |
                              (data[index + 4] << 32) | (data[index + 5] << 40) | (data[index + 6] << 48) | (data[index + 7] << 56));
            }

            public override ulong ToUInt64(byte[] data, int index)
            {
                return (ulong)(data[index] | (data[index + 1] << 8) | (data[index + 2] << 16) | (data[index + 3] << 24) |
                              (data[index + 4] << 32) | (data[index + 5] << 40) | (data[index + 6] << 48) | (data[index + 7] << 56));
            }

            public override string ToUnicodeString(byte[] data, int index, int lenght)
            {
                string str = unicodeEncoding.GetString(data, index, lenght);
                if (str.Contains("\0")) str = str.Remove(str.IndexOf('\0'));
                return str;
            }

            public override byte[] GetBytes(sbyte value)
            {
                byte[] data = new byte[1];
                data[0] = (byte)value;
                return data;
            }

            public override byte[] GetBytes(byte value)
            {
                byte[] data = new byte[1];
                data[0] = value;
                return data;
            }

            public override byte[] GetBytes(short value)
            {
                byte[] data = new byte[2];
                data[0] = (byte)(value);
                data[1] = (byte)(value >> 8);
                return data;
            }

            public override byte[] GetBytes(ushort value)
            {
                byte[] data = new byte[2];
                data[0] = (byte)(value);
                data[1] = (byte)(value >> 8);
                return data;
            }

            public override byte[] GetBytes(int value)
            {
                byte[] data = new byte[4];
                data[0] = (byte)(value);
                data[1] = (byte)(value >> 8);
                data[2] = (byte)(value >> 16);
                data[3] = (byte)(value >> 24);
                return data;
            }

            public override byte[] GetBytes(uint value)
            {
                byte[] data = new byte[4];
                data[0] = (byte)(value);
                data[1] = (byte)(value >> 8);
                data[2] = (byte)(value >> 16);
                data[3] = (byte)(value >> 24);
                return data;
            }

            public override byte[] GetBytes(long value)
            {
                byte[] data = new byte[8];
                data[0] = (byte)(value);
                data[1] = (byte)(value >> 8);
                data[2] = (byte)(value >> 16);
                data[3] = (byte)(value >> 24);
                data[2] = (byte)(value >> 32);
                data[3] = (byte)(value >> 40);
                data[2] = (byte)(value >> 48);
                data[3] = (byte)(value >> 56);
                return data;
            }

            public override byte[] GetBytes(ulong value)
            {
                byte[] data = new byte[8];
                data[0] = (byte)(value);
                data[1] = (byte)(value >> 8);
                data[2] = (byte)(value >> 16);
                data[3] = (byte)(value >> 24);
                data[2] = (byte)(value >> 32);
                data[3] = (byte)(value >> 40);
                data[2] = (byte)(value >> 48);
                data[3] = (byte)(value >> 56);
                return data;
            }

            public override byte[] GetBytesUnicode(string value)
            {
                return Encoding.Unicode.GetBytes(value);
            }

            public override int ToBytes(sbyte value, byte[] data, int index)
            {
                data[index] = (byte)(value);
                return 1;
            }

            public override int ToBytes(byte value, byte[] data, int index)
            {
                data[index] = value;
                return 1;
            }

            public override int ToBytes(short value, byte[] data, int index)
            {
                data[index + 1] = (byte)(value >> 8);
                data[index + 0] = (byte)(value);
                return 2;
            }

            public override int ToBytes(ushort value, byte[] data, int index)
            {
                data[index + 1] = (byte)(value >> 8);
                data[index + 0] = (byte)(value);
                return 2;
            }

            public override int ToBytes(int value, byte[] data, int index)
            {
                data[index + 3] = (byte)(value >> 24);
                data[index + 2] = (byte)(value >> 16);
                data[index + 1] = (byte)(value >> 8);
                data[index + 0] = (byte)(value);
                return 4;
            }

            public override int ToBytes(uint value, byte[] data, int index)
            {
                data[index + 3] = (byte)(value >> 24);
                data[index + 2] = (byte)(value >> 16);
                data[index + 1] = (byte)(value >> 8);
                data[index + 0] = (byte)(value);
                return 4;
            }

            public override int ToBytes(long value, byte[] data, int index)
            {
                data[index + 7] = (byte)(value >> 56);
                data[index + 6] = (byte)(value >> 48);
                data[index + 5] = (byte)(value >> 40);
                data[index + 4] = (byte)(value >> 32);
                data[index + 3] = (byte)(value >> 24);
                data[index + 2] = (byte)(value >> 16);
                data[index + 1] = (byte)(value >> 8);
                data[index + 0] = (byte)(value);
                return 8;
            }

            public override int ToBytes(ulong value, byte[] data, int index)
            {
                data[index + 7] = (byte)(value >> 56);
                data[index + 6] = (byte)(value >> 48);
                data[index + 5] = (byte)(value >> 40);
                data[index + 4] = (byte)(value >> 32);
                data[index + 3] = (byte)(value >> 24);
                data[index + 2] = (byte)(value >> 16);
                data[index + 1] = (byte)(value >> 8);
                data[index + 0] = (byte)(value);
                return 8;
            }
        }

        #endregion

        #region Static field and methods

        /// <summary>
        /// Big Endian means that the high-order byte of the number is stored in memory at the lowest address,
        /// and the low-order byte at the highest address.
        /// It is used on i.e. Mac platform.
        /// </summary>
        public static readonly ByteConverter BigEndian;

        /// <summary>
        /// Little Endian means that the low-order byte of the number is stored in memory at the lowest address,
        /// and the high-order byte at the highest address.
        /// It is used on PC platform.
        /// </summary>
        public static readonly ByteConverter LittleEndian;

        /// <summary>
        /// Gets ByteConverter with current platform byte-order.
        /// </summary>
        public static readonly ByteConverter Default;

        static ByteConverter()
        {
            if (BitConverter.IsLittleEndian) {
                BigEndian = new InverseConverter();
                LittleEndian = new NormalConverter();
            }
            else {
                BigEndian = new NormalConverter();
                LittleEndian = new InverseConverter();
            }

            Default = new NormalConverter();
        }

        public static ByteConverter Create(bool littleEndian)
        {
            if (littleEndian == BitConverter.IsLittleEndian)
                return new NormalConverter();
            else
                return new InverseConverter();
        }

        #endregion

        private Encoding encoding = Encoding.Default;

        public Encoding AsciiEncoding
        {
            get { return encoding; }
            set
            {
                if (value.IsSingleByte)
                    encoding = value;
                else
                    throw new ArgumentException("Encoding must be single-byte.");
            }
        }

        private Encoding unicodeEncoding;

        protected ByteConverter(Encoding unicodeEncoding)
        {
            this.unicodeEncoding = unicodeEncoding;
        }

        [CLSCompliant(false)]
        public virtual sbyte ToSByte(byte[] data, int index) { throw new NotImplementedException(); }
        public abstract byte ToByte(byte[] data, int index);
        public abstract short ToInt16(byte[] data, int index);
        [CLSCompliant(false)]
        public virtual ushort ToUInt16(byte[] data, int index) { throw new NotImplementedException(); }
        public abstract int ToInt32(byte[] data, int index);
        [CLSCompliant(false)]
        public virtual uint ToUInt32(byte[] data, int index) { throw new NotImplementedException(); }
        public abstract long ToInt64(byte[] data, int index);
        [CLSCompliant(false)]
        public virtual ulong ToUInt64(byte[] data, int index) { throw new NotImplementedException(); }
        public abstract string ToUnicodeString(byte[] data, int index, int lenght);

        public string ToAsciiString(byte[] data, int index, int lenght)
        {
            string str = encoding.GetString(data, index, lenght);
            if (str.Contains("\0")) str = str.Remove(str.IndexOf('\0'));
            return str;
        }

        [CLSCompliant(false)]
        public virtual byte[] GetBytes(sbyte value) { throw new NotImplementedException(); }
        public abstract byte[] GetBytes(byte value);
        public abstract byte[] GetBytes(short value);
        [CLSCompliant(false)]
        public virtual byte[] GetBytes(ushort value) { throw new NotImplementedException(); }
        public abstract byte[] GetBytes(int value);
        [CLSCompliant(false)]
        public virtual byte[] GetBytes(uint value) { throw new NotImplementedException(); }
        public abstract byte[] GetBytes(long value);
        [CLSCompliant(false)]
        public virtual byte[] GetBytes(ulong value) { throw new NotImplementedException(); }
        public abstract byte[] GetBytesUnicode(string value);

        public byte[] GetBytesAscii(string value)
        {
            return encoding.GetBytes(value);
        }

        [CLSCompliant(false)]
        public virtual int ToBytes(sbyte value, byte[] data, int index) { throw new NotImplementedException(); }
        public abstract int ToBytes(byte value, byte[] data, int index);
        public abstract int ToBytes(short value, byte[] data, int index);
        [CLSCompliant(false)]
        public virtual int ToBytes(ushort value, byte[] data, int index) { throw new NotImplementedException(); }
        public abstract int ToBytes(int value, byte[] data, int index);
        [CLSCompliant(false)]
        public virtual int ToBytes(uint value, byte[] data, int index) { throw new NotImplementedException(); }
        public abstract int ToBytes(long value, byte[] data, int index);
        [CLSCompliant(false)]
        public virtual int ToBytes(ulong value, byte[] data, int index) { throw new NotImplementedException(); }

        public int ToBytesAscii(string value, byte[] data, int index)
        {
            byte[] stringBytes = encoding.GetBytes(value);
            Array.Copy(stringBytes, 0, data, index, stringBytes.Length);
            return stringBytes.Length;
        }

        public int ToBytesAscii(string value, byte[] data, int index, int len)
        {
            return encoding.GetBytes(value, 0, Math.Min(len, value.Length), data, index);
        }


        public int ToBytesUnicode(string value, byte[] data, int index)
        {
            byte[] stringBytes = unicodeEncoding.GetBytes(value);
            Array.Copy(stringBytes, 0, data, index, stringBytes.Length);
            return stringBytes.Length;
        }

        public int ToBytesUnicode(string value, byte[] data, int index, int len)
        {
            return unicodeEncoding.GetBytes(value, 0, Math.Min(len, value.Length), data, index);
        }
    }
}
