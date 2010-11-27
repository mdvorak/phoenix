using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Drawing;

namespace Phoenix.Gui
{
    public sealed class ClientWindow : NativeWindow
    {
        #region Native methods

        const int WM_NULL = 0x0000;
        const int WM_MOUSELEAVE = 0x2A3;
        const int WM_MOUSEMOVE = 0x200;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        #endregion

        private Queue<ThreadStart> queue = new Queue<ThreadStart>();

        public void PostMessage(int msg, int wParam, int lParam)
        {
            PostMessage(Handle, msg, wParam, lParam);
        }

        public void SendMessage(int msg, int wParam, int lParam)
        {
            if (Thread.CurrentThread.ManagedThreadId != Core.ClientThreadId)
                throw new InvalidOperationException("Called from invalid thread.");

            SendMessage(Handle, msg, wParam, lParam);
        }

        public void BeginInvoke(ThreadStart callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            lock (queue) {
                queue.Enqueue(callback);
            }

            PostMessage(WM_NULL, 0, 0);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg) {

                case WM_NULL:
                    lock (queue) {
                        if (queue.Count > 0) {
                            try {
                                // Invoke
                                queue.Dequeue().Invoke();
                            }
                            catch (Exception e) {
                                Trace.WriteLine("Unhandled exception in the callback invoked on client thread.\n" + e.ToString(), "Gui");
                            }
                        }
                    }
                    break;
            }

            base.WndProc(ref m);
        }
    }
}
