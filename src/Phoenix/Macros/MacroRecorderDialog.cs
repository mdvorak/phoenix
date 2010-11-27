using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Macros
{
    public partial class MacroRecorderDialog : Form
    {
        public MacroRecorderDialog()
        {
            InitializeComponent();

            macroCommandList1.Macro = macroRecorder.Commands;
        }

        public Macro Macro
        {
            get { return macroRecorder.Commands; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            macroRecorder.Start();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!macroRecorder.Recording) {
                macroRecorder.Start();
            }
            else {
                macroRecorder.Stop();
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            macroRecorder.Commands.Clear();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            macroRecorder.Stop();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            macroRecorder.Stop();
            macroRecorder.Commands.Clear();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void insertWaitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            macroRecorder.Pause = insertWaitCheckBox.Checked ? 500 : -1;
        }

        private void macroRecorder_RecordingChanged(object sender, EventArgs e)
        {
            startButton.Text = macroRecorder.Recording ? "Stop" : "Start";
        }
    }
}