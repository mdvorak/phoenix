using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Phoenix.Configuration
{
    public struct LaunchData
    {
        public string ClientExe;
        public string UltimaDir;
        public string PhoenixDir;
        public string Culture;

        public string Username;
        /// <summary>
        /// Decrypted password.
        /// </summary>
        public string Password;

        public string Address;
        public string ServerKey1;
        public string ServerKey2;
        public string ServerEncryption;
        public string ServerEncName;

        public string ClientHash;

        public string LaunchEventId;

        public override string ToString()
        {
            if (ClientExe == null) ClientExe = "";
            if (UltimaDir == null) UltimaDir = "";
            if (PhoenixDir == null) PhoenixDir = "";
            if (Culture == null) Culture = "";
            if (Username == null) Username = "";
            if (Password == null) Password = "";
            if (Address == null) Address = "";
            if (ServerKey1 == null) ServerKey1 = "";
            if (ServerKey2 == null) ServerKey2 = "";
            if (ServerEncryption == null) ServerEncryption = "";
            if (ServerEncName == null) ServerEncName = "";
            if (ClientHash == null) ClientHash = "";
            if (LaunchEventId == null) LaunchEventId = "";

            string cmdLine = String.Format("\"{0}\" -ultimadir:\"{1}\" -phoenixdir:\"{2}\" -culture:\"{3}\" -clienthash:\"{4}\" -servencname:\"{5}\" -eventid:\"{6}\"",
                                            ClientExe, UltimaDir, PhoenixDir, Culture, ClientHash, ServerEncName, LaunchEventId);

            cmdLine += String.Format(" -address:\"{0}\" -serverkey1:\"{1}\" -serverkey2:\"{2}\" -serverenc:\"{3}\"",
                                      Address, ServerKey1, ServerKey2, ServerEncryption);

            cmdLine += String.Format(" -username:\"{0}\" -password:\"{1}\"", Username, Password);

            return cmdLine;
        }

        private static string ReadQuotes(string str, int index)
        {
            string quotes = null;

            if (index < 0) return null;

            while (index < str.Length)
            {
                if (quotes != null)
                {
                    if (str[index] == '"')
                    {
                        return quotes;
                    }
                    else
                    {
                        quotes += str[index];
                    }
                }
                else if (str[index] == '"')
                {
                    quotes = "";
                }

                index++;
            }

            return quotes;
        }

        public static LaunchData FromCmdLine(string cmdLine)
        {
            LaunchData info = new LaunchData();

            info.ClientExe = ReadQuotes(cmdLine, 0);
            info.UltimaDir = ReadQuotes(cmdLine, cmdLine.IndexOf("-ultimadir:"));
            info.PhoenixDir = ReadQuotes(cmdLine, cmdLine.IndexOf("-phoenixdir:"));
            info.Culture = ReadQuotes(cmdLine, cmdLine.IndexOf("-culture:"));
            info.Username = ReadQuotes(cmdLine, cmdLine.IndexOf("-username:"));
            info.Password = ReadQuotes(cmdLine, cmdLine.IndexOf("-password:"));
            info.Address = ReadQuotes(cmdLine, cmdLine.IndexOf("-address:"));
            info.ServerKey1 = ReadQuotes(cmdLine, cmdLine.IndexOf("-serverkey1:"));
            info.ServerKey2 = ReadQuotes(cmdLine, cmdLine.IndexOf("-serverkey2:"));
            info.ServerEncryption = ReadQuotes(cmdLine, cmdLine.IndexOf("-serverenc:"));
            info.ServerEncName = ReadQuotes(cmdLine, cmdLine.IndexOf("-servencname:"));
            info.ClientHash = ReadQuotes(cmdLine, cmdLine.IndexOf("-clienthash:"));
            info.LaunchEventId = ReadQuotes(cmdLine, cmdLine.IndexOf("-eventid:"));

            return info;
        }
    }
}
