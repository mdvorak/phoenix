using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication
{
    /// <summary>
    /// UltimaSocket error.
    /// </summary>
    public class SocketException : Exception
    {
        private string socketDump;

        internal SocketException(UltimaSocket socket)
        {
            socketDump = socket.Dump();
        }

        internal SocketException(string message, UltimaSocket socket)
            : base(message)
        {
            socketDump = socket.Dump();
        }

        internal SocketException(string message, UltimaSocket socket, byte[] data)
            : base(message)
        {
            socketDump = socket.Dump();
            socketDump += "Current data:\n" + PacketLogging.BuildString(data);
        }

        internal SocketException(string message, UltimaSocket socket, Exception inner)
            : base(message, inner)
        {
            socketDump = socket.Dump();
        }

        internal SocketException(string message, UltimaSocket socket, byte[] data, Exception inner)
            : base(message, inner)
        {
            socketDump = socket.Dump();
            socketDump += "Current data:\n" + PacketLogging.BuildString(data);
        }

        public string SocketDump
        {
            get { return socketDump; }
        }

        public override string ToString()
        {
            string str = base.ToString();
            str += "\n";
            str += socketDump;
            return str.Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
        }
    }
}
