using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace Phoenix.Runtime.Reflection
{
    public static class TypeHelper
    {
        private static List<Type> numericTypes;

        static TypeHelper()
        {
            numericTypes = new List<Type>(16);

            numericTypes.Add(typeof(SByte));
            numericTypes.Add(typeof(Byte));
            numericTypes.Add(typeof(Int16));
            numericTypes.Add(typeof(UInt16));
            numericTypes.Add(typeof(Int32));
            numericTypes.Add(typeof(UInt32));
            numericTypes.Add(typeof(Int64));
            numericTypes.Add(typeof(UInt64)); // TODO: i use long as value holder
            numericTypes.Add(typeof(IntPtr));
            numericTypes.Add(typeof(UIntPtr));

            numericTypes.Add(typeof(Serial));
            numericTypes.Add(typeof(Graphic));
            numericTypes.Add(typeof(UOColor));
        }

        public static TypeClass GetTypeClass(Type t)
        {
            if (numericTypes.Contains(t))
                return TypeClass.Number;
            else
                return TypeClass.Object;
        }

        public static TypeClass GetValueClass(object value, out long numValue)
        {
            if (value == null)
            {
                numValue = 0;
                return TypeClass.Object;
            }
            else if (Aliases.ObjectExists(value.ToString()))
            {
                numValue = 0;
                return TypeClass.Number;
            }
            else if (numericTypes.Contains(value.GetType()))
            {
                numValue = Convert.ToInt64(value);
                return TypeClass.Number;
            }
            else
            {
                string str = value.ToString().Trim();

                if ((str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) &&
                    Int64.TryParse(str.Remove(0, 2), System.Globalization.NumberStyles.HexNumber, null, out numValue)) ||
                    Int64.TryParse(str, out numValue))
                {
                    return TypeClass.Number;
                }
                else
                {
                    numValue = 0;
                    return TypeClass.Object;
                }
            }
        }
    }
}
