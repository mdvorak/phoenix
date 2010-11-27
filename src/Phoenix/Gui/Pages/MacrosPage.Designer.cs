namespace Phoenix.Gui.Pages
{
    partial class MacrosPage
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
            if (disposing && (components != null)) {
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
            this.components = new System.ComponentModel.Container();
            this.recordButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.macrosList = new System.Windows.Forms.ListView();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.deleteButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.loopButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.macrosListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.macroCommandList = new Phoenix.Macros.MacroCommandList();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.macrosListContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // recordButton
            // 
            this.recordButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.recordButton.Enabled = false;
            this.recordButton.Location = new System.Drawing.Point(3, 249);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(75, 23);
            this.recordButton.TabIndex = 1;
            this.recordButton.Text = "Record";
            this.recordButton.UseVisualStyleBackColor = true;
            this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.macrosList);
            this.splitContainer1.Panel1.Controls.Add(this.deleteButton);
            this.splitContainer1.Panel1.Controls.Add(this.newButton);
            this.splitContainer1.Panel1MinSize = 25;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.loopButton);
            this.splitContainer1.Panel2.Controls.Add(this.resetButton);
            this.splitContainer1.Panel2.Controls.Add(this.startButton);
            this.splitContainer1.Panel2.Controls.Add(this.recordButton);
            this.splitContainer1.Panel2.Controls.Add(this.macroCommandList);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2MinSize = 25;
            this.splitContainer1.Size = new System.Drawing.Size(333, 304);
            this.splitContainer1.SplitterDistance = 81;
            this.splitContainer1.TabIndex = 2;
            // 
            // macrosList
            // 
            this.macrosList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.macrosList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
            this.macrosList.FullRowSelect = true;
            this.macrosList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.macrosList.HideSelection = false;
            this.macrosList.LabelEdit = true;
            this.macrosList.LabelWrap = false;
            this.macrosList.Location = new System.Drawing.Point(0, 0);
            this.macrosList.MultiSelect = false;
            this.macrosList.Name = "macrosList";
            this.macrosList.ShowGroups = false;
            this.macrosList.Size = new System.Drawing.Size(82, 243);
            this.macrosList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.macrosList.TabIndex = 4;
            this.macrosList.UseCompatibleStateImageBehavior = false;
            this.macrosList.View = System.Windows.Forms.View.Details;
            this.macrosList.SelectedIndexChanged += new System.EventHandler(this.macrosList_SelectedIndexChanged);
            this.macrosList.SizeChanged += new System.EventHandler(this.macrosList_SizeChanged);
            this.macrosList.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.macrosList_AfterLabelEdit);
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "Name";
            this.columnHeader.Width = 125;
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(3, 278);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(79, 23);
            this.deleteButton.TabIndex = 3;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // newButton
            // 
            this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newButton.Location = new System.Drawing.Point(3, 249);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(79, 23);
            this.newButton.TabIndex = 1;
            this.newButton.Text = "New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // loopButton
            // 
            this.loopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.loopButton.Enabled = false;
            this.loopButton.Location = new System.Drawing.Point(84, 278);
            this.loopButton.Name = "loopButton";
            this.loopButton.Size = new System.Drawing.Size(75, 23);
            this.loopButton.TabIndex = 5;
            this.loopButton.Text = "Loop";
            this.loopButton.UseVisualStyleBackColor = true;
            this.loopButton.Click += new System.EventHandler(this.loopButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.resetButton.Enabled = false;
            this.resetButton.Location = new System.Drawing.Point(84, 249);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 2;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point(3, 278);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 4;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label1.Location = new System.Drawing.Point(165, 254);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 47);
            this.label1.TabIndex = 6;
            this.label1.Text = "Note: not working completely";
            // 
            // macrosListContextMenu
            // 
            this.macrosListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem,
            this.renameToolStripMenuItem});
            this.macrosListContextMenu.Name = "macrosListContextMenu";
            this.macrosListContextMenu.Size = new System.Drawing.Size(125, 48);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.renameToolStripMenuItem.Text = "R&ename";
            // 
            // macroCommandList
            // 
            this.macroCommandList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.macroCommandList.Enabled = false;
            this.macroCommandList.Location = new System.Drawing.Point(3, 0);
            this.macroCommandList.Macro = null;
            this.macroCommandList.Name = "macroCommandList";
            this.macroCommandList.Size = new System.Drawing.Size(242, 243);
            this.macroCommandList.TabIndex = 0;
            this.macroCommandList.MacroChanged += new System.EventHandler(this.macroCommandList_MacroChanged);
            // 
            // MacrosPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MacrosPage";
            this.Size = new System.Drawing.Size(333, 304);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.macrosListContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Phoenix.Macros.MacroCommandList macroCommandList;
        private System.Windows.Forms.Button recordButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.ListView macrosList;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ContextMenuStrip macrosListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button loopButton;
        private System.Windows.Forms.Label label1;
    }
}
