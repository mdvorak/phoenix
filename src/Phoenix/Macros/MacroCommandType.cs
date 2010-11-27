using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Macros
{
    public enum MacroCommandType
    {
        /// <summary>
        /// Pause is inserted after this command.
        /// </summary>
        Active,
        /// <summary>
        /// Command should send anything to server. (i.e. waittargetobject, wait..)
        /// </summary>
        Pasive,
        /// <summary>
        /// Command is inserted before last active command.
        /// </summary>
        PrecedingActive
    }
}
