using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Runtime
{
    public class RuntimeException : Exception
    {
        public RuntimeException()
        {
        }

        public RuntimeException(string message)
            : base(message)
        {
        }

        public RuntimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
