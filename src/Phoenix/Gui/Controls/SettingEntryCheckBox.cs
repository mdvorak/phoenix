using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using Phoenix.Configuration;

namespace Phoenix.Gui.Controls
{
    [ToolboxBitmap(typeof(CheckBox))]
    public class SettingEntryCheckBox : CheckBox
    {
        private const int WM_APP = 0x8000;
        private const int WM_SETCHECKED = (WM_APP + 1);

        [DllImport("user32.dll")]
        protected static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private IntPtr safeHandle = IntPtr.Zero;

        private SettingBoolEntry entry;

        public SettingEntryCheckBox()
        {
        }

        [Browsable(false)]
        [DefaultValue("")]
        public SettingBoolEntry SettingEntry
        {
            get { return entry; }
            set
            {
                if (entry != null)
                    entry.Changed -= entry_Changed;

                entry = value;

                if (entry != null)
                {
                    entry.Changed += new EventHandler(entry_Changed);

                    if (IsSafeHandleCreated)
                    {
                        PostMessage(SafeHandle, WM_SETCHECKED, 0, 0);
                    }
                }
            }
        }

        public virtual IntPtr SafeHandle
        {
            get
            {
                if (safeHandle == IntPtr.Zero)
                    throw new InvalidOperationException("Handle not created yet.");
                else
                    return safeHandle;
            }
        }

        public virtual bool IsSafeHandleCreated
        {
            get { return safeHandle != IntPtr.Zero; }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            safeHandle = Handle;
            base.OnHandleCreated(e);

            PostMessage(SafeHandle, WM_SETCHECKED, 0, 0);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            safeHandle = IntPtr.Zero;
            base.OnHandleDestroyed(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SETCHECKED)
            {
                if (entry != null)
                {
                    Checked = entry.Value;
                }
                m.Result = IntPtr.Zero;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            if (entry != null)
                entry.Value = Checked;

            base.OnCheckedChanged(e);
        }

        void entry_Changed(object sender, EventArgs e)
        {
            if (IsSafeHandleCreated)
            {
                PostMessage(SafeHandle, WM_SETCHECKED, 0, 0);
            }
        }
    }
}
