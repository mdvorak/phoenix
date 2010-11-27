using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Phoenix.Runtime
{
    public static class Helper
    {
        /// <summary>
        /// Returns validated name or throws exception.
        /// </summary>
        /// <param name="name">String to validate.</param>
        /// <returns>Validated lower-case name.</returns>
        /// <exception cref="Phoenix.Runtime.RuntimeException">Name containes some invalid characters.</exception>
        public static string CheckName(string name)
        {
            CheckName(ref name, true);
            return name;
        }

        /// <exception cref="Phoenix.Runtime.RuntimeException">Name containes some invalid characters.</exception>
        public static bool CheckName(ref string name, bool throwException)
        {
            if (name != null)
            {
                name = name.ToLowerInvariant();

                Regex regex = new Regex(@"\A(?:[a-z0-9]|\x5F)+\z");
                if (regex.IsMatch(name))
                    return true;
            }

            if (throwException)
                throw new RuntimeException("Name containes some invalid characters.");
            else
                return false;
        }

        public static FileInfo GetAssemblyFileInfo(System.Reflection.Assembly assembly)
        {
            return new FileInfo(new Uri(assembly.CodeBase).LocalPath);
        }
    }
}
