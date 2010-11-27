using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;
using Phoenix.Gui.Editor;

namespace Phoenix.Gui.Pages
{
    [PhoenixWindowTabPage("Phoenix.Properties.Resources.Page_Scripts", TitleIsResource = true, Position = 7, Icon = "Phoenix.Properties.Resources.ScriptsIcon")]
    public partial class ScriptsPage : UserControl
    {
        public ScriptsPage()
        {
            InitializeComponent();

            fileList.ScriptsDirectory = new System.IO.DirectoryInfo(RuntimeCore.ScriptsDirectory);
            fileList.NeedCompilationChanged += new EventHandler(fileList_NeedCompilationChanged);

            fileList.NeedCompilation = false;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // Some people reported Anchor misfunction
            fileList.Size = new Size(ClientSize.Width - 3 - fileList.Left, showReportsButton.Top - 6 - fileList.Top);
        }

        void fileList_NeedCompilationChanged(object sender, EventArgs e)
        {
            if (fileList.NeedCompilation)
                compileButton.ForeColor = Color.Red;
            else
                compileButton.ForeColor = SystemColors.ControlText;
        }

        private void compileButton_Click(object sender, EventArgs e)
        {
            RuntimeCore.CompileScripts();
            fileList.NeedCompilation = false;
        }

        private void showResultsButton_Click(object sender, EventArgs e)
        {
            ReportViewer.Show();
        }
    }
}
