using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Runtime
{
    public class ExecutionBlockedException : RuntimeException
    {
        public ExecutionBlockedException()
            : base("Multiple executions of this method are not allowed.")
        {
        }
    }
}
