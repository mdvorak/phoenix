using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Collections;
using Phoenix.Runtime;
using Phoenix.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Phoenix.Macros
{
    public class Macro : ListEx<IMacroCommand>
    {
        public Macro()
        {
        }

        public void Run()
        {
            IMacroCommand[] array;

            lock (SyncRoot) {
                array = ToArray();
            }

            foreach (IMacroCommand macroCmd in array) {
                RuntimeCore.Executions.Run(RuntimeCore.CommandList[macroCmd.TextCommand.Command], macroCmd.TextCommand.Arguments);
            }
        }
        /*
        internal string[] Serialize()
        {
            lock (SyncRoot) {
                string[] commands = new string[Count];
                BinaryFormatter f = new BinaryFormatter();

                for (int i = 0; i < commands.Length; i++) {
                    commands[i] = SerializeCommand(f, this[i]);
                }

                return commands;
            }
        }

        private string SerializeCommand(BinaryFormatter f, IMacroCommand cmd)
        {
            using (MemoryStream stream = new MemoryStream()) {
                f.Serialize(stream, cmd);
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream)) {
                    return reader.ReadToEnd();
                }
            }
        }*/
    }
}
