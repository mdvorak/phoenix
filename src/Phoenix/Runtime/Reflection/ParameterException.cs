using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Runtime.Reflection
{
    class ParameterException : RuntimeException
    {
        public ParameterException()
        {
        }

        public ParameterException(string message)
            : base(message)
        {
        }

        public ParameterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
