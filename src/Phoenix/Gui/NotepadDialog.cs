using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui
{
    public partial class NotepadDialog : Phoenix.Gui.Controls.FormEx
    {
        public NotepadDialog()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;

            base.OnClosing(e);
        }

        public void Clear()
        {
            textBox.Clear();
        }

        public void Write(string text)
        {
            textBox.AppendText(text);
        }

        public void WriteLine(string text)
        {
            textBox.AppendText(text + Environment.NewLine);
        }
    }
}