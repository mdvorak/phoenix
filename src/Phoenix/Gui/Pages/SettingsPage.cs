using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Phoenix.Gui.Controls;
using Phoenix.Runtime;

namespace Phoenix.Gui.Pages
{
    public partial class SettingsPage : UserControl, IAssemblyObjectList
    {
        private SettingCategories.GeneralCategory general;

        public SettingsPage()
        {
            InitializeComponent();

            general = new Phoenix.Gui.Pages.SettingCategories.GeneralCategory();
            categoryControl.AddCategory(new CategoryData(Phoenix.Properties.Resources.Settings_General, general));
        }

        internal void AddCategory(CategoryData data)
        {
            RuntimeCore.AddAssemblyObject(data, this);
            categoryControl.AddCategory(data);
        }

        internal void RemoveCategory(CategoryData data)
        {
            categoryControl.RemoveCategory(data);
            RuntimeCore.RemoveAssemblyObject(data);
        }

        void IAssemblyObjectList.Remove(IAssemblyObject obj)
        {
            CategoryData data = obj as CategoryData;

            if (data != null)
            {
                categoryControl.RemoveCategory(data);
            }
        }
    }
}
