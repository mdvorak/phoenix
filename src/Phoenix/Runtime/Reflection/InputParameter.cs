using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace Phoenix.Runtime.Reflection
{
    public class InputParameter
    {
        private ParameterInfo info;
        private TypeClass paramClass;

        public InputParameter(ParameterInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            this.info = info;
            paramClass = TypeHelper.GetTypeClass(info.ParameterType);
        }

        public bool IsOptional
        {
            get
            {
                return info.IsOptional || info.IsDefined(typeof(OptionalParameterAttribute), false);
            }
        }

        public bool IsParamArray
        {
            get { return info.IsDefined(typeof(ParamArrayAttribute), true); }
        }

        public Type Type
        {
            get { return info.ParameterType; }
        }

        public TypeClass Class
        {
            get { return paramClass; }
        }

        public ParameterInfo ParameterInfo
        {
            get { return info; }
        }
    }
}
