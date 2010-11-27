using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MulLib;

namespace Phoenix.Gui.Controls
{
    [Flags]
    public enum ImageAlignment
    {
        Center = 0x00,
        Left = 0x01,
        Right = 0x02,
        Top = 0x10,
        Bottom = 0x20,
        TopLeft = Top | Left,
        TopRight = Top | Right,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right,
    }

    [ToolboxBitmap(typeof(PictureBox))]
    public sealed class ArtImageControl : Control
    {
        private Bitmap bitmap;
        private Hues hues;
        private IArtData artData;
        private int dataIndex;
        private int hueIndex;
        private bool useHue;
        private bool stocked;
        private ImageAlignment imageAlignment;

        public ArtImageControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);

            hueIndex = 24;
            imageAlignment = ImageAlignment.Center;
        }

        [Browsable(false)]
        [DefaultValue("")]
        public IArtData ArtData
        {
            get { return artData; }
            set
            {
                if (value != artData) {
                    artData = value;
                    RedrawBitmap();
                }
            }
        }

        [Browsable(false)]
        [DefaultValue("")]
        public Hues Hues
        {
            get { return hues; }
            set
            {
                if (value != hues) {
                    hues = value;
                    RedrawBitmap();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(0)]
        public int DataIndex
        {
            get { return dataIndex; }
            set
            {
                if (value != dataIndex) {
                    dataIndex = value;
                    RedrawBitmap();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(24)]
        public int HueIndex
        {
            get { return hueIndex; }
            set
            {
                if (value != hueIndex) {
                    hueIndex = value;
                    RedrawBitmap();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool UseHue
        {
            get { return useHue; }
            set
            {
                if (value != useHue) {
                    useHue = value;
                    RedrawBitmap();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(ImageAlignment.Center)]
        public ImageAlignment ImageAlignment
        {
            get { return imageAlignment; }
            set
            {
                if (value != imageAlignment) {
                    imageAlignment = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Stocked
        {
            get { return stocked; }
            set
            {
                if (value != stocked) {
                    stocked = value;
                    RedrawBitmap();
                }
            }
        }

        public void RedrawBitmap()
        {
            bitmap = null;

            if (artData != null) {
                if (!stocked) {
                    bitmap = artData[dataIndex];
                }
                else {
                    Bitmap art = artData[dataIndex];

                    // Offset 5,5 is client-hardcoded
                    // Note: PixelFormat is NEEDED!
                    bitmap = new Bitmap(art.Width + 5, art.Height + 5, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    // Draw second image over the first one
                    using (Graphics g = Graphics.FromImage(bitmap)) {
                        g.DrawImageUnscaled(art, 0, 0);
                        g.DrawImageUnscaled(art, 5, 5);
                    }
                }

                if (useHue && hues != null) {
                    HueEntry entry = hues[hueIndex];
                    if (entry != null) {
                        Dyes.RecolorFull(entry, bitmap);
                    }
                }
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (hues != null) {
                if (bitmap != null) {
                    Point pt = new Point((Width - bitmap.Width) / 2, (Height - bitmap.Height) / 2);

                    if ((imageAlignment & ImageAlignment.Left) != 0)
                        pt.X = 0;
                    else if ((imageAlignment & ImageAlignment.Right) != 0)
                        pt.X = Width - bitmap.Width;

                    if ((imageAlignment & ImageAlignment.Top) != 0)
                        pt.Y = 0;
                    else if ((imageAlignment & ImageAlignment.Bottom) != 0)
                        pt.Y = Height - bitmap.Height;

                    e.Graphics.DrawImageUnscaled(bitmap, pt);
                }
            }
            else {
                e.Graphics.DrawRectangle(SystemPens.WindowText, 0, 0, Width - 1, Height - 1);

                SizeF fontSize = e.Graphics.MeasureString(Name, SystemFonts.DialogFont);
                e.Graphics.DrawString(Name, SystemFonts.DialogFont, SystemBrushes.WindowText, (Width - fontSize.Width) / 2, (Height - fontSize.Height) / 2);
            }

            base.OnPaint(e);
        }


        #region Disabled properties

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

        [Browsable(false)]
        [DefaultValue("")]
        public override string Text
        {
            get { return Name; }
            set { }
        }

        [Browsable(false)]
        [DefaultValue("")]
        public override Font Font
        {
            get { return SystemFonts.DefaultFont; }
            set { }
        }

        [Browsable(false)]
        [DefaultValue("")]
        public override RightToLeft RightToLeft
        {
            get { return RightToLeft.No; }
            set { }
        }

        #endregion
    }
}
