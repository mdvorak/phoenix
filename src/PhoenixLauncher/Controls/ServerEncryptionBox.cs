using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PhoenixLauncher.Data;

namespace PhoenixLauncher.Controls
{
    public partial class ServerEncryptionBox : UserControl
    {
        public ServerEncryptionBox()
        {
            InitializeComponent();

            foreach (KeyValuePair<String, UOKey> entry in Constants.UOKeys.List)
            {
                comboBox.Items.Add(entry.Key);
            }
        }

        public string Value
        {
            get { return (string)comboBox.SelectedItem; }
            set { comboBox.SelectedItem = value; }
        }
    }
}
