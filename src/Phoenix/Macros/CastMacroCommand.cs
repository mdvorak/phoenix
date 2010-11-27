using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Macros
{
    public class CastMacroCommand : IMacroCommand
    {
        private byte spell;
        public event EventHandler CommandChanged;

        public CastMacroCommand(byte spellNum)
        {
            this.spell = spellNum;
        }

        protected virtual void OnCommandChanged(EventArgs e)
        {
            SyncEvent.Invoke(CommandChanged, this, e);
        }

        public byte Spell
        {
            get { return spell; }
        }

        public MacroCommandType CommandType
        {
            get { return MacroCommandType.Active; }
        }

        public ToolStripDropDownItem[] CreateCustomMenu()
        {
            return null;
        }

        public TextCommand TextCommand
        {
            get { return new TextCommand("cast", spell); }
        }

        public override string ToString()
        {
            return String.Format("Cast {0}", spell);
        }
    }
}
