using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Phoenix.WorldData;

namespace Phoenix.Gui.Pages.InfoGroups
{
    [InfoGroup("World")]
    public partial class WorldInfo : UserControl
    {
        public WorldInfo()
        {
            InitializeComponent();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                itemCountBox.Text = World.ItemList.Count.ToString();
                charCountBox.Text = World.CharList.Count.ToString();
            }
        }
    }
}
