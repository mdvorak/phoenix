using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ComInterop
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("D0EB0689-15B4-4a27-8800-D0DF2A89DDF1")]
    public interface IWinSock
    {
        int recv(long s, [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] buf, int len, int flags);
        int send(long s, [In] [MarshalAs(UnmanagedType.LPArray)] byte[] buf, int len, int flags);
        int closesocket(long s);

        int SimulateCursorPos(int x, int y);
        int OverrideCursor();
        int ReleaseCursorOverride();
    }
}
