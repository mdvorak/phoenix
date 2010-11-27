using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Phoenix.Runtime
{
    static class AssemblyDisposer
    {
        public static void ClearAssembly(Assembly a)
        {
            Type[] types = a.GetTypes();

            foreach (Type t in types)
            {
                FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                foreach (FieldInfo f in fields)
                {
                    if (!f.IsInitOnly && !f.IsLiteral)
                    {
                        try
                        {
                            if (f.FieldType.IsValueType)
                            {
                                f.SetValue(null, Activator.CreateInstance(f.FieldType));
                            }
                            else
                            {
                                f.SetValue(null, null);
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Error freeing assembly variables: " + e.Message, "Runtime");
                        }
                    }
                }
            }
        }
    }
}
