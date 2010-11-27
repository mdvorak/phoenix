using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Phoenix.Runtime
{
    public sealed class TextCommand
    {
        private static readonly Regex regex = new Regex(@"\x22(?<Arg>.*?)\x22|(?<Arg>\S+)", RegexOptions.Compiled);

        private string fullCommand;
        private string command;
        private object[] arguments;

        private TextCommand(string cmdLine)
        {
            if (cmdLine == null || cmdLine.Length == 0)
                throw new ArgumentException("No command specified.");

            fullCommand = cmdLine;

            List<object> list = new List<object>();

            MatchCollection matches = regex.Matches(cmdLine);

            foreach (Match match in matches) {
                if (match.Groups["Arg"].Success)
                    list.Add(match.Groups["Arg"].Value);
            }

            command = Helper.CheckName(list[0].ToString().TrimStart(','));
            arguments = new object[list.Count - 1];
            list.CopyTo(1, arguments, 0, list.Count - 1);
        }

        public TextCommand(string command, params object[] args)
        {
            this.command = Helper.CheckName(command.TrimStart(','));
            arguments = args;

            fullCommand = this.command;

            for (int i = 0; i < arguments.Length; i++) {
                if (arguments[i] == null) {
                    Array.Resize<object>(ref arguments, i);
                    break;
                }

                string argString = arguments[i].ToString();

                if (argString.Contains(" "))
                    fullCommand += String.Format(" \"{0}\"", argString);
                else
                    fullCommand += " " + argString;
            }
        }

        public string Command
        {
            get { return command; }
        }

        public object[] Arguments
        {
            get { return arguments; }
        }

        public override int GetHashCode()
        {
            return command.GetHashCode();
        }

        // TODO: Equals
        /*
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == GetType()) {
                TextCommand cmd = (TextCommand)obj;
                if (command == cmd.command && arguments.Length == cmd.arguments.Length) {
                    for (int i = 0; i < arguments.Length; i++) {
                        if(arguments[0] != cmd.arguments.Length
                    }
                }
            }

            return false;
        }*/

        public override string ToString()
        {
            return fullCommand;
        }

        public static TextCommand Parse(string cmdLine)
        {
            return new TextCommand(cmdLine);
        }
    }
}
