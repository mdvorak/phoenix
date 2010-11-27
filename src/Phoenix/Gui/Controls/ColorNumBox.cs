using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Controls
{
    public partial class ColorNumBox : UserControl
    {
        [Category("Property Changed")]
        public event EventHandler ValueChanged;

        public ColorNumBox()
        {
            InitializeComponent(); 
        }

        public virtual void OnValueChanged(EventArgs e)
        {
            SyncEvent.Invoke(ValueChanged, this, e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DataFiles.Hues != null)
            {
                colorBox.MinValue = DataFiles.Hues.MinIndex;
                colorBox.MaxValue = DataFiles.Hues.MaxIndex;
            }
        }

        public ushort Value
        {
            get { return (ushort)colorBox.Value; }
            set { colorBox.Value = value; }
        }

        private void brosweButton_Click(object sender, EventArgs e)
        {
            UOSelectColorDialog dlg = new UOSelectColorDialog();
            dlg.Hues = DataFiles.Hues;
            dlg.Art = DataFiles.Art;
            dlg.Extended = true;
            dlg.SelectedColorIndex = Value;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Value = (ushort)dlg.SelectedColorIndex;
            }
        }

        private void colorBox_ValueChanged(object sender, EventArgs e)
        {
            OnValueChanged(e);
        }
    }
}
