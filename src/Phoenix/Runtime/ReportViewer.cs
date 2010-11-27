using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Phoenix.Gui;

namespace Phoenix.Runtime
{
    public static class ReportViewer
    {
        private static ReportViewerDialog dialog;

        internal static void Init()
        {
            if (Core.GuiThread.InvokeRequired)
            {
                Core.GuiThread.InvokeFast(new MethodInvoker(Init));
            }
            else
            {
                dialog = new ReportViewerDialog(RuntimeCore.Reports);
                dialog.FormClosing += new FormClosingEventHandler(dialog_FormClosing);
                dialog.CreateWindow();
            }
        }

        static void dialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            dialog.Hide();
        }

        public static void Show()
        {
            if (Core.GuiThread.InvokeRequired)
            {
                Core.GuiThread.InvokeFast(new MethodInvoker(Show));
            }
            else
            {
                dialog.Show();
                dialog.Focus();
            }
        }

        private delegate void ShowDelegate(RuntimeObjectsLoaderReport selectedReport);
        public static void Show(RuntimeObjectsLoaderReport selectedReport)
        {
            if (Core.GuiThread.InvokeRequired)
            {
                Core.GuiThread.InvokeFast(new ShowDelegate(Show), selectedReport);
            }
            else
            {
                dialog.SelectedReport = selectedReport;
                dialog.Show();
                dialog.Focus();
            }
        }
    }
}
