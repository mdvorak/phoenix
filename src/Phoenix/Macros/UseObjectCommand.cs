using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Macros
{
    [Serializable]
    class UseObjectCommand : ObjectCommandBase
    {
        public UseObjectCommand(Serial serial)
            : base(serial)
        {

        }

        public override MacroCommandType CommandType
        {
            get { return MacroCommandType.Active; }
        }

        protected override string CommandPrefix
        {
            get { return "Use"; }
        }
    }
}
