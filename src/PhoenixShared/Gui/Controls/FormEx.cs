using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Phoenix.Gui.Controls
{
    public class FormEx : Form
    {
        #region Native functions

        protected static class Native
        {
            [Flags]
            public enum SetWindowPosFlags : uint
            {
                SWP_NOSIZE = 0x0001,
                SWP_NOMOVE = 0x0002,
                SWP_NOZORDER = 0x0004,
                SWP_NOREDRAW = 0x0008,
                SWP_NOACTIVATE = 0x0010,
                /// <summary>The frame changed: send WM_NCCALCSIZE</summary>
                SWP_FRAMECHANGED = 0x0020,
                SWP_SHOWWINDOW = 0x0040,
                SWP_HIDEWINDOW = 0x0080,
                SWP_NOCOPYBITS = 0x0100,
                /// <summary>Don't do owner Z ordering</summary>
                SWP_NOOWNERZORDER = 0x0200,
                /// <summary>Don't send WM_WINDOWPOSCHANGING</summary>
                SWP_NOSENDCHANGING = 0x0400,

                SWP_DRAWFRAME = SWP_FRAMECHANGED,
                SWP_NOREPOSITION = SWP_NOOWNERZORDER,
                SWP_DEFERERASE = 0x2000,
                SWP_ASYNCWINDOWPOS = 0x4000
            }


            public const int HWND_TOP = 0;
            public const int HWND_BOTTOM = 1;
            public const int HWND_TOPMOST = -1;
            public const int HWND_NOTOPMOST = -2;

            public const uint MF_BYCOMMAND = 0x00000000;
            public const uint MF_BYPOSITION = 0x00000400;

            public const uint MF_ENABLED = 0x00000000;
            public const uint MF_GRAYED = 0x00000001;
            public const uint MF_DISABLED = 0x00000002;

            [DllImport("user32.dll")]
            public extern static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

            [DllImport("user32.dll")]
            public extern static bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);
            [DllImport("user32.dll")]
            public extern static bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
            [DllImport("user32.dll")]
            public extern static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        }

        #endregion

        private FormWindowState lastWindowState = FormWindowState.Normal;
        private bool closeButtonLastState = true;

        private bool safeIsHandleCreated = false;
        private Point safeLocation;
        private Size safeSize;

        public FormEx()
        {
            safeLocation = Location;
            safeSize = Size;
        }

        [Browsable(true)]
        [Category("Property Changed")]
        public event EventHandler WindowStateChanged;

        /// <summary>
        /// Enables or disables Close button of the window.
        /// </summary>
        [Browsable(true)]
        [Category("Window Style")]
        [DefaultValue(true)]
        public bool CloseButton
        {
            get { return closeButtonLastState; }
            set
            {
                if (value != closeButtonLastState)
                {
                    if (!value)
                    {
                        IntPtr hSysMenu = Native.GetSystemMenu(Handle, false);
                        Native.DeleteMenu(hSysMenu, 6, Native.MF_BYPOSITION);
                        Native.DeleteMenu(hSysMenu, 5, Native.MF_BYPOSITION);
                    }
                    else
                    {
                        Native.GetSystemMenu(Handle, true);
                    }

                    closeButtonLastState = value;
                }
            }
        }

        public Point SafeLocation
        {
            get { return safeLocation; }
        }

        public Size SafeSize
        {
            get { return safeSize; }
        }

        /// <summary>
        /// Gets whether window handle is created.
        /// </summary>
        public bool SafeIsHandleCreated
        {
            get { return safeIsHandleCreated; }
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            // .NET bugfix (when window is minimized location is set to -32000;-32000)
            if (Location.X > -32000 && Location.Y > -32000)
                safeLocation = Location;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // when window is minimized size set to SystemInformation.MinimizedWindowSize
            if (Size != SystemInformation.MinimizedWindowSize)
                safeSize = Size;
        }

        protected virtual void OnWindowStateChanged(EventArgs e)
        {
            SyncEvent.Invoke(WindowStateChanged, this, e);
        }

        /// <summary>
        /// Sets TopMost status without activating window.
        /// </summary>
        /// <param name="value">New TopMost value.</param>
        public void SetTopMost(bool value)
        {
            IntPtr insertAfter = (IntPtr)(value ? Native.HWND_TOPMOST : Native.HWND_NOTOPMOST);
            Native.SetWindowPos(Handle, insertAfter, 0, 0, 0, 0,
                Native.SetWindowPosFlags.SWP_NOMOVE | Native.SetWindowPosFlags.SWP_NOSIZE |
                Native.SetWindowPosFlags.SWP_NOACTIVATE | Native.SetWindowPosFlags.SWP_NOREPOSITION);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (WindowState != lastWindowState)
            {
                lastWindowState = WindowState;
                OnWindowStateChanged(EventArgs.Empty);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            safeIsHandleCreated = true;
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            safeIsHandleCreated = false;
            base.OnHandleDestroyed(e);
        }

        [DllImport("user32.dll")]
        public extern static bool LockWindowUpdate(IntPtr hWnd);

        public void CreateWindow()
        {
            Show();
            CreateControl();
            Hide();
        }
    }
}
