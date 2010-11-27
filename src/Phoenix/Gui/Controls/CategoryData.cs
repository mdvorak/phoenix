using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


namespace Phoenix.Gui.Controls
{
    internal class CategoryData : Phoenix.Runtime.IAssemblyObject
    {
        private Control control;
        private string title;

        public CategoryData()
        {
        }

        public CategoryData(string title, Control control)
        {
            this.control = control;
            this.title = title;
        }

        public Control Control
        {
            get { return control; }
            set { control = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public override string ToString()
        {
            return title;
        }

        #region IAssemblyObject Members

        System.Reflection.Assembly Phoenix.Runtime.IAssemblyObject.Assembly
        {
            get
            {
                if (control != null)
                    return control.GetType().Assembly;
                else
                    return null;
            }
        }

        #endregion
    }
}
