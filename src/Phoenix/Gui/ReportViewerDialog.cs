using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Phoenix.Collections;
using Phoenix.Runtime;
using System.Diagnostics;

namespace Phoenix.Gui
{
    public partial class ReportViewerDialog : Phoenix.Gui.Controls.FormEx
    {
        private ListEx<RuntimeObjectsLoaderReport> reportList;

        public ReportViewerDialog(ListEx<RuntimeObjectsLoaderReport> reportList)
        {
            InitializeComponent();

            this.reportList = reportList;

            lock (reportList.SyncRoot)
            {
                reportList.ItemInserted += new ListItemChangeEventHandler<RuntimeObjectsLoaderReport>(reportList_ItemInserted);
                reportList.ItemUpdated += new ListItemUpdateEventHandler<RuntimeObjectsLoaderReport>(reportList_ItemUpdated);
                reportList.ItemRemoved += new ListItemChangeEventHandler<RuntimeObjectsLoaderReport>(reportList_ItemRemoved);
                reportList.ListCleared += new EventHandler(reportList_ListCleared);

                Disposed += new EventHandler(ReportViewer_Disposed);

                foreach (RuntimeObjectsLoaderReport report in reportList)
                {
                    reportListBox.Items.Add(report);
                }

                if (reportListBox.Items.Count > 0)
                    reportListBox.SelectedIndex = 0;
            }
        }

        public ListEx<RuntimeObjectsLoaderReport> ReportList
        {
            get { return reportList; }
        }

        public RuntimeObjectsLoaderReport SelectedReport
        {
            get { return (RuntimeObjectsLoaderReport)reportListBox.SelectedItem; }
            set { reportListBox.SelectedItem = value; }
        }

        void ReportViewer_Disposed(object sender, EventArgs e)
        {
            lock (reportList.SyncRoot)
            {
                reportList.ItemInserted -= reportList_ItemInserted;
                reportList.ItemUpdated -= reportList_ItemUpdated;
                reportList.ItemRemoved -= reportList_ItemRemoved;
                reportList.ListCleared -= reportList_ListCleared;
            }
        }

        void reportList_ListCleared(object sender, EventArgs e)
        {
            reportListBox.Items.Clear();
        }

        void reportList_ItemUpdated(object sender, ListItemUpdateEventArgs<RuntimeObjectsLoaderReport> e)
        {
            reportListBox.Items[e.Index] = e.Item;
        }

        void reportList_ItemRemoved(object sender, ListItemChangeEventArgs<RuntimeObjectsLoaderReport> e)
        {
            Debug.Assert(e.Index == reportListBox.Items.IndexOf(e.Item));
            reportListBox.Items.Remove(e.Item);
        }

        void reportList_ItemInserted(object sender, ListItemChangeEventArgs<RuntimeObjectsLoaderReport> e)
        {
            reportListBox.Items.Insert(e.Index, e.Item);

            if (reportListBox.SelectedItem == null)
                reportListBox.SelectedItem = e.Item;
        }

        private void reportListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (reportListBox.SelectedItem != null)
            {
                reportControl.SetReport((RuntimeObjectsLoaderReport)reportListBox.SelectedItem);
            }
            else
            {
                reportControl.ResetReport();
            }
        }
    }
}