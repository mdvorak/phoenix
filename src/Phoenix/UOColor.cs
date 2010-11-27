using System;
using System.Globalization;
using System.Text;

namespace Phoenix
{
    [Serializable]
    public struct UOColor : IFormattable, IConvertible, IComparable<UInt16>, IComparable
    {
        public static UOColor Invariant
        {
            get { return new UOColor(UInt16.MaxValue); }
        }

        private ushort value;

        public UOColor(ushort value)
        {
            this.value = value;
        }

        public bool IsConstant
        {
            get { return value != UInt16.MaxValue; }
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is UOColor) return this == (UOColor)obj;
            if (obj is IConvertible)
            {
                ushort s = Convert.ToUInt16(obj);
                if (value == UInt16.MaxValue || s == UInt16.MaxValue)
                    return true;
                return value == s;
            }
            return false;
        }

        public override string ToString()
        {
            return "0x" + value.ToString("X4");
        }

        public static implicit operator UOColor(UInt16 value)
        {
            return new UOColor(value);
        }

        public static implicit operator UInt16(UOColor graphic)
        {
            return graphic.value;
        }

        public static implicit operator UOColor(Int32 value)
        {
            return new UOColor((ushort)value);
        }

        public static explicit operator Int16(UOColor graphic)
        {
            return (short)graphic.value;
        }

        public static bool operator ==(UOColor c1, UOColor c2)
        {
            if (c1.value == UInt16.MaxValue || c2.value == UInt16.MaxValue)
                return true;
            return c1.value == c2.value;
        }

        public static bool operator !=(UOColor c1, UOColor c2)
        {
            if (c1.value == UInt16.MaxValue || c2.value == UInt16.MaxValue)
                return false;
            return c1.value != c2.value;
        }

        public static UOColor Parse(string s)
        {
            if (s.StartsWith("0x"))
                return UInt16.Parse(s.Remove(0, 2), NumberStyles.HexNumber);
            else
                return UInt16.Parse(s);
        }

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.UInt16;
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
            return value;
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(value);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(value);
        }

        #endregion

        #region IComparable<ushort> Members

        int IComparable<ushort>.CompareTo(ushort other)
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
