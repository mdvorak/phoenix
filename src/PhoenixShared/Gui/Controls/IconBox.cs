using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Controls
{
    [ToolboxBitmap(typeof(PictureBox))]
    public class IconBox : Control
    {
        private Icon icon;
        private bool stretch;

        public IconBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            BackColor = System.Drawing.Color.Transparent;
            Size = new Size(32, 32);
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        public Icon Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Stretch
        {
            get { return stretch; }
            set
            {
                stretch = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DefaultValue("")]
        public override string Text
        {
            get { return "IconBox"; }
            set { }
        }

        [Browsable(false)]
        [DefaultValue("")]
        public override Image BackgroundImage
        {
            get { return null; }
            set { }
        }

        [Browsable(false)]
        [DefaultValue("")]
        public override ImageLayout BackgroundImageLayout
        {
            get { return ImageLayout.None; }
            set { }
        }


        public override Size GetPreferredSize(Size proposedSize)
        {
            if (icon != null)
                return icon.Size;
            else
                return proposedSize;
        }

        [Browsable(false)]
        [DefaultValue("")]
        public override System.Drawing.Color ForeColor
        {
            get { return System.Drawing.Color.Transparent; }
            set { }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (icon != null) {
                if (stretch)
                    e.Graphics.DrawIcon(icon, new Rectangle(0, 0, Width, Height));
                else
                    e.Graphics.DrawIconUnstretched(icon, new Rectangle(0, 0, Width, Height));
            }
        }
    }
}
