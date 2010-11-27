using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Pages._Log
{
    partial class LogFilter : Form
    {
        public LogFilter()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        public Dictionary<string, LogCategoryInfo> GetCategories()
        {
            Dictionary<string, LogCategoryInfo> list = new Dictionary<string, LogCategoryInfo>(8);

            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is LogCategoriesControl)
                {
                    LogCategoriesControl c = (LogCategoriesControl)Controls[i];

                    LogCategoryInfo info = new LogCategoryInfo(c.CategoryEnabled, c.CategoryColor);
                    list.Add(c.CategoryName, info);
                }
            }

            return list;
        }
    }
}