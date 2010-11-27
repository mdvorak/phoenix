using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Phoenix
{/*
    [Flags]
    public enum AutoStartFlags
    {
        Loaded,
        Unloading
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class AutoStartAttribute : RuntimeAttribute
    {
        private AutoStartFlags flags;

        public AutoStartAttribute(AutoStartFlags flags)
        {
            this.flags = flags;
        }

        public AutoStartFlags Flags
        {
            get { return flags; }
        }

        public bool IsSetLoaded
        {
            get { return (flags & AutoStartFlags.Loaded) != 0; }
        }

        public bool IsSetUnloading
        {
            get { return (flags & AutoStartFlags.Unloading) != 0; }
        }

        protected override string Register(MemberInfo mi, object target)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }*/
}
