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
    partial class ServerDialog : Form
    {
        public ServerDialog(bool editDialog)
        {
            InitializeComponent();

            if (editDialog)
            {
                this.Text = "Edit server..";
                okButton.Text = "Update";
                nameBox.ReadOnly = true;
            }
            else
            {
                this.Text = "Add server..";
                okButton.Text = "Add";
                nameBox.ReadOnly = false;
            }

            clientExeBox.Value = Constants.Client;
            ultimaDirBox.Value = Constants.UltimaDir;
        }

        public Server ServerData
        {
            get
            {
                Server s = new Server(nameBox.Value);
                s.Address = addressBox.Value;
                s.Encryption = serverEncryptionBox.Value;
                s.ClientExe = clientExeBox.Value;
                s.UltimaDir = ultimaDirBox.Value;
                return s;
            }
            set
            {
                nameBox.Value = value.Name;
                addressBox.Value = value.Address;
                serverEncryptionBox.Value = value.Encryption;
                clientExeBox.Value = value.ClientExe;
                ultimaDirBox.Value = value.UltimaDir;
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
                    DialogResult result = MessageBox.Show(errString + " Do you want to save server anyway?", "Warning", MessageBoxButtons.YesNo);
                    return result != DialogResult.Yes;

                default:
                    return false;
            }
        }

        private void ServerDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                ErrorType err = nameBox.Check();
                if (!e.Cancel) e.Cancel = ProcessError(err, nameBox.ErrorString);

                err = addressBox.Check();
                if (!e.Cancel) e.Cancel = ProcessError(err, addressBox.ErrorString);

                err = clientExeBox.Check();
                if (!e.Cancel) e.Cancel = ProcessError(err, clientExeBox.ErrorString);

                err = ultimaDirBox.Check();
                if (!e.Cancel) e.Cancel = ProcessError(err, ultimaDirBox.ErrorString);
            }
        }

        private void ServerDialog_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(SystemPens.ActiveBorder, new Point(13, 133), new Point(197, 133));
        }

        private void ServerDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                if (Constants.Client == null || Constants.Client.Length == 0)
                    Constants.Client = clientExeBox.Value;

                if (Constants.UltimaDir == null || Constants.UltimaDir.Length == 0)
                    Constants.UltimaDir = ultimaDirBox.Value;
            }
        }
    }
}