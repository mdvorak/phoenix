using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using Phoenix.Runtime;
using Phoenix.Runtime.Reflection;

namespace Phoenix
{
    /// <summary>
    /// Blocks more than one executions of function at same time.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class BlockMultipleExecutionsAttribute : ExecutionAttribute
    {
        private static Hashtable blockList = Hashtable.Synchronized(new Hashtable());
        private static AutoResetEvent listChanged = new AutoResetEvent(false);

        private string group;

        public BlockMultipleExecutionsAttribute()
        {
            group = null;
        }

        public BlockMultipleExecutionsAttribute(string group)
        {
            this.group = group;
        }

        public string Group
        {
            get { return group; }
        }

        private string GetGroupInternal(Method m)
        {
            return group != null ? group : m.Name;
        }

        protected internal override void Starting(Method m)
        {
            Debug.Assert(m != null, "Parameter null.");

            string name = GetGroupInternal(m);

            if (blockList.ContainsValue(name)) {
                throw new ExecutionBlockedException();
            }

            blockList.Add(Thread.CurrentThread.ManagedThreadId, name);
        }

        protected internal override void Finished(Method m)
        {
            Debug.Assert(m != null, "Parameter null.");

            blockList.Remove(Thread.CurrentThread.ManagedThreadId);
        }
    }
}
