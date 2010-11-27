using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Phoenix.Gui.Editor
{
    public class BooScriptTemplate : IItemTemplate
    {
        private const string Template =
@"import System
import System.Collections.Generic;
import Phoenix
import Phoenix.WorldData

[Executable]
def %class%():
    UO.Print(""Hello World"")
";

        string IItemTemplate.Name
        {
            get { return "Boo Script"; }
        }

        string IItemTemplate.Description
        {
            get { return "A Boo script file"; }
        }

        System.Drawing.Icon IItemTemplate.Icon
        {
            get { return TemplateIcons.BooFile; }
        }

        string IItemTemplate.Extension
        {
            get { return ".boo"; }
        }

        string IItemTemplate.Group
        {
            get { return null; }
        }

        void IItemTemplate.Create(string path, string name)
        {
            using (StreamWriter writer = File.CreateText(path)) {
                writer.Write(Template.Replace("%class%", name).Replace("%namespace%", "Phoenix.Scripts"));
            }
        }
    }
}
