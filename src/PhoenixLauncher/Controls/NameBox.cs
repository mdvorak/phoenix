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
    public partial class NameBox : FieldBoxBase
    {
        public NameBox()
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
            if (textBox.Text.Length == 0)
            {
                ErrorString = "Name must be specified.";
                return ErrorType.Error;
            }

            HistoryCache.Add(GetType().ToString(), Value);
            ErrorString = null;
            return ErrorType.None;
        }
    }
}
