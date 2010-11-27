using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;
using Phoenix.WorldData;

namespace Phoenix.Macros
{
    abstract class ObjectCommandBase : IMacroCommand
    {
        private Serial serial = Serial.Invalid;
        private Graphic graphic = Graphic.Invariant;
        private UOColor color = UOColor.Invariant;

        private string commandTarget;
        public event EventHandler CommandChanged;

        protected ObjectCommandBase(Serial serial)
        {
            this.serial = serial;

            RealItem item = World.GetRealItem(serial);
            if (item.Serial != World.InvalidSerial) {
                graphic = item.Graphic;
                color = item.Color;
            }

            CommandTarget = "Object";
        }

        /// <summary>
        /// Gets or sets current command type.
        /// </summary>
        /// <remarks>Everything after '#' is not included in command string.</remarks>
        public string CommandTarget
        {
            get { return commandTarget; }
            set
            {
                if (value != commandTarget) {
                    commandTarget = value;
                    OnCommandTargetChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets CommandPrefix + CommandTarget without #..
        /// </summary>
        protected string Command
        {
            get { return CommandPrefix + (commandTarget.Contains("#") ? commandTarget.Remove(commandTarget.IndexOf('#')) : commandTarget); }
        }

        public abstract MacroCommandType CommandType { get; }

        /// <summary>
        /// Return first command part (i.e. waittarget, use).
        /// </summary>
        /// <remarks>Command is constructed this way: Command + CommandType + [Arguments]</remarks>
        protected abstract string CommandPrefix { get; }

        protected virtual void OnCommandTargetChanged(EventArgs e)
        {
            OnCommandChanged(EventArgs.Empty);
        }

        protected virtual void OnCommandChanged(EventArgs e)
        {
            SyncEvent.Invoke(CommandChanged, this, e);
        }

        public virtual object[] Arguments
        {
            get
            {
                if (CommandTarget == "Object")
                    return new object[] { serial };
                else if (CommandTarget == "Type")
                    return new object[] { graphic };
                else if (CommandTarget == "Type#Color")
                    return new object[] { graphic, color };
                else
                    throw new InternalErrorException("Unknown command type.");
            }
        }

        public virtual TextCommand TextCommand
        {
            get { return new TextCommand(Command, Arguments); }
        }

        public virtual ToolStripDropDownItem[] CreateCustomMenu()
        {
            List<ToolStripDropDownItem> menuList = new List<ToolStripDropDownItem>();

            if (serial.IsValid) {
                ToolStripMenuItem objectMenuItem = new ToolStripMenuItem("Serial");
                objectMenuItem.ToolTipText = "Specific object.";
                objectMenuItem.Checked = CommandTarget == "Object";
                objectMenuItem.Click += new EventHandler(objectMenuItem_Click);
                menuList.Add(objectMenuItem);
            }

            if (!graphic.IsInvariant) {
                ToolStripMenuItem typeMenuItem = new ToolStripMenuItem("Type");
                typeMenuItem.ToolTipText = "Item type in backpack.";
                typeMenuItem.Checked = CommandTarget == "Type";
                typeMenuItem.Click += new EventHandler(typeMenuItem_Click);
                menuList.Add(typeMenuItem);

                ToolStripMenuItem typeColorMenuItem = new ToolStripMenuItem("Type && Color");
                typeColorMenuItem.ToolTipText = "Item type with specific color in backpack.";
                typeColorMenuItem.Checked = CommandTarget == "Type#Color";
                typeColorMenuItem.Click += new EventHandler(typeColorMenuItem_Click);
                menuList.Add(typeColorMenuItem);
            }

            return menuList.ToArray();
        }

        void objectMenuItem_Click(object sender, EventArgs e)
        {
            CommandTarget = "Object";
        }

        void typeMenuItem_Click(object sender, EventArgs e)
        {
            CommandTarget = "Type";
        }

        void typeColorMenuItem_Click(object sender, EventArgs e)
        {
            CommandTarget = "Type#Color";
        }

        public override string ToString()
        {
            string str = Command;

            foreach (object o in Arguments) {
                str += String.Format(" {0}", o);
            }

            return str;
        }
    }
}
