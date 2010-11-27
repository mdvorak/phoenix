using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PhoenixLauncher.Data;

namespace PhoenixLauncher.Controls
{
    partial class AccountDialog : Form
    {
        public AccountDialog(bool editDialog)
        {
            InitializeComponent();

            if (editDialog)
            {
                this.Text = "Edit account..";
                okButton.Text = "Update";
                accountBox.ReadOnly = true;
            }
            else
            {
                this.Text = "Add account..";
                okButton.Text = "Add";
                accountBox.ReadOnly = false;
            }
        }

        public Account AccountData
        {
            get
            {
                Account a = new Account(accountBox.Value);
                a.Password = passwordBox.Value;
                return a;
            }
            set
            {
                accountBox.Value = value.Name;
                passwordBox.Value = value.Password;
            }
        }

        private bool ProcessError(ErrorType err, string errString)
        {
            switch (err)
            {
                case ErrorType.Error:
                    MessageBox.Show(errString, "Error");
                    return true;

                case ErrorType.Warning:
                    DialogResult result = MessageBox.Show(errString + " Do you want to save account anyway?", "Warning", MessageBoxButtons.YesNo);
                    return result != DialogResult.Yes;

                default:
                    return false;
            }
        }

        private void AccountDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                ErrorType err = accountBox.Check();
                if (!e.Cancel) e.Cancel = ProcessError(err, accountBox.ErrorString);

                err = passwordBox.Check();
                if (!e.Cancel) e.Cancel = ProcessError(err, passwordBox.ErrorString);
            }
        }
    }
}
