using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Macros
{
    public class WaitMacroCommand : IMacroCommand
    {
        private int pause;
        public event EventHandler CommandChanged;

        public WaitMacroCommand()
        {
            Pause = 500;
        }

        public WaitMacroCommand(int pause)
        {
            Pause = pause;
        }

        public int Pause
        {
            get { return pause; }
            set
            {
                if (value != pause) {
                    pause = value;
                    OnPauseChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnPauseChanged(EventArgs e)
        {
            OnCommandChanged(EventArgs.Empty);
        }

        protected virtual void OnCommandChanged(EventArgs e)
        {
            SyncEvent.Invoke(CommandChanged, this, e);
        }

        public override string ToString()
        {
            return String.Format("Wait {0} ms", pause);
        }

        ToolStripDropDownItem[] IMacroCommand.CreateCustomMenu()
        {
            ToolStripMenuItem editMenuItem = new ToolStripMenuItem("Edit Pause");
            editMenuItem.Click += new EventHandler(editMenuItem_Click);

            return new ToolStripDropDownItem[] { editMenuItem };
        }

        void editMenuItem_Click(object sender, EventArgs e)
        {
            using (ChangePauseDialog dlg = new ChangePauseDialog()) {
                dlg.Pause = Pause;
                if (dlg.ShowDialog() == DialogResult.OK) {
                    Pause = dlg.Pause;
                }
            }
        }

        public virtual TextCommand TextCommand
        {
            get { return new TextCommand("wait", pause); }
        }

        public MacroCommandType CommandType
        {
            get { return MacroCommandType.Pasive; }
        }

        /// <summary>
        /// Displays Change Pause dialog and 
        /// returns WaitMacroCommand object with specified pause or null.
        /// </summary>
        public static WaitMacroCommand PromptUser(int pause)
        {
            WaitMacroCommand cmd = null;

            using (ChangePauseDialog dlg = new ChangePauseDialog()) {
                dlg.Pause = pause;
                if (dlg.ShowDialog() == DialogResult.OK) {
                    cmd = new WaitMacroCommand();
                    cmd.Pause = dlg.Pause;
                }
            }

            return cmd;
        }
    }
}
