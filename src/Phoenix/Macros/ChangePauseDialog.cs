using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Macros
{
    partial class ChangePauseDialog : Form
    {
        public ChangePauseDialog()
        {
            InitializeComponent();
        }

        public int Pause
        {
            get { return numTextBox1.Value; }
            set { numTextBox1.Value = value; }
        }
    }
}