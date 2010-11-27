using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Phoenix.Gui.Editor
{
    public class VBClassTemplate : IItemTemplate
    {
        private const string Template =
   @"Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Phoenix
Imports Phoenix.WorldData

Namespace %namespace%
    Public Class %class%
        Public Sub New()
            
		End Sub
    End Class
End Namespace
";

        string IItemTemplate.Name
        {
            get { return "VB Class"; }
        }

        string IItemTemplate.Description
        {
            get { return "An empty Visual Basic class definiton"; }
        }

        System.Drawing.Icon IItemTemplate.Icon
        {
            get { return TemplateIcons.VBClass; }
        }

        string IItemTemplate.Extension
        {
            get { return ".vb"; }
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
