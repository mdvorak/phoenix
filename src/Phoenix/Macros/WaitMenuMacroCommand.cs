using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Macros
{
    public class WaitMenuMacroCommand : IMacroCommand
    {
        private MenuSelection[] menus;

        public event EventHandler CommandChanged;

        public WaitMenuMacroCommand(params MenuSelection[] menus)
        {
            if (menus == null)
                throw new ArgumentNullException("menus");

            this.menus = menus;
        }

        public MenuSelection[] Menus
        {
            get { return menus; }
        }

        protected virtual void OnCommandChanged(EventArgs e)
        {
            SyncEvent.Invoke(CommandChanged, this, e);
        }

        public ToolStripDropDownItem[] CreateCustomMenu()
        {
            return null;
        }

        public TextCommand TextCommand
        {
            get
            {
                object[] args = new object[menus.Length * 2];

                for (int i = 0; i < menus.Length; i++) {
                    args[i * 2] = menus[i].Name;
                    args[i * 2 + 1] = menus[i].Option;
                }

                return new TextCommand("waitmenu", args);
            }
        }

        public MacroCommandType CommandType
        {
            get { return MacroCommandType.PrecedingActive; }
        }

        public override string ToString()
        {
            string text = "WaitMenu";

            for (int i = 0; i < menus.Length; i++) {
                text += String.Format(" \"{0}\"", menus[i].Name);

                if (menus[i].Option != null)
                    text += String.Format(" \"{0}\"", menus[i].Option);
            }

            return text;
        }
    }
}
