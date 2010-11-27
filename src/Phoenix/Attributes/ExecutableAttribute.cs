using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Phoenix.Runtime;
using Phoenix.Runtime.Reflection;

namespace Phoenix
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ExecutableAttribute : RuntimeAttribute
    {
        private string name;

        public ExecutableAttribute()
        {
            name = null;
        }

        public ExecutableAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets executable name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        protected override string Register(MemberInfo mi, object target)
        {
            if (name == null) name = Helper.CheckName(mi.Name);
            else name = Helper.CheckName(name);

            RuntimeCore.ExecutableList.Get(name, true).Add(new Executable((MethodInfo)mi, target, this));
            return String.Format("Executable '{0}' registered.", name);
        }
    }
}
