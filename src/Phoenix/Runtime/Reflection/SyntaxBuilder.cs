using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Runtime.Reflection
{
    static class SyntaxBuilder
    {
        public static string[] Build(string name, List<Method> overloads)
        {
            // TODO

            List<string> list = new List<string>();

            foreach (Method method in overloads)
            {
                string s = method.Name;

                bool optional = false;

                for (int i = 0; i < method.Parameters.Length; i++)
                {
                    if (method.Parameters[i].IsParamArray)
                        s += String.Format(" [{0}...]", method.Parameters[i].ParameterInfo.Name);
                    else if (optional || method.Parameters[i].IsOptional || list.Contains(s))
                    {
                        list.Remove(s);
                        s += String.Format(" [{0}]", method.Parameters[i].ParameterInfo.Name);
                        optional = true;
                    }
                    else
                        s += " " + method.Parameters[i].ParameterInfo.Name;
                }

                list.Add(s);
            }

            return list.ToArray();
        }
    }
}
