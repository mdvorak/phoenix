using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Phoenix.Gui.Editor
{
    public class CSClassTemplate : IItemTemplate
    {
        private const string Template =
@"using System;
using System.Collections.Generic;
using System.Text;
using Phoenix;
using Phoenix.WorldData;

namespace %namespace%
{
    public class %class%
    {
        public %class%()
        {
        
        }
    }
}";

        string IItemTemplate.Name
        {
            get { return "C# Class"; }
        }

        string IItemTemplate.Description
        {
            get { return "An empty C# class definiton"; }
        }

        System.Drawing.Icon IItemTemplate.Icon
        {
            get { return TemplateIcons.CSClass; }
        }

        string IItemTemplate.Extension
        {
            get { return ".cs"; }
        }

        string IItemTemplate.Group
        {
            get { return null; }
        }

        void IItemTemplate.Create(string path, string name)
        {
            using (StreamWriter writer = File.CreateText(path))
            {
                writer.Write(Template.Replace("%class%", name).Replace("%namespace%", "Phoenix.Scripts"));
            }
        }
    }
}
