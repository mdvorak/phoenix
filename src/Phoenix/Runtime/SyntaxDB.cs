using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.Runtime.Reflection;

namespace Phoenix.Runtime
{
    public class MethodSyntax
    {
        private string name;
        private string[] syntaxList;
        private List<MethodSyntax> methods;

        internal MethodSyntax(string name, string[] syntaxList)
        {
            this.name = name;
            this.syntaxList = syntaxList;
            methods = new List<MethodSyntax>();
        }

        public string Name
        {
            get { return name; }
        }

        public string[] SyntaxList
        {
            get { return syntaxList; }
        }
    }

    public static class SyntaxDB
    {
        private static List<MethodSyntax> list = new List<MethodSyntax>();

        public static void Update()
        {
            try
            {
                list.Clear();

                foreach (MethodOverloads command in RuntimeCore.CommandList)
                {
                    MethodSyntax syntax = new MethodSyntax(command.Name, command.Syntax);
                    list.Add(syntax);
                }

                Trace.WriteLine("SyntaxDB updated.", "Runtime");
            }
            catch (Exception e)
            {
                Trace.WriteLine("Error updating SyntaxDB. Exception:\n" + e.ToString(), "Runtime");
            }
        }
    }
}
