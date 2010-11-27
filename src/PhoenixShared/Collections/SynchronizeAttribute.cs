using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Collections
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SynchronizeAttribute : Attribute
    {
        private bool synchronize;

        public SynchronizeAttribute()
        {
            synchronize = true;
        }

        public SynchronizeAttribute(bool synchronize)
        {
            this.synchronize = synchronize;
        }

        public bool Synchronize
        {
            get { return synchronize; }
        }
    }
}
