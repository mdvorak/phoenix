using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using Phoenix.Runtime;

namespace Phoenix.Macros
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Implementer should override ToString().</remarks>
    public interface IMacroCommand
    {
        /// <summary>
        /// Creates custom menu items for command modification.
        /// </summary>
        /// <returns>Menu items array (could be empty) or null.</returns>
        /// <remarks>
        /// Called everytime menu is shown, so implementer doesn't need to take care
        /// about items state. After menu is hidden items are disposed.
        /// </remarks>
        ToolStripDropDownItem[] CreateCustomMenu();

        /// <summary>
        /// Gets executable command. Must not be null.
        /// </summary>
        TextCommand TextCommand { get; }

        event EventHandler CommandChanged;

        /// <summary>
        /// Gets command type.
        /// </summary>
        MacroCommandType CommandType { get; }
    }
}
