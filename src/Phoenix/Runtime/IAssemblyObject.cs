using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Phoenix.Runtime
{
    internal interface IAssemblyObject
    {
        Assembly Assembly { get; }
    }

    internal interface IAssemblyObjectList
    {
        /// <summary>
        /// Don't use it directly, called only by RuntimeCore.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        void Remove(IAssemblyObject obj);
    }
}
