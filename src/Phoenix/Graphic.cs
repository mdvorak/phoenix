using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Phoenix
{
    [Serializable]
    public struct Graphic : IFormattable, IConvertible, IComparable<UInt16>, IComparable
    {
        public static Graphic Invariant
        {
            get { return new Graphic(UInt16.MaxValue); }
        }

        private ushort value;

        public Graphic(ushort value)
        {
            this.value = value;
        }

        public bool IsInvariant
        {
            get { return value == UInt16.MaxValue; }
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Graphic) return this == (Graphic)obj;
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

        public static implicit operator Graphic(UInt16 value)
        {
            return new Graphic(value);
        }

        public static implicit operator UInt16(Graphic graphic)
        {
            return graphic.value;
        }

        public static implicit operator Graphic(Int32 value)
        {
            return new Graphic((ushort)value);
        }

        public static explicit operator Int16(Graphic graphic)
        {
            return (short)graphic.value;
        }

        public static bool operator ==(Graphic g1, Graphic g2)
        {
            if (g1.value == UInt16.MaxValue || g2.value == UInt16.MaxValue)
                return true;
            return g1.value == g2.value;
        }

        public static bool operator !=(Graphic g1, Graphic g2)
        {
            if (g1.value == UInt16.MaxValue || g2.value == UInt16.MaxValue)
                return false;
            return g1.value != g2.value;
        }

        public static Graphic Parse(string s)
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
