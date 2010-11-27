using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Macros
{
    public class OpenDoorMacroCommand : IMacroCommand
    {
        public event EventHandler CommandChanged;

        public ToolStripDropDownItem[] CreateCustomMenu()
        {
            return null;
        }

        protected virtual void OnCommandChanged(EventArgs e)
        {
            SyncEvent.Invoke(CommandChanged, this, e);
        }

        public MacroCommandType CommandType
        {
            get { return MacroCommandType.Active; }
        }

        public TextCommand TextCommand
        {
            get { return new TextCommand("opendoor"); }
        }

        public override string ToString()
        {
            return "OpenDoor";
        }
    }
}
