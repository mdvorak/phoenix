using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Phoenix.Runtime.Reflection
{
    public class Executable : Method
    {
        private ExecutableAttribute attribute;

        public Executable(MethodInfo method, Object target, ExecutableAttribute attribute)
            : base(method, target)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            this.attribute = attribute;
        }

        public override string Name
        {
            get { return attribute.Name; }
        }

        public override string ToString()
        {
            return String.Format("Executable Name=\"{0}\" Method=\"{1}.{2}()\"", Name, MethodInfo.DeclaringType, MethodInfo.Name);
        }
    }
}
