using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using Phoenix.Properties;

namespace Phoenix.Gui
{
    partial class AboutDialog : Form
    {
        private class Licence
        {
            public readonly string Title;
            public readonly string Copyright;
            public readonly string Text;

            public Licence(string title, string copyright, string text)
            {
                Title = title;
                Copyright = copyright;
                Text = text;
            }

            public override string ToString()
            {
                return Title;
            }
        }

        public AboutDialog()
        {
            InitializeComponent();

            linkLabel.Text = Core.HomepageUrl;

            licenceBox.ResetText();
            copyrightBox.ResetText();
            licencesListBox.Items.Clear();

            licencesListBox.Items.Add(new Licence(Core.VersionString, AssemblyCopyright, Resources.Licence_GPL));
            licencesListBox.Items.Add(new Licence("UOEncryption", "Copyright © 2004 Daniel 'Necr0Potenc3' Cavalcanti", Resources.Licence_GPL));
            licencesListBox.Items.Add(new Licence("MagicLibrary", "Copyright © 2003 Crownwood Consulting Ltd.", Resources.Licence_MagicLibrary));
            licencesListBox.Items.Add(new Licence("XPTable", "Copyright © 2005 Mathew Hall. All rights reserved", Resources.Licence_XPTable));

            licencesListBox.SelectedIndex = 0;

            Text = "About " + Core.VersionString;
            versionBox.Text = "Version " + Core.Version.ToString();
#if DEBUG
            versionBox.Text += " Debug";
#endif
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            using (LinearGradientBrush b = new LinearGradientBrush(new Point(), new Point(0, 160), Color.Gold, BackColor))
            {
                e.Graphics.FillRectangle(b, 0, 0, ClientSize.Width, 160);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void licenceBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Helper.OpenBroswer(e.LinkText);
        }

        private void licencesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            licenceBox.ResetText();

            if (licencesListBox.SelectedItem != null)
            {
                copyrightBox.Text = ((Licence)licencesListBox.SelectedItem).Copyright;
                licenceBox.Text = ((Licence)licencesListBox.SelectedItem).Text;
            }
            else
            {
                copyrightBox.ResetText();
            }
        }

        private void linkLabel_Click(object sender, EventArgs e)
        {
            Helper.OpenBroswer(Core.HomepageUrl);
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                // Get all Title attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
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

        public string AssemblyCompany
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
