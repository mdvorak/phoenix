using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PhoenixLauncher.Controls
{
    [DesignTimeVisible(true)]
    public partial class UltimaDirBox : FieldBoxBase
    {
        public UltimaDirBox()
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
                ErrorString = "Ultima Online directory hasn't been specified.";
                return ErrorType.Warning;
            }

            if (!System.IO.Directory.Exists(textBox.Text))
            {
                ErrorString = "Directory \"" + textBox.Text + "\" doesn't exist.";
                return ErrorType.Warning;
            }

            HistoryCache.Add(GetType().ToString(), Value);
            ErrorString = null;
            return ErrorType.None;
        }

        private void pathButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = textBox.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
