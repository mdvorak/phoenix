using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Controls
{
    internal partial class CategoryControl : UserControl
    {
        private Control activeControl;

        public event EventHandler CategoryListWidthChanged;

        public CategoryControl()
        {
            InitializeComponent();
        }

        private void SetControlSize(Control control)
        {
            control.Dock = DockStyle.None;
            control.Location = new Point(listBox.Width + Padding.Left, Padding.Top);
            control.Size = new Size(Width - control.Location.X - Padding.Right, Height - Padding.Vertical);
            control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        public void AddCategory(CategoryData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Control == null)
                throw new ArgumentNullException("data.Control");

            if (data.Title == null)
                throw new ArgumentNullException("data.Title");

            listBox.Items.Add(data);
            listBox.SelectedIndex = 0;

            data.Control.CreateControl();
        }

        public void RemoveCategory(CategoryData data)
        {
            listBox.Items.Remove(data);
        }

        public CategoryData this[int index]
        {
            get { return (CategoryData)listBox.Items[index]; }
        }

        public int Count
        {
            get { return listBox.Items.Count; }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(150)]
        [Description("Width of categories list.")]
        public int CategoryListWidth
        {
            get { return listBox.Width; }
            set
            {
                if (listBox.Width != value)
                {
                    listBox.Width = value;
                    OnCategoryListWidthChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnCategoryListWidthChanged(EventArgs e)
        {
            SyncEvent.Invoke(CategoryListWidthChanged, this, e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            listBox.ForeColor = this.ForeColor;
            base.OnForeColorChanged(e);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            listBox.BackColor = this.BackColor;
            base.OnBackColorChanged(e);
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CategoryData data = listBox.SelectedItem as CategoryData;

            if (data == null || data.Control != activeControl)
            {
                if (activeControl != null)
                {
                    activeControl.Hide();
                    Controls.Remove(activeControl);
                    activeControl = null;
                }

                if (data != null)
                {
                    activeControl = data.Control;
                    SetControlSize(activeControl);
                    Controls.Add(activeControl);
                    activeControl.Show();
                }
            }
        }

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0 && e.Index < listBox.Items.Count)
            {
                object item = listBox.Items[e.Index];
                Font font;

                if ((e.State & DrawItemState.Selected) != 0)
                {
                    font = new Font(e.Font, FontStyle.Bold);

                    LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, e.BackColor, BackColor, LinearGradientMode.Horizontal);
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
                else
                {
                    font = e.Font;
                    e.DrawBackground();
                }

                SizeF fontSize = e.Graphics.MeasureString(item.ToString(), e.Font);
                PointF fontStart = new PointF(e.Bounds.Left + 2, e.Bounds.Top + (e.Bounds.Height - fontSize.Height) / 2);

                e.Graphics.DrawString(item.ToString(), font, new SolidBrush(e.ForeColor), fontStart);
            }
        }
    }
}
