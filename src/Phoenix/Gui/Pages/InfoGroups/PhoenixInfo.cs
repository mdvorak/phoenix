using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Phoenix.Gui.Pages.InfoGroups
{
    [InfoGroup("Phoenix")]
    public partial class PhoenixInfo : UserControl
    {
        public PhoenixInfo()
        {
            InitializeComponent();

            versionBox.Text = Core.Version.ToString();

#if DEBUG
            versionBox.Text += " Debug";
#endif

            copyrightBox.Text = AssemblyCopyright;
        }

        private void phoenixIcon_Click(object sender, EventArgs e)
        {
            using (AboutDialog dlg = new AboutDialog())
            {
                dlg.ShowDialog();
            }            
        }

        public string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
    }
}
