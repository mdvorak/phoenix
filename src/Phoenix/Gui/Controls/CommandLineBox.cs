using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Gui.Controls
{
    public partial class CommandLineBox : RichTextBox
    {
        private const int WM_KEYDOWN = 0x0100;

        private CommandBuilderDialog builder;

        public CommandLineBox()
        {
            InitializeComponent();

            builder = new CommandBuilderDialog();
            builder.FormClosing += new FormClosingEventHandler(builder_FormClosing);
            builder.InsertText += new InsertTextDelegate(builder_InsertText);
        }

        void builder_InsertText(object sender, string text)
        {
            SelectedText = text;
        }

        protected override void OnMultilineChanged(EventArgs e)
        {
            if (!Multiline && Text.Contains("\n")) {
                Text = Text.Remove(Text.IndexOf('\n')).Replace("\r", "");
            }

            base.OnMultilineChanged(e);
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            if (msg.Msg == WM_KEYDOWN) {
                KeyEventArgs e = new KeyEventArgs((Keys)msg.WParam);

                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V) {
                    Paste();
                }
            }

            return base.PreProcessMessage(ref msg);
        }

        public virtual new void Paste()
        {
            if (Clipboard.ContainsText()) {
                SelectedText = Clipboard.GetText();
            }
        }

        void builder_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            builder.Hide();
        }

        private void showBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            builder.Show();
            builder.Focus();
        }

        private void recordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Macros.MacroRecorderDialog dlg = new Phoenix.Macros.MacroRecorderDialog();

            if (dlg.ShowDialog() == DialogResult.OK) {
                StringBuilder macro = new StringBuilder();

                foreach (Macros.IMacroCommand cmd in dlg.Macro) {
                    macro.AppendLine(cmd.TextCommand.ToString());
                }

                SelectedText = macro.ToString();
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            cutToolStripMenuItem.Enabled = SelectionLength > 0;
            copyToolStripMenuItem.Enabled = SelectionLength > 0;
            pasteToolStripMenuItem.Enabled = Clipboard.ContainsText();
        }
    }
}
