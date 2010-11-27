using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Phoenix.Gui;

namespace Phoenix
{
    public static class Notepad
    {
        private static NotepadDialog dialog;

        internal static void Init()
        {
            if (!Core.GuiThread.InvokeRequired)
            {
                dialog = new NotepadDialog();
                dialog.CreateWindow();
            }
            else
            {
                Core.GuiThread.InvokeFast(new MethodInvoker(Init));
            }
        }

        public static void Open()
        {
            if (!Core.GuiThread.InvokeRequired)
            {
                dialog.Show();
                dialog.Focus();
            }
            else
            {
                Core.GuiThread.InvokeFast(new MethodInvoker(Open));
            }
        }

        public static void Close()
        {
            if (!Core.GuiThread.InvokeRequired)
            {
                dialog.Hide();
            }
            else
            {
                Core.GuiThread.InvokeFast(new MethodInvoker(Close));
            }
        }

        public static void Clear()
        {
            if (!Core.GuiThread.InvokeRequired)
            {
                dialog.Show();
                dialog.Focus();
            }
            else
            {
                Core.GuiThread.InvokeFast(new MethodInvoker(Clear));
            }
        }

        public static void Write(object text)
        {
            if (!Core.GuiThread.InvokeRequired)
            {
                Open();
                dialog.Write(text.ToString());
            }
            else
            {
                Core.GuiThread.InvokeFast(new ParameterizedThreadStart(Write), text);
            }
        }

        public static void Write(string format, params object[] args)
        {
            Write((object)String.Format(format, args));
        }

        public static void WriteLine(object text)
        {
            if (!Core.GuiThread.InvokeRequired)
            {
                Open();
                dialog.WriteLine(text.ToString());
            }
            else
            {
                Core.GuiThread.InvokeFast(new ParameterizedThreadStart(WriteLine), text);
            }
        }

        public static void WriteLine(string format, params object[] args)
        {
            WriteLine((object)String.Format(format, args));
        }

        public static void WriteLine()
        {
            WriteLine((object)"");
        }
    }
}
