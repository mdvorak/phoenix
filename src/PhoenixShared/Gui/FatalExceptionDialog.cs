using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui
{
    public partial class FatalExceptionDialog : Form
    {
        public FatalExceptionDialog()
        {
            InitializeComponent();
        }

        public Exception Exception
        {
            get { return exceptionControl.Exception; }
            set { exceptionControl.Exception = value; }
        }

        public bool TryToContinueEnabled
        {
            get { return continueButton.Enabled; }
            set { continueButton.Enabled = value; }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            Close();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
            Close();
        }
    }
}