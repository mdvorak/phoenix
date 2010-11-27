namespace Phoenix.Gui.Editor
{
    partial class FileList
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
            this.components = new System.ComponentModel.Container();
            this.treeView = new System.Windows.Forms.TreeView();
            this.dummyContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.iconList = new System.Windows.Forms.ImageList(this.components);
            this.fileWatcher = new System.IO.FileSystemWatcher();
            this.directoryWatcher = new System.IO.FileSystemWatcher();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuIcons = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.fileWatcher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.directoryWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            this.treeView.ContextMenuStrip = this.dummyContextMenu;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.ImageKey = "unknown";
            this.treeView.ImageList = this.iconList;
            this.treeView.LabelEdit = true;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageKey = "unknown";
            this.treeView.Size = new System.Drawing.Size(278, 268);
            this.treeView.Sorted = true;
            this.treeView.TabIndex = 0;
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
            this.treeView.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.treeView_QueryContinueDrag);
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
            this.treeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView_NodeToggled);
            this.treeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseUp);
            this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseMove);
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            this.treeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_BeforeLabelEdit);
            this.treeView.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            this.treeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeView_NodeToggled);
            // 
            // dummyContextMenu
            // 
            this.dummyContextMenu.Enabled = true;
            this.dummyContextMenu.GripMargin = new System.Windows.Forms.Padding(2);
            this.dummyContextMenu.Location = new System.Drawing.Point(21, 36);
            this.dummyContextMenu.Name = "dummyContextMenu";
            this.dummyContextMenu.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dummyContextMenu.Size = new System.Drawing.Size(61, 4);
            this.dummyContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.dummyContextMenu_Opening);
            // 
            // iconList
            // 
            this.iconList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.iconList.ImageSize = new System.Drawing.Size(16, 16);
            this.iconList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // fileWatcher
            // 
            this.fileWatcher.EnableRaisingEvents = true;
            this.fileWatcher.IncludeSubdirectories = true;
            this.fileWatcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite)));
            this.fileWatcher.SynchronizingObject = this;
            this.fileWatcher.Renamed += new System.IO.RenamedEventHandler(this.fileWatcher_Renamed);
            this.fileWatcher.Deleted += new System.IO.FileSystemEventHandler(this.fileWatcher_Changed);
            this.fileWatcher.Created += new System.IO.FileSystemEventHandler(this.fileWatcher_Changed);
            this.fileWatcher.Changed += new System.IO.FileSystemEventHandler(this.fileWatcher_Changed);
            // 
            // directoryWatcher
            // 
            this.directoryWatcher.EnableRaisingEvents = true;
            this.directoryWatcher.Filter = "*";
            this.directoryWatcher.IncludeSubdirectories = true;
            this.directoryWatcher.NotifyFilter = System.IO.NotifyFilters.DirectoryName;
            this.directoryWatcher.SynchronizingObject = this;
            this.directoryWatcher.Renamed += new System.IO.RenamedEventHandler(this.directoryWatcher_Renamed);
            this.directoryWatcher.Deleted += new System.IO.FileSystemEventHandler(this.directoryWatcher_Changed);
            this.directoryWatcher.Created += new System.IO.FileSystemEventHandler(this.directoryWatcher_Changed);
            this.directoryWatcher.Changed += new System.IO.FileSystemEventHandler(this.directoryWatcher_Changed);
            // 
            // openFileDialog
            // 
            this.openFileDialog.AddExtension = false;
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.RestoreDirectory = true;
            // 
            // menuIcons
            // 
            this.menuIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.menuIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.menuIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // FileList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView);
            this.Name = "FileList";
            this.Size = new System.Drawing.Size(278, 268);
            ((System.ComponentModel.ISupportInitialize)(this.fileWatcher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.directoryWatcher)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.IO.FileSystemWatcher fileWatcher;
        private System.IO.FileSystemWatcher directoryWatcher;
        private System.Windows.Forms.ImageList iconList;
        private System.Windows.Forms.ContextMenuStrip dummyContextMenu;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ImageList menuIcons;
    }
}
