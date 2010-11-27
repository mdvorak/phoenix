using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace Phoenix.Runtime.Reflection
{
    public class MethodOverloads : ICollection<Method>, IAssemblyObjectList
    {
        private readonly object syncRoot;
        private List<Method> overloads;
        private string name;
        private string[] syntax;

        public MethodOverloads(string name)
        {
            syncRoot = new object();
            overloads = new List<Method>();
            this.name = name;
            syntax = null;
        }

        public string Name
        {
            get { return name; }
        }

        public string[] Syntax
        {
            get
            {
                lock (syncRoot)
                {
                    if (syntax == null)
                        syntax = SyntaxBuilder.Build(name, overloads);

                    return syntax;
                }
            }
        }


        public Method[] FindOverloads(ParameterData[] parameters)
        {
            lock (syncRoot)
            {
                // First get valid overloads
                List<Method> valid = new List<Method>();

                foreach (Method method in overloads)
                {
                    if (method.MinParameterCount <= parameters.Length && method.MaxParameterCount >= parameters.Length)
                    {
                        bool invalid = false;
                        TypeClass paramClass = TypeClass.Object;

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (i < method.Parameters.Length)
                            {
                                if (method.Parameters[i].IsParamArray)
                                {
                                    Debug.Assert(i == method.Parameters.Length - 1);
                                    paramClass = TypeHelper.GetTypeClass(method.Parameters[i].Type.GetElementType());
                                }
                                else
                                {
                                    paramClass = method.Parameters[i].Class;
                                }
                            }

                            if (paramClass < parameters[i].Class)
                            {
                                invalid = true;
                                break;
                            }
                        }

                        if (!invalid)
                        {
                            valid.Add(method);
                        }
                    }
                }

                valid.Sort(new Comparison<Method>(MethodComparer));

                if (valid.Count > 0)
                {
                    return valid.ToArray();
                }
                else
                {
                    throw new RuntimeException("No suitable method overload found.");
                }
            }
        }

        public void Add(Method item)
        {
            lock (syncRoot)
            {
                if (item == null)
                    throw new ArgumentNullException("item");

                if (item.Name != name)
                    throw new ArgumentException("Cannot add overload with different name.");

                if (overloads.Count > 0)
                {
                    if (item.Assembly != overloads[0].Assembly || item.MethodInfo.DeclaringType != overloads[0].MethodInfo.DeclaringType)
                    {
                        throw new ArgumentException("Cannot add overload from different assembly or type.");
                    }

                    if (item.MethodInfo.IsStatic != overloads[0].MethodInfo.IsStatic)
                    {
                        throw new ArgumentException("Cannot combine static and non-static methods.");
                    }
                }

                foreach (Method method in overloads)
                {
                    if (method.MinParameterCount > item.MaxParameterCount || method.MaxParameterCount < item.MinParameterCount)
                    {
                        continue;
                    }
                    else
                    {
                        bool different = false;

                        for (int i = 0; i < item.Parameters.Length; i++)
                        {
                            // TODO
                            different = true;
                        }

                        if (!different)
                        {
                            throw new RuntimeException(String.Format("Cannot add overload. Ambiguous method call with {0}", method));
                        }
                    }
                }

                RuntimeCore.AddAssemblyObject(item, this);
                overloads.Add(item);
                syntax = null;
            }
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                foreach (Method m in overloads)
                {
                    RuntimeCore.RemoveAssemblyObject(m);
                }

                overloads.Clear();
                syntax = null;
            }
        }

        public bool Contains(Method item)
        {
            lock (syncRoot)
            {
                return overloads.Contains(item);
            }
        }

        public int Count
        {
            get
            {
                lock (syncRoot)
                {
                    return overloads.Count;
                }
            }
        }

        public bool Remove(Method item)
        {
            lock (syncRoot)
            {
                bool b = overloads.Remove(item);
                RuntimeCore.RemoveAssemblyObject(item);
                syntax = null;
                return b;
            }
        }

        public IEnumerator<Method> GetEnumerator()
        {
            return overloads.GetEnumerator();
        }

        #region ICollection<Method> Members

        void ICollection<Method>.CopyTo(Method[] array, int arrayIndex)
        {
            lock (syncRoot)
            {
                overloads.CopyTo(array, arrayIndex);
            }
        }

        bool ICollection<Method>.IsReadOnly
        {
            get { return false; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return overloads.GetEnumerator();
        }

        #endregion

        #region IAssemblyObjectList Members

        void IAssemblyObjectList.Remove(IAssemblyObject obj)
        {
            lock (syncRoot)
            {
                Debug.Assert(obj is Method);
                overloads.Remove((Method)obj);
            }
        }

        #endregion

        private static int MethodComparer(Method m1, Method m2)
        {
            // TODO
            InputParameter[] p1 = m1.Parameters;
            InputParameter[] p2 = m2.Parameters;

            for (int i = 0; i < p1.Length && i < p2.Length; i++)
            {
                if (p1[i].Class != p2[i].Class)
                    return (int)(p1[i].Class - p2[i].Class);
            }

            return m1.MinParameterCount - m2.MinParameterCount;
        }
    }
}
