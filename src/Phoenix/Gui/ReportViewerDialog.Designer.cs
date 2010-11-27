namespace Phoenix.Gui
{
    partial class ReportViewerDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportViewerDialog));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.reportListBox = new System.Windows.Forms.ListBox();
            this.reportControl = new Phoenix.Gui.Controls.ReportControl();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.reportListBox);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.reportControl);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.splitContainer.Size = new System.Drawing.Size(604, 366);
            this.splitContainer.SplitterDistance = 163;
            this.splitContainer.TabIndex = 1;
            // 
            // reportListBox
            // 
            this.reportListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportListBox.IntegralHeight = false;
            this.reportListBox.Location = new System.Drawing.Point(3, 3);
            this.reportListBox.Name = "reportListBox";
            this.reportListBox.Size = new System.Drawing.Size(157, 360);
            this.reportListBox.TabIndex = 0;
            this.reportListBox.SelectedIndexChanged += new System.EventHandler(this.reportListBox_SelectedIndexChanged);
            // 
            // reportControl
            // 
            this.reportControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportControl.Location = new System.Drawing.Point(3, 3);
            this.reportControl.Name = "reportControl";
            this.reportControl.Size = new System.Drawing.Size(431, 363);
            this.reportControl.TabIndex = 0;
            // 
            // ReportViewerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 366);
            this.Controls.Add(this.splitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportViewerDialog";
            this.Text = "Report Viewer";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Phoenix.Gui.Controls.ReportControl reportControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListBox reportListBox;
    }
}