using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Macros
{
    public class SpeechMacroCommand : IMacroCommand
    {
        private string text;
        private UOColor color;
        private bool useColor;

        public event EventHandler CommandChanged;

        public SpeechMacroCommand(UOColor color, string text)
        {
            this.color = color;
            this.text = text;

            useColor = true;
        }

        public string Text
        {
            get { return text; }
        }

        public UOColor Color
        {
            get { return color; }
        }

        public bool UseColor
        {
            get { return useColor; }
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
            List<ToolStripDropDownItem> menuList = new List<ToolStripDropDownItem>();

            ToolStripMenuItem ignoreColorMenuItem = new ToolStripMenuItem("Ignore color");
            ignoreColorMenuItem.Checked = !useColor;
            ignoreColorMenuItem.Click += new EventHandler(ignoreColorMenuItem_Click);
            menuList.Add(ignoreColorMenuItem);

            return menuList.ToArray();
        }

        void ignoreColorMenuItem_Click(object sender, EventArgs e)
        {
            useColor = !useColor;
            OnCommandChanged(EventArgs.Empty);
        }

        public TextCommand TextCommand
        {
            get
            {
                if (useColor)
                    return new TextCommand("say", Color, Text);
                else
                    return new TextCommand("say", Text);
            }
        }

        public override string ToString()
        {
            if (useColor)
                return String.Format("Say {0} \"{1}\"", Color, Text);
            else
                return String.Format("Say \"{0}\"", Text);
        }
    }
}
