using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Pages._Log
{
    [ToolboxItem(false)]
    partial class LogCategoriesControl : UserControl
    {
        public LogCategoriesControl()
        {
            InitializeComponent();

            colorButton.BackColor = System.Drawing.Color.Black;
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            colorDialog.Color = colorButton.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                colorButton.BackColor = colorDialog.Color;
            }
        }

        [Category("Data")]
        [DefaultValue("")]
        public string CategoryName
        {
            get { return checkBox.Text; }
            set { checkBox.Text = value; }
        }

        [Category("Data")]
        [DefaultValue(true)]
        public bool CategoryEnabled
        {
            get { return checkBox.Checked; }
            set { checkBox.Checked = value; }
        }

        [Category("Data")]
        [DefaultValue("")]
        public System.Drawing.Color CategoryColor
        {
            get { return colorButton.BackColor; }
            set { colorButton.BackColor = value; }
        }
    }
}
