using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using Phoenix.Runtime.Reflection;

namespace Phoenix.Runtime
{
    public class ExecutionInfo : IAssemblyObject
    {
        private Method method;
        private Thread thread;

        internal ExecutionInfo(Method method)
        {
            this.method = method;
            this.thread = Thread.CurrentThread;
        }

        public Method Method
        {
            get { return method; }
        }

        public Thread Thread
        {
            get { return thread; }
        }

        public int Id
        {
            get { return thread.ManagedThreadId; }
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", Id, method.Name);
        }

        public Assembly Assembly
        {
            get { return method.Assembly; }
        }
    }
}
