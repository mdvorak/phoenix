using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Gui.Pages._Log
{
    class LogCategoryInfo
    {
        public LogCategoryInfo(bool enabled, System.Drawing.Color color)
        {
            Enabled = enabled;
            Color = color;
        }

        public bool Enabled;
        public System.Drawing.Color Color;
    }
}
