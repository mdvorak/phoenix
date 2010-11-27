using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Runtime.Reflection
{
    public class MethodCollection : IEnumerable<MethodOverloads>
    {
        private readonly object syncRoot;
        private Dictionary<string, MethodOverloads> list;

        public MethodCollection()
        {
            syncRoot = new object();
            list = new Dictionary<string, MethodOverloads>();
        }

        public MethodOverloads Get(string name, bool create)
        {
            if (name == null || name.Length == 0)
                throw new ArgumentNullException("name");

            // TODO: MethodOverloads are case-sensitive, MethodCollection not.
            // for current implementation it's ok but it could cause prolbems
            // when someone would like to use his own object inherited from Method.
            name = name.ToLowerInvariant();

            lock (syncRoot) {
                MethodOverloads overload;
                if (list.TryGetValue(name, out overload)) {
                    return overload;
                }

                if (create) {
                    Helper.CheckName(ref name, true);
                    overload = new MethodOverloads(name);
                    list.Add(name, overload);
                    return overload;
                }
                else {
                    throw new RuntimeException("Method of name \"" + name + "\" not found.");
                }
            }
        }

        public bool Contains(string name)
        {
            lock (syncRoot) {
                return list.ContainsKey(name.ToLowerInvariant());
            }
        }

        public MethodOverloads this[string name]
        {
            get { return Get(name, false); }
        }

        public IEnumerator<MethodOverloads> GetEnumerator()
        {
            lock (syncRoot) {
                return list.Values.GetEnumerator();
            }
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (syncRoot) {
                return list.Values.GetEnumerator();
            }
        }

        #endregion
    }
}
