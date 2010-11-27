using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Phoenix.Runtime;
using Phoenix.Runtime.Reflection;

namespace Phoenix
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CommandAttribute : RuntimeAttribute
    {
        private string name;

        public CommandAttribute()
        {
            name = null;
        }

        public CommandAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets command name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        protected override string Register(MemberInfo mi, object target)
        {
            if (name == null) name = Helper.CheckName(mi.Name);
            else name = Helper.CheckName(name);

            RuntimeCore.CommandList.Get(name, true).Add(new Command((MethodInfo)mi, target, this));
            return String.Format("Command '{0}' registered.", name);
        }
    }
}
