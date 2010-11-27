using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Phoenix.Runtime
{
    public class UnregisteringAssemblyEventArgs : EventArgs
    {
        private Assembly assembly;

        public UnregisteringAssemblyEventArgs(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public Assembly Assembly
        {
            get { return assembly; }
        }
    }

    public delegate void UnregisteringAssemblyEventHandler(object sender, UnregisteringAssemblyEventArgs e);

    public sealed class UnregisteringAssemblyPublicEvent : PublicEvent<UnregisteringAssemblyEventHandler, UnregisteringAssemblyEventArgs>
    {
    }
}
