using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace Phoenix.Utils
{
    public static class NativeTimer
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern uint timeGetTime();
    }
}
