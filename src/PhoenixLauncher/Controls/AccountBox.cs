using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PhoenixLauncher.Controls
{
    [DesignTimeVisible(true)]
    public partial class AccountBox : FieldBoxBase
    {
        public AccountBox()
        {
            InitializeComponent();

            textBox.AutoCompleteCustomSource.AddRange(HistoryCache.GetPosibilities(GetType().ToString()));
        }

        public override string Value
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return textBox.ReadOnly; }
            set { textBox.ReadOnly = value; }
        }

        public override ErrorType Check()
        {
            if (textBox.TextLength == 0)
            {
                ErrorString = "Account must be specified.";
                return ErrorType.Error;
            }

            for (int i = 0; i < textBox.TextLength; i++)
            {
                if (textBox.Text[i] < 33)
                {
                    ErrorString = "Account contains some invalid characters.";
                    return ErrorType.Warning;
                }
            }

            HistoryCache.Add(GetType().ToString(), Value);
            ErrorString = null;
            return ErrorType.None;
        }
    }
}
