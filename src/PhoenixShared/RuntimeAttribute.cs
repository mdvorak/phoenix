using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Phoenix
{
    public abstract class RuntimeAttribute : Attribute
    {
        /// <summary>
        /// Called during type analyzation.
        /// </summary>
        /// <param name="mi">Member which is this attribute used on.</param>
        /// <param name="target">Instance of target object for non-static methods; otherwise null.</param>
        /// <returns>String containing result information or null.</returns>
        protected abstract string Register(MemberInfo mi, Object target);
    }
}
