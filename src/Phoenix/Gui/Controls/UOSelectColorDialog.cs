using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MulLib;

namespace Phoenix.Gui.Controls
{
    public sealed partial class UOSelectColorDialog : Form
    {
        private static readonly Size FullSize = new Size(644, 502);
        private static readonly Size SmallSize = new Size(484, 302);

        private Hues hues;
        private Art art;

        public UOSelectColorDialog()
        {
            InitializeComponent();

            huesField.Brightness = brightnessBar.Value;
        }

        [Browsable(false)]
        [DefaultValue("")]
        public Hues Hues
        {
            get { return hues; }
            set
            {
                hues = value;

                if (hues != null)
                {
                    colorIndexBox.MinValue = hues.MinIndex;
                    colorIndexBox.MaxValue = hues.MaxIndex;
                }

                huesField.Hues = hues;
                hueSpectrum.Hues = hues;
                artImageControl.Hues = hues;
            }
        }

        [Browsable(false)]
        [DefaultValue("")]
        public Art Art
        {
            get { return art; }
            set
            {
                art = value;

                if (art != null)
                {
                    artImageControl.ArtData = art.Items;
                    previewIdBox.MaxValue = artImageControl.ArtData.Count - 1;
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(1)]
        public int SelectedColorIndex
        {
            get { return huesField.SelectedColorIndex; }
            set { huesField.SelectedColorIndex = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool Extended
        {
            get { return extendedCheckBox.Checked; }
            set { extendedCheckBox.Checked = value; }
        }

        private void brightnessBar_ValueChanged(object sender, EventArgs e)
        {
            huesField.Brightness = brightnessBar.Value;
        }

        private void colorIndexBox_ValueChanged(object sender, EventArgs e)
        {
            huesField.SelectedColorIndex = colorIndexBox.Value;
        }

        private void huesField_SelectedColorIndexChanged(object sender, EventArgs e)
        {
            colorIndexBox.Value = huesField.SelectedColorIndex;
            hueSpectrum.HueIndex = huesField.SelectedColorIndex;
            artImageControl.HueIndex = huesField.SelectedColorIndex;

            colorNameLabel.Text = hues[huesField.SelectedColorIndex].Name;
        }

        private void extendedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.Size = extendedCheckBox.Checked ? FullSize : SmallSize;
            huesField.ShowAllColors = extendedCheckBox.Checked;
        }

        private void previewIdBox_ValueChanged(object sender, EventArgs e)
        {
            artImageControl.DataIndex = previewIdBox.Value;
        }
    }
}