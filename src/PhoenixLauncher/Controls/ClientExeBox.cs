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
    public partial class ClientExeBox : FieldBoxBase
    {
        public ClientExeBox()
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
                ErrorString = "Client hasn't been specified.";
                return ErrorType.Warning;
            }

            if (!System.IO.File.Exists(textBox.Text))
            {
                ErrorString = "Client \"" + textBox.Text + "\" doesn't exist.";
                return ErrorType.Warning;
            }

            HistoryCache.Add(GetType().ToString(), Value);
            ErrorString = null;
            return ErrorType.None;
        }

        private void pathButton_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(textBox.Text))
            {
                openFileDialog.FileName = textBox.Text;
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = openFileDialog.FileName;
            }
        }
    }
}
