using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

namespace Phoenix.Communication
{
    public static class WinSock
    {
        private static ComInterop.IWinSock winSock = null;
        private static int threadId = -1;

        internal static void Initialize(ComInterop.IWinSock winSock)
        {
            WinSock.winSock = winSock;
            threadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static int recv(long socket, byte[] data, int len, int flags)
        {
            if (threadId != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("Called from invalid thread.");

            return winSock.recv(socket, data, len, flags);
        }

        public static int send(long socket, byte[] data, int len, int flags)
        {
            if (threadId != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("Called from invalid thread.");

            return winSock.send(socket, data, len, flags);
        }

        public static int closesocket(long socket)
        {
            return winSock.closesocket(socket);
        }

        public static void SimulateCursorPos(int x, int y)
        {
            winSock.SimulateCursorPos(x, y);
        }

        public static void OverrideCursor()
        {
            winSock.OverrideCursor();
        }

        public static void ReleaseCursorOverride()
        {
            winSock.ReleaseCursorOverride();
        }
    }
}
