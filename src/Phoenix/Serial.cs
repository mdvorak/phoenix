using System;
using System.Globalization;
using System.Text;

namespace Phoenix
{
    [Serializable]
    public struct Serial : IFormattable, IConvertible, IComparable<UInt32>, IComparable
    {
        public const uint Invalid = UInt32.MaxValue;

        private uint value;

        public Serial(uint value)
        {
            this.value = value;
        }

        public bool IsValid
        {
            get { return value != Serial.Invalid; }
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Serial) return this == (Serial)obj;
            if (obj is IConvertible) return value == Convert.ToUInt32(obj);
            return false;
        }

        public override string ToString()
        {
            return "0x" + value.ToString("X8");
        }

        public static implicit operator Serial(UInt32 value)
        {
            return new Serial(value);
        }

        public static implicit operator UInt32(Serial serial)
        {
            return serial.value;
        }

        public static implicit operator Serial(Int32 value)
        {
            return new Serial((uint)value);
        }

        public static explicit operator Int32(Serial serial)
        {
            return (int)serial.value;
        }

        public static bool operator ==(Serial s1, Serial s2)
        {
            return s1.value == s2.value;
        }

        public static bool operator !=(Serial s1, Serial s2)
        {
            return s1.value != s2.value;
        }

        public static Serial Parse(string s)
        {
            if (s.StartsWith("0x"))
                return (Serial)UInt32.Parse(s.Remove(0, 2), NumberStyles.HexNumber);
            else
                return (Serial)UInt32.Parse(s);
        }

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.UInt32;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(value);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(value);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(value);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(value);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(value);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(value);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(value);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(value);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(value);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(value);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return Convert.ToString(value);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (GetType() == conversionType)
                return this;
            else
                return Convert.ChangeType(value, conversionType);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(value);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return value;
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(value);
        }

        #endregion

        #region IComparable<uint> Members

        int IComparable<uint>.CompareTo(uint other)
        {
            return value.CompareTo(other);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return value.CompareTo(obj);
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString();
            else
            return value.ToString(format, formatProvider);
        }

        #endregion
    }
}
