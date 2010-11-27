using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;
using Phoenix.Macros;
using Phoenix.Collections;
using System.Diagnostics;
using System.Threading;
using Phoenix.Runtime.Reflection;

namespace Phoenix.Gui.Pages
{
    [PhoenixWindowTabPage("Macros", Position = 8)]
    public partial class MacrosPage : UserControl
    {
        private MacroRecorder recorder = null;

        public MacrosPage()
        {
            InitializeComponent();

            RuntimeCore.Macros.ItemAdded += new DictionaryItemChangeEventHandler<string, Macro>(Macros_ItemAdded);
            RuntimeCore.Macros.ItemUpdated += new DictionaryItemUpdateEventHandler<string, Macro>(Macros_ItemUpdated);
            RuntimeCore.Macros.ItemRemoved += new DictionaryItemChangeEventHandler<string, Macro>(Macros_ItemRemoved);
            RuntimeCore.Macros.DictionaryCleared += new EventHandler(Macros_DictionaryCleared);
        }

        private ListViewItem SelectedItem
        {
            get { return (macrosList.SelectedItems.Count > 0) ? macrosList.SelectedItems[0] : null; }
        }

        void Macros_ItemAdded(object sender, DictionaryItemChangeEventArgs<string, Macro> e)
        {
            ListViewItem item = new ListViewItem(e.Key);
            item.Name = e.Key;
            item.Tag = e.Item;

            macrosList.Items.Add(item);
        }

        void Macros_ItemUpdated(object sender, DictionaryItemUpdateEventArgs<string, Macro> e)
        {
            if (SelectedItem != null && SelectedItem.Name == e.Key) {
                Debug.Assert(SelectedItem.Tag == e.OldItem);

                SelectedItem.Tag = e.Item;
                macroCommandList.Macro = e.Item;
            }
        }

        void Macros_ItemRemoved(object sender, DictionaryItemChangeEventArgs<string, Macro> e)
        {
            macrosList.Items.RemoveByKey(e.Key);

            // .NET bugfix
            macrosList_SelectedIndexChanged(macrosList, EventArgs.Empty);
        }

        void Macros_DictionaryCleared(object sender, EventArgs e)
        {
            macrosList.Items.Clear();
        }

        private void macrosList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedItem != null) {
                Debug.Assert(SelectedItem.Tag != null);
                macroCommandList.Macro = (Macro)SelectedItem.Tag;
            }
            else {
                macroCommandList.Macro = null;
            }

            deleteButton.Enabled = (SelectedItem != null);
        }

        private void macroCommandList_MacroChanged(object sender, EventArgs e)
        {
            if (recorder != null) {
                ((IDisposable)recorder).Dispose();
                recorder = null;
            }

            if (macroCommandList.Macro != null) {
                recorder = new MacroRecorder(macroCommandList.Macro);

                recordButton.Enabled = true;
                recordButton.Text = "Record";
                resetButton.Enabled = true;
                startButton.Enabled = true;
                loopButton.Enabled = true;
                macroCommandList.Enabled = true;
            }
            else {
                macroCommandList.Enabled = false;
                recordButton.Enabled = false;
                recordButton.Text = "Record";

                resetButton.Enabled = false;
                startButton.Enabled = false;
                loopButton.Enabled = false;
            }
        }

        private void recordButton_Click(object sender, EventArgs e)
        {
            Debug.Assert(recorder != null);

            if (recorder.Recording) {
                recorder.Stop();
                recordButton.Text = "Record";
                startButton.Enabled = true;
                loopButton.Enabled = true;
            }
            else {
                recorder.Start();
                recordButton.Text = "Stop";
                startButton.Enabled = false;
                loopButton.Enabled = false;
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            Debug.Assert(recorder != null);

            recorder.Commands.Clear();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            Macro macro = new Macro();
            string name = null;

            lock (RuntimeCore.Macros.SyncRoot) {
                int index = 1;

                while (RuntimeCore.Macros.ContainsKey("macro" + index.ToString()))
                    index++;

                name = "macro" + index.ToString();
                RuntimeCore.Macros.Add(name, macro);

                macrosList.Items[name].Selected = true;
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            Debug.Assert(SelectedItem.Tag != null);

            RuntimeCore.Macros.Remove(SelectedItem.Name);
        }

        private void macrosList_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label != null && e.Label.Length > 0) {
                lock (RuntimeCore.Macros.SyncRoot) {
                    string oldName = macrosList.Items[e.Item].Name;

                    if (RuntimeCore.Macros.ContainsKey(oldName)) {
                        Macro macro = RuntimeCore.Macros[oldName];

                        RuntimeCore.Macros.Remove(oldName);
                        RuntimeCore.Macros.Add(e.Label, macro);

                        macrosList.Items[e.Label].Selected = true;
                    }
                    else {
                        Trace.WriteLine("Unable to rename non-existing item. Internal error.", "Gui");
                    }
                }
            }
        }

        private void macrosList_SizeChanged(object sender, EventArgs e)
        {
            columnHeader.Width = macrosList.ClientSize.Width - SystemInformation.Border3DSize.Width * 2;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            Debug.Assert(SelectedItem != null, "macroCommandList.Macro");

            if (recorder.Recording) {
                recorder.Stop();
                recordButton.Text = "Record";
            }

            RuntimeCore.Executions.Execute(RuntimeCore.CommandList["macro"], SelectedItem.Name);
        }

        private void loopButton_Click(object sender, EventArgs e)
        {
            Debug.Assert(SelectedItem != null, "macroCommandList.Macro");

            if (recorder.Recording) {
                recorder.Stop();
                recordButton.Text = "Record";
            }

            RuntimeCore.Executions.Execute(RuntimeCore.CommandList["loopcmd"], "macro", SelectedItem.Name);
        }
    }
}
