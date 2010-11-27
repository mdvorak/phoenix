using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Phoenix.Gui;

namespace Phoenix
{
    public static class Client
    {
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool SetWindowText([In] IntPtr hWnd, [In] string lpString);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText([In] IntPtr hWnd, [In, Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, [In] int nMaxCount);

        private static IntPtr hwnd;
        private static string text;
        private static bool focus;
        private static DefaultPublicEvent textChanged = new DefaultPublicEvent();
        private static DefaultPublicEvent windowFocusChanged = new DefaultPublicEvent();
        private static ClientWindow window = new ClientWindow();

        internal static void OnWindowCreated(IntPtr hwnd)
        {
            Debug.Assert(Client.hwnd == IntPtr.Zero, "hwnd == IntPtr.Zero");

            Client.hwnd = hwnd;

            StringBuilder sb = new StringBuilder(1024);
            int count = GetWindowText(hwnd, sb, sb.Capacity);
            // Debug.Assert(count > 0, "GetWindowText(hwnd, sb, sb.MaxCapacity)");
            text = sb.ToString();

            window.AssignHandle(hwnd);

            Trace.WriteLine(String.Format("Client window created. HWND=0x{0} Text={1}", hwnd.ToString("X8"), text), "Native");
        }

        internal static void OnTextChanged(string text)
        {
            Client.text = text;
            Debug.WriteLine("Client window text changed to: " + text, "Native");
            textChanged.BeginInvoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Gets client window handle.
        /// </summary>
        public static IntPtr HWND
        {
            get { return hwnd; }
        }


        public static ClientWindow Window
        {
            get { return Client.window; }
        }

        /// <summary>
        /// Gets or sets client capiton text.
        /// </summary>
        public static string Text
        {
            get { return text; }
            set
            {
                bool result = SetWindowText(hwnd, value);
                Debug.Assert(result, "SetWindowText(hwnd, value)");
            }
        }

        public static bool WindowFocus
        {
            get { return focus; }
            internal set
            {
                if (value != focus) {
                    focus = value;
                    windowFocusChanged.InvokeAsync(null, EventArgs.Empty);
                }
            }
        }

        public static event EventHandler TextChanged
        {
            add { textChanged.AddHandler(value); }
            remove { textChanged.RemoveHandler(value); }
        }

        public static event EventHandler WindowFocusChanged
        {
            add { windowFocusChanged.AddHandler(value); }
            remove { windowFocusChanged.RemoveHandler(value); }
        }

        /// <summary>
        /// Posts message to client.
        /// </summary>
        /// <param name="Msg">Message id.</param>
        /// <param name="wParam">Message parameter.</param>
        /// <param name="lParam">Message parameter.</param>
        /// <returns>True if message has been sent; otherwise false.</returns>
        public static bool PostMessage(int Msg, int wParam, int lParam)
        {
            return PostMessage(hwnd, Msg, wParam, lParam);
        }
    }
}
