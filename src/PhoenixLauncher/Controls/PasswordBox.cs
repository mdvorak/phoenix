using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PhoenixLauncher.Controls
{
    [DesignTimeVisible(true)]
    public partial class PasswordBox : FieldBoxBase
    {
        public PasswordBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets decrypted password.
        /// </summary>
        public override string Value
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public override ErrorType Check()
        {
            if (textBox.Text.Length == 0)
            {
                ErrorString = "Password hasn't been specified.";
                return ErrorType.Warning;
            }

            for (int i = 0; i < textBox.TextLength; i++)
            {
                if (textBox.Text[i] < 33)
                {
                    ErrorString = "Password contains some invalid characters.";
                    return ErrorType.Warning;
                }
            }

            ErrorString = null;
            return ErrorType.None;
        }
    }
}
