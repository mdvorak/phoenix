using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Gui.Editor
{
    public class EmptyFileTemplate : IItemTemplate
    {
        private string name;
        private string description;
        private string extension;
        private string group;
        private System.Drawing.Icon icon;

        public EmptyFileTemplate(string name, string description, string extension, string group, System.Drawing.Icon icon)
        {
            this.name = name;
            this.description = description;
            this.extension = extension;
            this.group = group;
            this.icon = icon;
        }

        string IItemTemplate.Name
        {
            get { return name; }
        }

        string IItemTemplate.Description
        {
            get { return description; }
        }

        System.Drawing.Icon IItemTemplate.Icon
        {
            get { return icon; }
        }

        string IItemTemplate.Extension
        {
            get { return extension; }
        }

        string IItemTemplate.Group
        {
            get { return group; }
        }

        void IItemTemplate.Create(string path, string name)
        {
            System.IO.File.Create(path).Close();
        }
    }
}
