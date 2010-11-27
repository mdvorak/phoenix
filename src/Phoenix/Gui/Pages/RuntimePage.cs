using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Phoenix.Runtime;
using System.Diagnostics;

namespace Phoenix.Gui.Pages
{
    [PhoenixWindowTabPage("Phoenix.Properties.Resources.Page_Runtime", TitleIsResource = true, Position = 6, Icon = "Phoenix.Properties.Resources.RuntimeIcon")]
    public partial class RuntimePage : UserControl
    {
        public RuntimePage()
        {
            InitializeComponent();

            Config.Profile.InternalSettings.Loaded += new EventHandler(Settings_Loaded);
            Config.Profile.InternalSettings.Saving += new EventHandler(Settings_Saving);

            RuntimeCore.Executions.ExecutionStarted += new ExecutionsChangedEventHandler(Executions_ExecutionStarted);
            RuntimeCore.Executions.ExecutionFinished += new ExecutionsChangedEventHandler(Executions_ExecutionFinished);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // Some people reported Anchor misfunction
            runningListBox.Size = new Size(splitContainer.Panel1.ClientSize.Width - 3 - runningListBox.Left, terminateButton.Top - 6 - runningListBox.Top);
        }

        void Settings_Loaded(object sender, EventArgs e)
        {
            try {
                int splitter = Config.Profile.InternalSettings.GetAttribute(-1, "splitter", "Config", "Window", "RuntimePage");

                if (splitter < 0)
                    System.Diagnostics.Trace.WriteLine("Invalid SplitterDistance for RuntimePage.", "Gui");
                else
                    splitContainer.SplitterDistance = splitContainer.ClientSize.Height - splitter;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Unable to load splitter distance from settings. Exception:\n" + ex.ToString(), "Gui");
            }
        }

        void Settings_Saving(object sender, EventArgs e)
        {
            Config.Profile.InternalSettings.SetAttribute(splitContainer.ClientSize.Height - splitContainer.SplitterDistance, "splitter", "Config", "Window", "RuntimePage");
        }

        void Executions_ExecutionStarted(object sender, ExecutionsChangedEventArgs e)
        {
            runningListBox.Items.Add(e.ExecutionInfo);
        }

        void Executions_ExecutionFinished(object sender, ExecutionsChangedEventArgs e)
        {
            runningListBox.Items.Remove(e.ExecutionInfo);
        }

        private void runningListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            terminateButton.Enabled = runningListBox.SelectedItem != null;
        }

        private void terminateButton_Click(object sender, EventArgs e)
        {
            if (runningListBox.SelectedItem != null) {
                ExecutionInfo ei = (ExecutionInfo)runningListBox.SelectedItem;
                ei.Thread.Abort();
            }

            terminateButton.Enabled = runningListBox.SelectedItem != null;
        }

        private void terminateAllButton_Click(object sender, EventArgs e)
        {
            RuntimeCore.Executions.TerminateAll();
        }


        private void runButton_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void commandbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Control) {
                Run();
                e.SuppressKeyPress = true;
            }
        }

        private void Run()
        {
            CommandList cl = new CommandList(commandBox.Text);
            ThreadStarter.StartBackround(new RunWorkerDelegate(RunWorker), cl);
        }

        private delegate void RunWorkerDelegate(CommandList cmdList);
        private static void RunWorker(CommandList cmdList)
        {
            try {
                cmdList.Run();
            }
            catch (ThreadAbortException) {
                UO.PrintInformation("Command execution terminated.");
            }
            catch (ScriptErrorException e) {
                Trace.WriteLine("Unhandled exception:\n" + e.ToString(), "Script");
                UO.PrintError(e.Message);
            }
            catch (Exception e) {
                Trace.WriteLine("Unhandled error during command execution. Exception:" + Environment.NewLine + e.ToString(), "Runtime");
                UO.PrintError("Command error: {0}", e.Message);
            }
        }
    }
}
