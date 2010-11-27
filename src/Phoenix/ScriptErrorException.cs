using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    public class ScriptErrorException : Exception
    {
        [ThreadStatic]
        private static bool exceptionEnabled = true;

        public static bool ExceptionEnabled
        {
            get { return ScriptErrorException.exceptionEnabled; }
            set { ScriptErrorException.exceptionEnabled = value; }
        }

        internal static void ThreadInit()
        {
            exceptionEnabled = true;
        }

        public ScriptErrorException(string message)
            : base(message)
        {
        }

        public ScriptErrorException(string format, params object[] args)
            : base(String.Format(format, args))
        {
        }

        /// <summary>
        /// Throws exception or writes error and returns false.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Always false.</returns>
        public static bool Throw(string message)
        {
            if (exceptionEnabled)
                throw new ScriptErrorException(message);
            else
            {
                UO.PrintError(message);
                return false;
            }
        }

        /// <summary>
        /// Throws exception or writes error and returns false.
        /// </summary>
        /// <param name="format">Error message format string. See <see cref="System.String.Format"/> for details.</param>
        /// <param name="args">Format string arguments</param>
        /// <returns>Always false.</returns>
        public static bool Throw(string format, params object[] args)
        {
            if (exceptionEnabled)
                throw new ScriptErrorException(format, args);
            else
            {
                UO.PrintError(format, args);
                return false;
            }
        }
    }
}
