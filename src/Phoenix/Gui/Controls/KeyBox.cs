using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Controls
{
    public class KeyBox : TextBox
    {
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSDEADCHAR = 0x0107;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_MOUSEWHEEL = 0x20A;
        private const int MK_MBUTTON = 0x10;
        private const int MK_XBUTTON1 = 0x20;
        private const int MK_XBUTTON2 = 0x40;
        private const int WM_APPCOMMAND = 0x319;

        private string key;

        [Browsable(true)]
        [Category("Property Changed")]
        public event EventHandler KeyChanged;

        public KeyBox()
        {
            ReadOnly = true;
            BackColor = SystemColors.Window;
            WordWrap = false;
            MaxLength = 64;

            Key = Keys.None.ToString(); ;
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public override bool Multiline
        {
            get { return false; }
            set { }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public override bool ShortcutsEnabled
        {
            get { return false; }
            set { }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("None")]
        public string Key
        {
            get { return key; }
            set
            {
                if (value != key) {
                    Text = value;
                    key = value;
                    OnKeyChanged(EventArgs.Empty);
                }
            }
        }

        private void SetKey(Keys key)
        {
            Key = key.ToString();
        }

        protected virtual void OnKeyChanged(EventArgs e)
        {
            SyncEvent.Invoke(KeyChanged, this, e);
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg) {
                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                case WM_SYSDEADCHAR:
                    Keys keyCode = (Keys)m.WParam;

                    if (keyCode != Keys.ControlKey && keyCode != Keys.ShiftKey && keyCode != Keys.Menu) {
                        SetKey(keyCode | Control.ModifierKeys);
                    }

                    m.Result = IntPtr.Zero;
                    return;

                case WM_MBUTTONDOWN:
                    switch (m.WParam.ToInt32()) {
                        case MK_MBUTTON:
                            SetKey(Keys.MButton | Control.ModifierKeys);
                            break;

                        case MK_XBUTTON1:
                            SetKey(Keys.XButton1 | Control.ModifierKeys);
                            break;

                        case MK_XBUTTON2:
                            SetKey(Keys.XButton2 | Control.ModifierKeys);
                            break;
                    }
                    m.Result = IntPtr.Zero;
                    return;

                case WM_MOUSEWHEEL:
                    int delta = m.WParam.ToInt32() >> 16;
                    string k;

                    if (delta > 0) {
                        k= "WheelUp";
                    }
                    else {
                        k = "WheelDown";
                    }

                    Keys mods = Control.ModifierKeys;
                    if (mods != Keys.None)
                        k += ", " + mods.ToString();

                    Key = k;
                    m.Result = IntPtr.Zero;
                    return;

                case WM_APPCOMMAND:
                    AppCommand cmd = Helper.GET_APPCOMMAND_LPARAM(m.LParam.ToInt32());

                    if (cmd != AppCommand.None && new List<AppCommand>((AppCommand[])Enum.GetValues(typeof(AppCommand))).Contains(cmd)) {
                        Key = cmd.ToString();
                    }

                    m.Result = IntPtr.Zero;
                    return;

                default:
                    base.WndProc(ref m);
                    return;
            }
        }
    }
}
