using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui
{
    public partial class ExceptionDialog : Form
    {
        public ExceptionDialog()
        {
            InitializeComponent();
        }

        public Exception Exception
        {
            get { return exceptionControl.Exception; }
            set { exceptionControl.Exception = value; }
        }

        public string Message
        {
            get { return messageBox.Text; }
            set { messageBox.Text = value; }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        public static DialogResult Show(Exception e, string message)
        {
            using (ExceptionDialog dlg = new ExceptionDialog()) {
                dlg.Message = message;
                dlg.Exception = e;
                return dlg.ShowDialog();
            }
        }
    }
}