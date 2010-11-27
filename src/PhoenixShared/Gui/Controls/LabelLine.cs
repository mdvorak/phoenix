using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Phoenix.Gui.Controls
{
    [ToolboxBitmap(typeof(Label))]
    public class LabelLine : Label
    {
        private System.Drawing.Color lineColor;

        [Browsable(true)]
        [Category("Property Changed")]
        public event EventHandler LineColorChanged;

        public LabelLine()
        {
            Height = FontHeight;
            AutoSize = false;
            Margin = new Padding(3);
            ForeColor = System.Drawing.Color.RoyalBlue;
            lineColor = SystemColors.ActiveBorder;
        }

        [DefaultValue(false)]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Color of the backround line.")]
        public System.Drawing.Color LineColor
        {
            get { return lineColor; }
            set
            {
                if (lineColor != value)
                {
                    lineColor = value;
                    OnLineColorChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnLineColorChanged(EventArgs e)
        {
            SyncEvent.Invoke(LineColorChanged, this, e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Pen linePen = new Pen(LineColor);
            SizeF fontSize = e.Graphics.MeasureString(Text, Font);

            int y = FontHeight / 2 + 1;

            e.Graphics.DrawLine(linePen, 0, y, 4, y);
            e.Graphics.DrawLine(linePen, 8 + fontSize.Width, y, Width, y);

            e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), 6, 0);
        }
    }
}
