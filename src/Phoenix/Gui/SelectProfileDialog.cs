using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Phoenix.Gui.Controls;

namespace Phoenix.Gui
{
    internal partial class SelectProfileDialog : FormEx
    {
        private string selectedProfile;

        public SelectProfileDialog()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        [DefaultValue("Default")]
        public string SelectedProfile
        {
            get { return selectedProfile; }
            set
            {
                selectedProfile = value;

                if (profileListBox.Items.Contains(value))
                {
                    selectRadioButton.Checked = true;
                    profileListBox.SelectedItem = value;
                }
                else
                {
                    createRadioButton.Checked = true;
                    newProfileTextBox.Text = value;
                }

                ValidateSelectedProfile();
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public bool CreateProfile
        {
            get { return createRadioButton.Checked; }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public bool CancelEnabled
        {
            get { return cancelButton.Enabled; }
            set
            {
                cancelButton.Enabled = value;
                CloseButton = value;
            }
        }

        public void AddExistingProfiles(params string[] profiles)
        {
            profileListBox.Items.AddRange(profiles);

            if (profileListBox.Items.Count > 0)
            {
                profileListBox.SelectedIndex = 0;
                selectRadioButton.Enabled = true;
            }
        }

        private void createRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            newProfileTextBox.Enabled = createRadioButton.Checked;

            if (createRadioButton.Checked)
            {
                selectedProfile = newProfileTextBox.Text;

                ValidateSelectedProfile();
            }
        }

        private void selectRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            profileListBox.Enabled = selectRadioButton.Checked;

            if (selectRadioButton.Checked)
            {
                selectedProfile = (string)profileListBox.SelectedItem;

                ValidateSelectedProfile();
            }
        }

        private void newProfileTextBox_TextChanged(object sender, EventArgs e)
        {
            selectedProfile = newProfileTextBox.Text;

            ValidateSelectedProfile();
        }

        private void profileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(profileListBox.SelectedItem != null);

            selectedProfile = (string)profileListBox.SelectedItem;

            ValidateSelectedProfile();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ValidateSelectedProfile()
        {
            Regex regex = new Regex(@"\A(?:[a-zA-Z0-9]|\x5F|\x20)+\z");
            okButton.Enabled = selectedProfile != null && selectedProfile.Length > 0 && regex.IsMatch(selectedProfile);
        }

        private void SelectProfileDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}