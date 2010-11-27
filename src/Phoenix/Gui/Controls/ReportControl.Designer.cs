namespace Phoenix.Gui.Controls
{
    partial class ReportControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new Crownwood.Magic.Controls.TabControl();
            this.analyzerErrorsTabPage = new Crownwood.Magic.Controls.TabPage();
            this.analyzerErrorsListBox = new System.Windows.Forms.ListView();
            this.aeDescriptionHeader = new System.Windows.Forms.ColumnHeader();
            this.aePathHeader = new System.Windows.Forms.ColumnHeader();
            this.aeAttributeHeader = new System.Windows.Forms.ColumnHeader();
            this.outputTabPage = new Crownwood.Magic.Controls.TabPage();
            this.outputTextBox = new Phoenix.Gui.Controls.RichTextBoxEx();
            this.compilerErrorsTabPage = new Crownwood.Magic.Controls.TabPage();
            this.compilerErrorsListBox = new System.Windows.Forms.ListView();
            this.ceDescriptionHeader = new System.Windows.Forms.ColumnHeader();
            this.ceFileHeader = new System.Windows.Forms.ColumnHeader();
            this.ceLineHeader = new System.Windows.Forms.ColumnHeader();
            this.ceColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.tabControl.SuspendLayout();
            this.analyzerErrorsTabPage.SuspendLayout();
            this.outputTabPage.SuspendLayout();
            this.compilerErrorsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.AllowDrop = false;
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.IDEPixelArea = true;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 1;
            this.tabControl.SelectedTab = this.compilerErrorsTabPage;
            this.tabControl.Size = new System.Drawing.Size(509, 288);
            this.tabControl.TabIndex = 0;
            this.tabControl.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
            this.outputTabPage,
            this.compilerErrorsTabPage,
            this.analyzerErrorsTabPage});
            // 
            // analyzerErrorsTabPage
            // 
            this.analyzerErrorsTabPage.Controls.Add(this.analyzerErrorsListBox);
            this.analyzerErrorsTabPage.Location = new System.Drawing.Point(0, 0);
            this.analyzerErrorsTabPage.Name = "analyzerErrorsTabPage";
            this.analyzerErrorsTabPage.Size = new System.Drawing.Size(509, 263);
            this.analyzerErrorsTabPage.TabIndex = 5;
            this.analyzerErrorsTabPage.Title = "Analyzer Errors";
            // 
            // analyzerErrorsListBox
            // 
            this.analyzerErrorsListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.aeDescriptionHeader,
            this.aePathHeader,
            this.aeAttributeHeader});
            this.analyzerErrorsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analyzerErrorsListBox.FullRowSelect = true;
            this.analyzerErrorsListBox.GridLines = true;
            this.analyzerErrorsListBox.Location = new System.Drawing.Point(0, 0);
            this.analyzerErrorsListBox.Name = "analyzerErrorsListBox";
            this.analyzerErrorsListBox.Size = new System.Drawing.Size(509, 263);
            this.analyzerErrorsListBox.TabIndex = 0;
            this.analyzerErrorsListBox.UseCompatibleStateImageBehavior = false;
            this.analyzerErrorsListBox.View = System.Windows.Forms.View.Details;
            // 
            // aeDescriptionHeader
            // 
            this.aeDescriptionHeader.Text = "Description";
            this.aeDescriptionHeader.Width = 224;
            // 
            // aePathHeader
            // 
            this.aePathHeader.Text = "Path";
            this.aePathHeader.Width = 160;
            // 
            // aeAttributeHeader
            // 
            this.aeAttributeHeader.Text = "Attribute";
            this.aeAttributeHeader.Width = 160;
            // 
            // outputTabPage
            // 
            this.outputTabPage.Controls.Add(this.outputTextBox);
            this.outputTabPage.Location = new System.Drawing.Point(0, 0);
            this.outputTabPage.Name = "outputTabPage";
            this.outputTabPage.Selected = false;
            this.outputTabPage.Size = new System.Drawing.Size(509, 263);
            this.outputTabPage.TabIndex = 3;
            this.outputTabPage.Title = "Output";
            // 
            // outputTextBox
            // 
            this.outputTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.outputTextBox.DetectUrls = false;
            this.outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.outputTextBox.Location = new System.Drawing.Point(0, 0);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.Size = new System.Drawing.Size(509, 263);
            this.outputTextBox.TabIndex = 0;
            this.outputTextBox.Text = "";
            this.outputTextBox.WordWrap = false;
            // 
            // compilerErrorsTabPage
            // 
            this.compilerErrorsTabPage.Controls.Add(this.compilerErrorsListBox);
            this.compilerErrorsTabPage.Location = new System.Drawing.Point(0, 0);
            this.compilerErrorsTabPage.Name = "compilerErrorsTabPage";
            this.compilerErrorsTabPage.Size = new System.Drawing.Size(509, 263);
            this.compilerErrorsTabPage.TabIndex = 4;
            this.compilerErrorsTabPage.Title = "Compiler Errors";
            // 
            // compilerErrorsListBox
            // 
            this.compilerErrorsListBox.AllowColumnReorder = true;
            this.compilerErrorsListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ceDescriptionHeader,
            this.ceFileHeader,
            this.ceLineHeader,
            this.ceColumnHeader});
            this.compilerErrorsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compilerErrorsListBox.FullRowSelect = true;
            this.compilerErrorsListBox.GridLines = true;
            this.compilerErrorsListBox.Location = new System.Drawing.Point(0, 0);
            this.compilerErrorsListBox.Name = "compilerErrorsListBox";
            this.compilerErrorsListBox.Size = new System.Drawing.Size(509, 263);
            this.compilerErrorsListBox.TabIndex = 7;
            this.compilerErrorsListBox.UseCompatibleStateImageBehavior = false;
            this.compilerErrorsListBox.View = System.Windows.Forms.View.Details;
            // 
            // ceDescriptionHeader
            // 
            this.ceDescriptionHeader.Text = "Description";
            this.ceDescriptionHeader.Width = 300;
            // 
            // ceFileHeader
            // 
            this.ceFileHeader.Text = "File";
            this.ceFileHeader.Width = 80;
            // 
            // ceLineHeader
            // 
            this.ceLineHeader.Text = "Line";
            // 
            // ceColumnHeader
            // 
            this.ceColumnHeader.Text = "Column";
            // 
            // ReportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Name = "ReportControl";
            this.Size = new System.Drawing.Size(509, 288);
            this.tabControl.ResumeLayout(false);
            this.analyzerErrorsTabPage.ResumeLayout(false);
            this.outputTabPage.ResumeLayout(false);
            this.compilerErrorsTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Crownwood.Magic.Controls.TabControl tabControl;
        private Crownwood.Magic.Controls.TabPage outputTabPage;
        private Crownwood.Magic.Controls.TabPage compilerErrorsTabPage;
        private Crownwood.Magic.Controls.TabPage analyzerErrorsTabPage;
        private Phoenix.Gui.Controls.RichTextBoxEx outputTextBox;
        private System.Windows.Forms.ListView compilerErrorsListBox;
        private System.Windows.Forms.ColumnHeader ceDescriptionHeader;
        private System.Windows.Forms.ColumnHeader ceFileHeader;
        private System.Windows.Forms.ColumnHeader ceLineHeader;
        private System.Windows.Forms.ColumnHeader ceColumnHeader;
        private System.Windows.Forms.ListView analyzerErrorsListBox;
        private System.Windows.Forms.ColumnHeader aeDescriptionHeader;
        private System.Windows.Forms.ColumnHeader aePathHeader;
        private System.Windows.Forms.ColumnHeader aeAttributeHeader;
    }
}
