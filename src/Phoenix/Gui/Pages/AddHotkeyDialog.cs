using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Gui.Pages
{
    partial class AddHotkeyDialog : Form
    {
        public AddHotkeyDialog()
        {
            InitializeComponent();
        }

        [Category("Appearance")]
        public string Shortcut
        {
            get { return keyBox.Key; }
            set { keyBox.Key = value; ; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void keyBox_KeyChanged(object sender, EventArgs e)
        {
            bool registered = RuntimeCore.Hotkeys.Contains(keyBox.Key);
            warningLabel.Visible = registered;
            okButton.Enabled = !registered && keyBox.Key != Keys.None.ToString();
        }
    }
}