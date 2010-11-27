using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    /// <summary>
    /// This attribute doesn't register anything, only thing it does is that it forces runtime to create class instance.
    /// </summary>
    /// <remarks>Class must have parameterless constructor.</remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class RuntimeObjectAttribute : RuntimeAttribute
    {
        protected override string Register(System.Reflection.MemberInfo mi, object target)
        {
            return null;
        }
    }
}
