using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace Phoenix.Runtime.Reflection
{
    public class ParameterData
    {
        private object source;
        private string valueString;
        private long number;
        private TypeClass dataClass;

        public ParameterData(object value)
        {
            source = value;
            dataClass = TypeClass.Object;

            if (value != null)
            {
                valueString = value.ToString();
                dataClass = TypeHelper.GetValueClass(value, out number);
            }
        }

        public bool IsNull
        {
            get { return source == null; }
        }

        public object Source
        {
            get { return source; }
        }

        public string String
        {
            get { return valueString; }
        }

        public long Number
        {
            get
            {
                if (dataClass != TypeClass.Number)
                    throw new RuntimeException("Cannot get numeric value for non-numeric object value.");
                else
                    return number;
            }
        }

        public TypeClass Class
        {
            get { return dataClass; }
        }

        public object ConvertTo(InputParameter targetParam)
        {
            return ConvertTo(targetParam.Type, targetParam.Class);
        }

        public object ConvertTo(Type targetType, TypeClass targetClass)
        {
            try
            {
                // Check null value
                if (this.IsNull)
                {
                    if (!targetType.IsValueType)
                        return null;
                    else
                        throw new ParameterException("Cannot convert null to ValueType.");
                }

                // Check if it isn't already in appropriate type
                if (targetType == this.Source.GetType())
                {
                    return this.Source;
                }

                // If string is requested return already known value
                if (targetType == typeof(String))
                {
                    return this.String;
                }

                // Convert numbers
                if (targetClass == TypeClass.Number)
                {
                    if (this.String.Length > 0)
                    {
                        // Try to resolve alias
                        ParameterData aliasData = null;

                        if (targetType == typeof(uint) || targetType == typeof(int) || targetType == typeof(Serial))
                        {
                            if (Aliases.ObjectExists(this.String))
                            {
                                aliasData = new ParameterData(Aliases.GetObject(this.String));
                            }
                        }

                        if (aliasData != null)
                        {
                            return aliasData.ConvertTo(targetType, targetClass);
                        }
                    }

                    // If requested type is one of built-in use implemented converter
                    if (targetType.IsPrimitive)
                    {
                        return Convert.ChangeType(this.Number, targetType);
                    }
                    else if (targetType == typeof(Serial))
                    {
                        return (Serial)(uint)this.Number;
                    }
                    else if (targetType == typeof(Graphic))
                    {
                        return (Graphic)(ushort)this.Number;
                    }
                    else if (targetType == typeof(UOColor))
                    {
                        return (UOColor)(ushort)this.Number;
                    }
                    else
                    {
                        throw new InternalErrorException("Unknown numeric type requested.");
                    }
                }

                // Unimplemented type
                if (targetClass == TypeClass.Object)
                {
                    // Try enum
                    if (targetType.IsEnum)
                    {
                        try
                        {
                            return Enum.Parse(targetType, this.String, true);
                        }
                        catch { }
                    }

                    // Try IConvertible
                    if (this.Source is IConvertible)
                    {
                        try
                        {
                            return ((IConvertible)this.Source).ToType(targetType, null);
                        }
                        catch { }
                    }

                    // Try to find operators
                    List<MethodInfo> methods = new List<MethodInfo>();
                    methods.AddRange(targetType.GetMethods(BindingFlags.Public | BindingFlags.Static));
                    methods.AddRange(this.Source.GetType().GetMethods(BindingFlags.Public | BindingFlags.Static));
                    for (int i = 0; i < methods.Count; i++)
                    {
                        try
                        {
                            if (methods[i].Name == "op_Implicit" || methods[i].Name == "op_Explicit")
                            {
                                if (methods[i].ReturnType == targetType)
                                {
                                    ParameterInfo[] parameters = methods[i].GetParameters();
                                    if (parameters.Length == 1 && parameters[0].ParameterType == this.Source.GetType())
                                    {
                                        return methods[i].Invoke(null, new object[] { this.Source });
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    // Try to find Parse function
                    MethodInfo parseMethod = targetType.GetMethod("Parse", new Type[] { typeof(String) });
                    if (parseMethod != null)
                    {
                        try
                        {
                            return parseMethod.Invoke(null, new object[] { this.String });
                        }
                        catch { }
                    }

                    throw new ParameterException("No suitable convert method found.");
                }
                else
                {
                    throw new InternalErrorException("Invalid targetClass specified.");
                }
            }
            catch (Exception e)
            {
                string valueString = this.Source != null ? this.String : "null";
                if (valueString.Contains(" "))
                    valueString = "\"" + valueString + "\"";

                throw new ParameterException("Unable to convert " + valueString + " to " + targetType.ToString(), e);
            }
        }
    }
}
