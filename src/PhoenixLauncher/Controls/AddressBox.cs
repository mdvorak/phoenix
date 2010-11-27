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
    public partial class AddressBox : FieldBoxBase
    {
        public AddressBox()
        {
            InitializeComponent();

            textBox.AutoCompleteCustomSource.AddRange(HistoryCache.GetPosibilities(GetType().ToString()));
        }

        public override string Value
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public override ErrorType Check()
        {
            if (textBox.Text.Length == 0)
            {
                ErrorString = "Address hasn't been specified.";
                return ErrorType.Warning;
            }

            Regex regex = new Regex(@"\A(?:[a-zA-Z0-9]|\x5F|\x2E)+,\d+\z");
            if (!regex.IsMatch(textBox.Text))
            {
                ErrorString = "Address has invalid format.\n\nExamples:\nlogin.owo.com,7775\n127.0.0.1,2593\n\n";
                return ErrorType.Warning;
            }

            HistoryCache.Add(GetType().ToString(), Value);
            ErrorString = null;
            return ErrorType.None;
        }
    }
}
