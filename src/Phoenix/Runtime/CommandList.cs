using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Runtime;

namespace Phoenix.Runtime
{
    internal class CommandList
    {
        private string text;
        private TextCommand[] list;

        public CommandList(string text)
        {
            string cmdLine = "";

            string[] commands = text.Replace("\r", "").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<TextCommand> commandList = new List<TextCommand>(commands.Length);

            foreach (string command in commands) {
                if (command.Length > 0) {
                    cmdLine += command + "\n";
                    commandList.Add(TextCommand.Parse(command));
                }
            }

            if (cmdLine.Contains("\n"))
                cmdLine = cmdLine.Remove(cmdLine.LastIndexOf('\n'));

            list = commandList.ToArray();
            this.text = cmdLine;
        }

        public string Text
        {
            get { return text; }
        }

        public TextCommand[] List
        {
            get { return list; }
        }

        internal void Run()
        {
            foreach (TextCommand cmd in list) {
                RuntimeCore.Executions.Run(RuntimeCore.CommandList[cmd.Command], cmd.Arguments);
            }
        }
    }
}
