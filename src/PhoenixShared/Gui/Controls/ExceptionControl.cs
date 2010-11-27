using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Controls
{
    public partial class ExceptionControl : UserControl
    {
        private Exception exception;

        public ExceptionControl()
        {
            InitializeComponent();
        }

        public Exception Exception
        {
            get { return exception; }
            set
            {
                exception = value;
                if (exception != null)
                {
                    messageBox.Text = exception.Message;
                    toolTip.SetToolTip(messageBox, exception.Message);
                    detailsBox.Text = exception.ToString();
                }
                else
                {
                    messageBox.Text = "";
                    toolTip.SetToolTip(messageBox, null);
                    detailsBox.Text = "";
                }
            }
        }
    }
}
