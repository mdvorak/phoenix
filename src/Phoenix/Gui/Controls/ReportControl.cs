using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.CodeDom.Compiler;
using Phoenix.Collections;
using Phoenix.Runtime;

namespace Phoenix.Gui.Controls
{
    public partial class ReportControl : UserControl
    {
        private RuntimeObjectsLoaderReport report;

        public ReportControl()
        {
            InitializeComponent();

            outputTabPage.Selected = true;
            Disposed += new EventHandler(ReportControl_Disposed);
        }

        void ReportControl_Disposed(object sender, EventArgs e)
        {
            ResetReport();
        }

        public RuntimeObjectsLoaderReport Report
        {
            get { return report; }
        }

        /// <summary>
        /// This method is thread safe.
        /// </summary>
        public void ResetReport()
        {
            ResetEvents();
            ResetContent();
        }

        private void ResetEvents()
        {
            if (report != null)
            {
                report.Output.ItemInserted -= Output_ItemInserted;
                report.Output.ListCleared -= Output_ListCleared;
                report.CompilerErrors.ItemInserted -= CompilerErrors_ItemInserted;
                report.CompilerErrors.ListCleared -= CompilerErrors_ListCleared;
                report.AnalyzerErrors.ItemInserted -= AnalyzerErrors_ItemInserted;
                report.AnalyzerErrors.ListCleared -= AnalyzerErrors_ListCleared;
            }
        }

        private void ResetContent()
        {
            if (InvokeRequired)
            {
                Invoke(new ThreadStart(ResetContent));
            }
            else
            {
                outputTextBox.ResetText();
                compilerErrorsListBox.Items.Clear();
                analyzerErrorsListBox.Items.Clear();
            }
        }

        private delegate void SetReportDelegate(RuntimeObjectsLoaderReport value);

        /// <summary>
        /// This method is thread safe.
        /// </summary>
        /// <param name="value"></param>
        public void SetReport(RuntimeObjectsLoaderReport value)
        {
            if (InvokeRequired)
            {
                Invoke(new SetReportDelegate(SetReport), value);
            }
            else
            {
                if (value != report)
                {
                    ResetReport();

                    report = value;

                    if (report != null)
                    {
                        // Events
                        report.Output.ItemInserted += new ListItemChangeEventHandler<string>(Output_ItemInserted);
                        report.Output.ListCleared += new EventHandler(Output_ListCleared);
                        report.CompilerErrors.ItemInserted += new ListItemChangeEventHandler<CompilerError>(CompilerErrors_ItemInserted);
                        report.CompilerErrors.ListCleared += new EventHandler(CompilerErrors_ListCleared);
                        report.AnalyzerErrors.ItemInserted += new ListItemChangeEventHandler<AnalyzerError>(AnalyzerErrors_ItemInserted);
                        report.AnalyzerErrors.ListCleared += new EventHandler(AnalyzerErrors_ListCleared);

                        // Load current content
                        outputTextBox.Lines = report.Output.ToArray();
                        outputTextBox.Select();

                        foreach (CompilerError error in report.CompilerErrors)
                        {
                            compilerErrorsListBox.Items.Add(CreateCompilerErrorItem(error));
                        }

                        foreach (AnalyzerError error in report.AnalyzerErrors)
                        {
                            analyzerErrorsListBox.Items.Add(CreateAnalyzerErrorItem(error));
                        }
                    }
                }
            }
        }

        void AnalyzerErrors_ItemInserted(object sender, ListItemChangeEventArgs<AnalyzerError> e)
        {
            if (e.Index < analyzerErrorsListBox.Items.Count)
            {
                analyzerErrorsListBox.Items.Insert(e.Index, CreateAnalyzerErrorItem(e.Item));
            }
            else
            {
                analyzerErrorsListBox.Items.Add(CreateAnalyzerErrorItem(e.Item));
            }
        }

        void AnalyzerErrors_ListCleared(object sender, EventArgs e)
        {
            analyzerErrorsListBox.Items.Clear();
        }

        void CompilerErrors_ItemInserted(object sender, ListItemChangeEventArgs<CompilerError> e)
        {
            if (e.Index < compilerErrorsListBox.Items.Count)
            {
                compilerErrorsListBox.Items.Insert(e.Index, CreateCompilerErrorItem(e.Item));
            }
            else
            {
                compilerErrorsListBox.Items.Add(CreateCompilerErrorItem(e.Item));
            }
        }

        void CompilerErrors_ListCleared(object sender, EventArgs e)
        {
            compilerErrorsListBox.Items.Clear();
        }

        void Output_ItemInserted(object sender, ListItemChangeEventArgs<string> e)
        {
            try
            {
                outputTextBox.LockWindowUpdate();

                outputTextBox.AppendText(e.Item + Environment.NewLine);
            }
            finally
            {
                outputTextBox.Select(outputTextBox.TextLength, 0);
                outputTextBox.ScrollToBottom();

                outputTextBox.UnlockWindowUpdate();
            }
        }

        void Output_ListCleared(object sender, EventArgs e)
        {
            outputTextBox.ResetText();
        }

        protected ListViewItem CreateCompilerErrorItem(CompilerError error)
        {
            ListViewItem item = new ListViewItem(error.ErrorText);
            item.SubItems.Add(error.FileName);
            item.SubItems.Add(error.Line.ToString());
            item.SubItems.Add(error.Column.ToString());
            return item;
        }

        protected ListViewItem CreateAnalyzerErrorItem(AnalyzerError error)
        {
            ListViewItem item = new ListViewItem(error.Description);
            item.SubItems.Add(error.Path);
            item.SubItems.Add(error.Attribute);
            return item;
        }
    }
}
