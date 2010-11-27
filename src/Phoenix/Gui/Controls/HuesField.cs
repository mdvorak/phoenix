using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MulLib;
using System.Diagnostics;

namespace Phoenix.Gui.Controls
{
    public class HuesField : Control
    {
        private Bitmap[] bitmapCache;
        private Hues hues;
        private Size hueSize;
        private int brightness;
        private bool showAllColors;
        private int selectedColorIndex;
        private Timer focusRectRefreshTimer;
        private IContainer components;
        private Color focusRectangleColor = Color.White;

        [Category("Property Changed")]
        public event EventHandler BrightnessChanged;
        [Category("Property Changed")]
        public event EventHandler HueSizeChanged;
        [Category("Property Changed")]
        public event EventHandler ShowAllColorsChanged;
        [Category("Property Changed")]
        public event EventHandler SelectedColorIndexChanged;

        public HuesField()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.StandardClick, true);
            SetStyle(ControlStyles.UserPaint, true);

            bitmapCache = new Bitmap[32];
            hues = null;
            hueSize = new Size(8, 8);
            brightness = 24;
            showAllColors = false;
            selectedColorIndex = 1;

            InitializeComponent();
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
                SelectedColorIndex = selectedColorIndex;
                ClearCache();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("8; 8")]
        [Description("Size of one hue rectangle.")]
        public Size HueSize
        {
            get { return hueSize; }
            set
            {
                if (value != hueSize) {
                    hueSize = value;
                    OnHueSizeChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(24)]
        public int Brightness
        {
            get { return brightness; }
            set
            {
                int newValue = Math.Max(0, Math.Min(value, 31));
                if (newValue != brightness) {
                    brightness = newValue;
                    OnBrightnessChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowAllColors
        {
            get { return showAllColors; }
            set
            {
                if (value != showAllColors) {
                    showAllColors = value;
                    OnBrightnessChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(1)]
        public int SelectedColorIndex
        {
            get { return selectedColorIndex; }
            set
            {
                int newValue;

                if (hues != null)
                    newValue = Math.Max(hues.MinIndex, Math.Min(value, hues.MaxIndex));
                else
                    newValue = value;

                if (newValue != selectedColorIndex) {
                    RefreshFocusRectangle();
                    selectedColorIndex = newValue;
                    RefreshFocusRectangle();
                    OnSelectedColorIndexChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnBrightnessChanged(EventArgs e)
        {
            Invalidate();
            SyncEvent.Invoke(BrightnessChanged, this, e);
        }

        protected virtual void OnHueSizeChanged(EventArgs e)
        {
            ClearCache();
            Invalidate();
            SyncEvent.Invoke(HueSizeChanged, this, e);
        }

        protected virtual void OnShowAllColorsChanged(EventArgs e)
        {
            ClearCache();
            Invalidate();
            SelectedColorIndex = SelectedColorIndex;
            SyncEvent.Invoke(ShowAllColorsChanged, this, e);
        }

        protected virtual void OnSelectedColorIndexChanged(EventArgs e)
        {
            SyncEvent.Invoke(SelectedColorIndexChanged, this, e);
        }

        protected void ClearCache()
        {
            for (int i = 0; i < bitmapCache.Length; i++) {
                if (bitmapCache[i] != null)
                    bitmapCache[i].Dispose();
            }

            bitmapCache = new Bitmap[32];
        }

        private Rectangle GetFocusRectangle()
        {
            int selectedIndex = selectedColorIndex;

            if (selectedIndex >= 0 && !hueSize.IsEmpty) {
                int startIndex = showAllColors ? 0 : 1;
                int colorsPerColumn = Height / hueSize.Height;

                if (colorsPerColumn > 0) {
                    int x = (selectedIndex - startIndex - 1) / colorsPerColumn * hueSize.Height;
                    int y = ((selectedIndex - startIndex - 1) % colorsPerColumn) * hueSize.Width;

                    return new Rectangle(x, y, HueSize.Width, hueSize.Height);
                }
            }

            return new Rectangle();
        }

        private void RefreshFocusRectangle()
        {
            Rectangle focusRect = GetFocusRectangle();

            if (!focusRect.IsEmpty) {
                Invalidate(focusRect);
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (hues != null) {
                if (bitmapCache[brightness] == null) {
                    bitmapCache[brightness] = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format16bppArgb1555);

                    int min = showAllColors ? 0 : 1;
                    int max = showAllColors ? 0 : 0x3e9;
                    HuesRenderer.DrawField(bitmapCache[brightness], hues, min, max, hueSize, brightness);
                }

                pe.Graphics.DrawImageUnscaled(bitmapCache[brightness], 0, 0);

                PaintFocusRectangle(pe.Graphics);
            }
            else {
                pe.Graphics.DrawRectangle(SystemPens.WindowText, 0, 0, Width - 1, Height - 1);

                SizeF fontSize = pe.Graphics.MeasureString(Name, SystemFonts.DialogFont);
                pe.Graphics.DrawString(Name, SystemFonts.DialogFont, SystemBrushes.WindowText, (Width - fontSize.Width) / 2, (Height - fontSize.Height) / 2);
            }

            base.OnPaint(pe);
        }

        private void PaintFocusRectangle(Graphics g)
        {
            Debug.Assert(g != null, "Graphics == null");

            if (hues != null & bitmapCache[brightness] != null) {
                Rectangle focusRect = GetFocusRectangle();

                if (!focusRect.IsEmpty) {
                    focusRect.Width--;
                    focusRect.Height--;

                    using (Pen pen = new Pen(Color.FromArgb(180, focusRectangleColor))) {
                        g.DrawRectangle(pen, focusRect);
                    }
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            ClearCache();

            base.OnSizeChanged(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                int x = e.Location.X / hueSize.Width;
                int y = e.Location.Y / hueSize.Height;
                int colorsPerColumn = Height / hueSize.Height;
                int startIndex = showAllColors ? 0 : 1;
                int index = startIndex + y + x * colorsPerColumn + 1;

                if (hues != null) {
                    int max = showAllColors ? hues.MaxIndex : 0x03e9;
                    if (index >= hues.MinIndex && index <= max) {
                        SelectedColorIndex = index;
                    }
                }
            }

            base.OnMouseClick(e);
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.focusRectRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // focusRectRefreshTimer
            // 
            this.focusRectRefreshTimer.Enabled = true;
            this.focusRectRefreshTimer.Interval = 600;
            this.focusRectRefreshTimer.Tick += new System.EventHandler(this.focusRectRefreshTimer_Tick);
            this.ResumeLayout(false);

        }

        private void focusRectRefreshTimer_Tick(object sender, EventArgs e)
        {
            focusRectangleColor = Color.FromArgb(255 - focusRectangleColor.R, 255 - focusRectangleColor.G, 255 - focusRectangleColor.B);
            RefreshFocusRectangle();
        }
    }
}
