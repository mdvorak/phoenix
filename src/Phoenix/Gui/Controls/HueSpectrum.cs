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
    public class HueSpectrum : Control
    {
        private Bitmap cache;
        private Hues hues;
        private int hueIndex;

        [Category("Property Changed")]
        public event EventHandler HueIndexChanged;

        public HueSpectrum()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.StandardClick, true);
            SetStyle(ControlStyles.UserPaint, true);

            cache = null;
            hues = null;
            hueIndex = 1;
        }

        [Browsable(false)]
        [DefaultValue("")]
        public Hues Hues
        {
            get { return hues; }
            set
            {
                hues = value;
                // To be sure that index is in range
                HueIndex = hueIndex;
                ClearCache();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(1)]
        public int HueIndex
        {
            get { return hueIndex; }
            set
            {
                int newValue;

                if (hues != null)
                    newValue = Math.Max(hues.MinIndex, Math.Min(value, hues.MaxIndex));
                else
                    newValue = value;

                if (newValue != hueIndex)
                {
                    hueIndex = newValue;
                    OnHueIndexChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnHueIndexChanged(EventArgs e)
        {
            ClearCache();
            Invalidate();
            SyncEvent.Invoke(HueIndexChanged, this, e);
        }

        protected void ClearCache()
        {
            if (cache != null)
                cache.Dispose();

            cache = null;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (hues != null)
            {
                if (cache == null)
                {
                    cache = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format16bppArgb1555);
                    HuesRenderer.DrawSpectrum(cache, hues, hueIndex);
                }

                pe.Graphics.DrawImageUnscaled(cache, 0, 0);
            }
            else
            {
                pe.Graphics.DrawRectangle(SystemPens.WindowText, 0, 0, Width - 1, Height - 1);

                SizeF fontSize = pe.Graphics.MeasureString(Name, SystemFonts.DialogFont);
                pe.Graphics.DrawString(Name, SystemFonts.DialogFont, SystemBrushes.WindowText, (Width - fontSize.Width) / 2, (Height - fontSize.Height) / 2);
            }

            base.OnPaint(pe);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            ClearCache();

            base.OnSizeChanged(e);
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
