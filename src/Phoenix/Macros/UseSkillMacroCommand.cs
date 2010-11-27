using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Macros
{
    public class UseSkillMacroCommand : IMacroCommand
    {
        private byte skill;
        public event EventHandler CommandChanged;

        public UseSkillMacroCommand(byte skillNum)
        {
            this.skill = skillNum;
        }

        public byte Skill
        {
            get { return skill; }
        }

        public MacroCommandType CommandType
        {
            get { return MacroCommandType.Active; }
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
                if (DataFiles.Skills != null && skill < DataFiles.Skills.Count)
                    return new TextCommand("useskill", DataFiles.Skills[skill].Name);
                else
                    return new TextCommand("useskill", skill);
            }
        }

        public override string ToString()
        {
            if (DataFiles.Skills != null && skill < DataFiles.Skills.Count)
                return String.Format("UseSkill \"{0}\"", DataFiles.Skills[skill].Name);
            else
                return String.Format("UseSkill {0}", skill);
        }
    }
}
