using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Phoenix.Runtime.Reflection;

namespace Phoenix
{
    public abstract class ExecutionAttribute : Attribute
    {
        /// <summary>
        /// Called before execution is started.
        /// </summary>
        /// <param name="mi"></param>
        /// <param name="target"></param>
        protected internal abstract void Starting(Method m);
        /// <summary>
        /// Called when execution finished.
        /// </summary>
        /// <param name="mi"></param>
        /// <param name="target"></param>
        protected internal abstract void Finished(Method m);
    }
}
