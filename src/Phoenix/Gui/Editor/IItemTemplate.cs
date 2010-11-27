using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Phoenix.Gui.Editor
{
    public interface IItemTemplate
    {
        string Name { get; }
        string Description { get; }
        Icon Icon { get; }
        string Extension { get; }
        string Group { get; }

        void Create(string path, string name);
    }
}
