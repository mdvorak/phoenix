using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Pages._Journal
{
    partial class MaxJournalLenghtDialog : Form
    {
        public MaxJournalLenghtDialog()
        {
            InitializeComponent();
        }

        private void MaxJournalLenghtDialog_Load(object sender, EventArgs e)
        {
            numericUpDown.Value = Config.Profile.MaxJournalLen.Value;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Config.Profile.MaxJournalLen.Value = (int)numericUpDown.Value;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}