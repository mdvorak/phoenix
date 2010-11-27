using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Crownwood.Magic.Menus;
using Phoenix.Properties;

namespace Phoenix.Gui.Editor
{
    public partial class FileList : UserControl
    {
        private const string RootDirErrMsg = "Root directory doesn't exist.";
        private const string FolderOpenedImageKey = "FolderOpened";
        private const string FolderClosedImageKey = "FolderClosed";

        #region FileTreeNode class

        private class FileTreeNode : TreeNode
        {
            private FileSystemObject file;

            public FileTreeNode()
            {
            }

            public FileSystemObject File
            {
                get { return file; }
                set
                {
                    file = value;
                    if (file != null) {
                        Text = file.Info.Name;
                        Name = Text;

                        if (file.Type == FileSystemObjectType.File) {
                            ImageKey = file.Info.Extension;
                        }
                        else if (file.Type == FileSystemObjectType.Directory) {
                            ImageKey = IsExpanded ? FolderOpenedImageKey : FolderClosedImageKey;
                        }
                        else ImageKey = "";
                    }
                    else {
                        ImageKey = "";
                        Text = "";
                        Name = null;
                    }

                    SelectedImageKey = ImageKey;
                }
            }
        }

        #endregion

        [Serializable]
        private class CutFileList
        {
            public readonly StringCollection Files = new StringCollection();
        }

        private List<string> extensions;

        private DirectoryInfo scriptsDirectory;
        private FileTreeNode rootNode;
        private PopupMenu dirMenu;
        private PopupMenu fileMenu;
        private bool needCompilation;

        private bool showAllFiles;

        private string editItem = null;

        [Category("Property Changed")]
        public event EventHandler ScriptsDirectoryChanged;
        [Category("Property Changed")]
        public event EventHandler ShowAllFilesChanged;
        [Category("Property Changed")]
        public event EventHandler NeedCompilationChanged;

        public FileList()
        {
            extensions = new List<string>();
            extensions.Add(".cs");
            extensions.Add(".vb");
            extensions.Add(".resx");
            extensions.Add(".boo");

            InitializeComponent();
            InitializeMenu();

            openFileDialog.Filter = @"All known formats (*.cs;*.vb;*.resx;*.boo)|*.cs;*.vb;*.resx;*.boo|C# files (*.cs)|*.cs|VisualBasic files (*.vb)|*.vb|Boo Files (*.boo)|*.boo|Resource files (*.resx)|*.resx|All files (*.*)|*.*";

            iconList.Images.Add(".cs", Resources.cs);
            iconList.Images.Add(".vb", Resources.vb);
            iconList.Images.Add(".jsl", Resources.jsl);
            iconList.Images.Add(".boo", Resources.boo);
            iconList.Images.Add(".resx", Resources.resx);
            iconList.Images.Add("unknown", Resources.unknown);
            iconList.Images.Add("FolderOpened", Resources.FolderOpened);
            iconList.Images.Add("FolderClosed", Resources.FolderClosed);

            rootNode = new FileTreeNode();
            rootNode.Text = RootDirErrMsg;
            rootNode.ImageKey = "FolderOpened";
            rootNode.SelectedImageKey = rootNode.ImageKey;
            rootNode.Expand();

            treeView.Nodes.Add(rootNode);

            showAllFiles = false;
            needCompilation = true;
        }

        private void InitializeMenu()
        {
            menuIcons.Images.Add(Resources.FolderOpened);
            menuIcons.Images.Add(Resources.FolderClosed);

            fileMenu = new PopupMenu();
            MenuCommand edit = new MenuCommand("Edit");

            edit.ImageList = menuIcons;

            edit.Click += new EventHandler(edit_Click);

            fileMenu.MenuCommands.Add(edit);
            fileMenu.MenuCommands.Add(new MenuCommand("-"));
            AddDefaultCommands(fileMenu);
            fileMenu.MenuCommands["Paste"].Visible = false;

            dirMenu = new PopupMenu();
            MenuCommand add = new MenuCommand("Add");
            MenuCommand open = new MenuCommand("Open");
            MenuCommand expandAll = new MenuCommand("Expand All");
            MenuCommand collapseAll = new MenuCommand("Collapse All");

            add.ImageList = menuIcons;
            open.ImageList = menuIcons;
            expandAll.ImageList = menuIcons;
            collapseAll.ImageList = menuIcons;

            open.ImageIndex = 0;

            open.Click += new EventHandler(open_Click);
            expandAll.Click += new EventHandler(expandAll_Click);
            collapseAll.Click += new EventHandler(collapseAll_Click);

            MenuCommand addFile = new MenuCommand("New File");
            MenuCommand addExisting = new MenuCommand("Existing File");
            MenuCommand addFolder = new MenuCommand("New Folder");

            addFile.ImageList = menuIcons;
            addExisting.ImageList = menuIcons;
            addFolder.ImageList = menuIcons;

            addFolder.ImageIndex = 1;

            addFile.Click += new EventHandler(addFile_Click);
            addExisting.Click += new EventHandler(addExisting_Click);
            addFolder.Click += new EventHandler(addFolder_Click);

            add.MenuCommands.Add(addFile);
            add.MenuCommands.Add(addExisting);
            add.MenuCommands.Add(addFolder);

            dirMenu.MenuCommands.Add(add);
            dirMenu.MenuCommands.Add(open);
            dirMenu.MenuCommands.Add(new MenuCommand("-"));
            dirMenu.MenuCommands.Add(expandAll);
            dirMenu.MenuCommands.Add(collapseAll);
            dirMenu.MenuCommands.Add(new MenuCommand("-"));
            AddDefaultCommands(dirMenu);
        }

        private void AddDefaultCommands(PopupMenu menu)
        {
            MenuCommand cut = new MenuCommand("Cut");
            MenuCommand copy = new MenuCommand("Copy");
            MenuCommand paste = new MenuCommand("Paste");
            MenuCommand delete = new MenuCommand("Delete");
            MenuCommand rename = new MenuCommand("Rename");
            MenuCommand reload = new MenuCommand("Reload");
            MenuCommand showAllFiles = new MenuCommand("Show All Files");

            cut.ImageList = menuIcons;
            copy.ImageList = menuIcons;
            paste.ImageList = menuIcons;
            delete.ImageList = menuIcons;
            rename.ImageList = menuIcons;
            reload.ImageList = menuIcons;
            showAllFiles.ImageList = menuIcons;

            cut.Click += new EventHandler(cut_Click);
            copy.Click += new EventHandler(copy_Click);
            paste.Click += new EventHandler(paste_Click);
            delete.Click += new EventHandler(delete_Click);
            rename.Click += new EventHandler(rename_Click);
            reload.Click += new EventHandler(reload_Click);
            showAllFiles.Click += new EventHandler(showAllFiles_Click);

            menu.MenuCommands.Add(cut);
            menu.MenuCommands.Add(copy);
            menu.MenuCommands.Add(paste);
            menu.MenuCommands.Add(delete);
            menu.MenuCommands.Add(rename);
            menu.MenuCommands.Add(new MenuCommand("-"));
            menu.MenuCommands.Add(reload);
            menu.MenuCommands.Add(showAllFiles);
        }

        [Category("Behaviour")]
        [DefaultValue(null)]
        public DirectoryInfo ScriptsDirectory
        {
            get { return scriptsDirectory; }
            set
            {
                if (value != scriptsDirectory) {
                    scriptsDirectory = value;
                    OnScriptsDirectoryChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowAllFiles
        {
            get { return showAllFiles; }
            set
            {
                if (value != showAllFiles) {
                    showAllFiles = value;
                    OnShowAllFilesChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(true)]
        public bool NeedCompilation
        {
            get { return needCompilation; }
            set
            {
                if (value != needCompilation) {
                    needCompilation = value;
                    OnNeedCompilationChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnScriptsDirectoryChanged(EventArgs e)
        {
            if (scriptsDirectory != null) {
                openFileDialog.InitialDirectory = scriptsDirectory.FullName;

                try {
                    fileWatcher.Path = scriptsDirectory.FullName;
                    directoryWatcher.Path = scriptsDirectory.FullName;

                    fileWatcher.EnableRaisingEvents = true;
                    directoryWatcher.EnableRaisingEvents = true;
                }
                catch (PlatformNotSupportedException ex) {
                    string msg = "Error: " + ex.ToString() + "\n" +
                                 "Scripts tab will not update itself. To see any changes use 'Reload Scripts' command from file tree context menu.";
                    MessageBox.Show(msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                NeedCompilation = true;
            }
            else {
                try {
                    fileWatcher.EnableRaisingEvents = false;
                    directoryWatcher.EnableRaisingEvents = false;
                }
                catch (PlatformNotSupportedException) {
                }
            }

            ReloadScripts();

            if (ScriptsDirectoryChanged != null) ScriptsDirectoryChanged(this, e);
        }

        protected virtual void OnShowAllFilesChanged(EventArgs e)
        {
            ReloadScripts();

            if (ShowAllFilesChanged != null) ShowAllFilesChanged(this, e);
        }

        protected virtual void OnNeedCompilationChanged(EventArgs e)
        {
            if (NeedCompilationChanged != null) NeedCompilationChanged(this, e);
        }

        public FileInfo[] GetFileList()
        {
            List<FileInfo> fileList = new List<FileInfo>(32);

            for (int i = 0; i < extensions.Count; i++) {
                fileList.AddRange(scriptsDirectory.GetFiles("*" + extensions[i], SearchOption.AllDirectories));
            }

            return fileList.ToArray();
        }

        public void ReloadScripts()
        {
            rootNode.Nodes.Clear();
            rootNode.File = (FileSystemObject)scriptsDirectory;

            if (scriptsDirectory != null && scriptsDirectory.Exists) {
                ReloadDir(rootNode);
                rootNode.Expand();
            }
            else {
                rootNode.Text = RootDirErrMsg;
            }
        }

        /// <returns>Added files count.</returns>
        private int ReloadDir(FileTreeNode node)
        {
            DirectoryInfo dir = node.File.Info as DirectoryInfo;
            Debug.Assert(dir != null, "dir != null");

            node.Nodes.Clear();

            int count = 0;

            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo f in files) {
                if (showAllFiles || extensions.Contains(f.Extension)) {
                    FileTreeNode n = new FileTreeNode();
                    n.File = (FileSystemObject)f;
                    node.Nodes.Add(n);
                    count++;
                }
            }

            DirectoryInfo[] directories = dir.GetDirectories();

            foreach (DirectoryInfo d in directories) {
                FileTreeNode n = new FileTreeNode();
                n.File = (FileSystemObject)d;
                node.Nodes.Add(n);

                count += ReloadDir(n);
            }

            if (count > 0) {
                node.Expand();
                NeedCompilation = true;
            }

            return count;
        }

        private void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (showAllFiles || extensions.Contains(Path.GetExtension(e.FullPath))) {
                string[] path = FileList.SplitPath(e.Name);

                FileTreeNode node = rootNode;
                for (int i = 0; i < path.Length - 1; i++) {
                    Debug.Assert(node.File.Type == FileSystemObjectType.Directory);

                    FileTreeNode childNode = (FileTreeNode)node.Nodes[path[i]];

                    if (childNode == null) {
                        ReloadDir(node);
                        return;
                    }

                    node = childNode;
                }

                switch (e.ChangeType) {
                    case WatcherChangeTypes.Created:
                        FileTreeNode n = new FileTreeNode();
                        n.File = new FileSystemObject(new FileInfo(e.FullPath));
                        node.Nodes.Add(n);
                        node.Expand();
                        if (n.FullPath == editItem) {
                            editItem = null;
                            n.EnsureVisible();
                            n.BeginEdit();
                        }
                        break;

                    case WatcherChangeTypes.Deleted:
                        node.Nodes.RemoveByKey(path[path.Length - 1]); // Name
                        break;
                }

                NeedCompilation = true;
            }
        }

        private void fileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (showAllFiles || extensions.Contains(Path.GetExtension(e.OldFullPath))) {
                string[] path = FileList.SplitPath(e.OldName);

                FileTreeNode node = rootNode;
                for (int i = 0; i < path.Length; i++) {
                    FileTreeNode childNode = (FileTreeNode)node.Nodes[path[i]];

                    if (childNode == null) {
                        ReloadDir(node);
                        return;
                    }

                    node = childNode;
                }

                Debug.Assert(node.File.Type == FileSystemObjectType.File);

                string[] newPath = SplitPath(e.Name);
                string name = newPath[newPath.Length - 1];

                if (showAllFiles || extensions.Contains(Path.GetExtension(name))) {
                    node.File = new FileSystemObject(new FileInfo(e.FullPath));
                }
                else {
                    node.Remove();
                }

                NeedCompilation = true;
            }
        }

        private void directoryWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            string[] path = FileList.SplitPath(e.Name);

            FileTreeNode node = rootNode;
            for (int i = 0; i < path.Length - 1; i++) {
                Debug.Assert(node.File.Type == FileSystemObjectType.Directory);

                FileTreeNode childNode = (FileTreeNode)node.Nodes[path[i]];

                if (childNode == null) {
                    ReloadDir(node);
                    return;
                }

                node = childNode;
            }

            switch (e.ChangeType) {
                case WatcherChangeTypes.Created:
                    FileTreeNode n = new FileTreeNode();
                    n.File = new FileSystemObject(new DirectoryInfo(e.FullPath));
                    node.Nodes.Add(n);
                    ReloadDir(n);
                    n.Expand();
                    if (n.FullPath == editItem) {
                        editItem = null;
                        n.EnsureVisible();
                        n.BeginEdit();
                    }
                    break;

                case WatcherChangeTypes.Deleted:
                    node.Nodes.RemoveByKey(path[path.Length - 1]); // Name
                    break;
            }

            NeedCompilation = true;
        }

        private void directoryWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            string[] path = FileList.SplitPath(e.OldName);

            FileTreeNode node = rootNode;
            for (int i = 0; i < path.Length; i++) {
                Debug.Assert(node.File.Type == FileSystemObjectType.Directory);

                FileTreeNode childNode = (FileTreeNode)node.Nodes[path[i]];

                if (childNode == null) {
                    ReloadDir(node);
                    return;
                }

                node = childNode;
            }

            node.File = new FileSystemObject(new DirectoryInfo(e.FullPath));
            NeedCompilation = true;
        }

        private void treeView_NodeToggled(object sender, TreeViewEventArgs e)
        {
            Debug.Assert(((FileTreeNode)e.Node).File.Type == FileSystemObjectType.Directory);

            e.Node.ImageKey = e.Action == TreeViewAction.Collapse ? FolderClosedImageKey : FolderOpenedImageKey;
            e.Node.SelectedImageKey = e.Node.ImageKey;
        }

        private void dummyContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            if (treeView.SelectedNode != null) {
                FileTreeNode node = (FileTreeNode)treeView.SelectedNode;
                PopupMenu menu;

                if (node.File.Type == FileSystemObjectType.File) {
                    menu = fileMenu;
                }
                else {
                    menu = dirMenu;
                }

                bool isRootSelected = treeView.SelectedNode == rootNode;

                menu.MenuCommands["Paste"].Enabled = Clipboard.ContainsFileDropList() || Clipboard.ContainsData(typeof(CutFileList).ToString());
                menu.MenuCommands["Cut"].Enabled = !isRootSelected;
                menu.MenuCommands["Rename"].Enabled = !isRootSelected;
                menu.MenuCommands["Delete"].Enabled = !isRootSelected;
                menu.MenuCommands["Show All Files"].Checked = ShowAllFiles;
                menu.TrackPopup(Control.MousePosition);
            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                treeView.SelectedNode = e.Node;
            }
        }

        void edit_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                Debug.Assert(((FileTreeNode)treeView.SelectedNode).File.Type == FileSystemObjectType.File);

                OpenEditor((FileInfo)((FileTreeNode)treeView.SelectedNode).File.Info);
            }
        }

        void open_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                Debug.Assert(((FileTreeNode)treeView.SelectedNode).File.Type == FileSystemObjectType.Directory);

                Thread t = new Thread(new ParameterizedThreadStart(FileList.OpenWorker));
                t.Start(((FileTreeNode)treeView.SelectedNode).File.Info.FullName);
            }
        }

        void expandAll_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                treeView.SelectedNode.ExpandAll();
            }
        }

        void collapseAll_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                treeView.SelectedNode.Collapse(false);
            }
        }

        void cut_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                Clipboard.Clear();

                CutFileList cutList = new CutFileList();
                cutList.Files.Add(((FileTreeNode)treeView.SelectedNode).File.Info.FullName);

                DataObject o = new DataObject();
                o.SetFileDropList(cutList.Files);
                o.SetData(typeof(CutFileList).ToString(), cutList);

                Clipboard.SetDataObject(o);
            }
        }

        void copy_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                StringCollection files = new StringCollection();
                files.Add(((FileTreeNode)treeView.SelectedNode).File.Info.FullName);
                Clipboard.Clear();
                Clipboard.SetFileDropList(files);
            }
        }

        void paste_Click(object sender, EventArgs e)
        {
            FileTreeNode node = (FileTreeNode)treeView.SelectedNode;
            if (node != null && node.File.Type == FileSystemObjectType.Directory) {
                if (Clipboard.ContainsData(typeof(CutFileList).ToString())) {
                    CutFileList data = (CutFileList)Clipboard.GetData(typeof(CutFileList).ToString());
                    Paste(node, data.Files, false);
                }
                else if (Clipboard.ContainsFileDropList()) {
                    StringCollection files = Clipboard.GetFileDropList();
                    Paste(node, files, true);
                }
            }
        }

        void delete_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                ((FileTreeNode)treeView.SelectedNode).File.Delete();
            }
        }

        void rename_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                treeView.SelectedNode.BeginEdit();
            }
        }

        void reload_Click(object sender, EventArgs e)
        {
            ReloadScripts();
        }

        void showAllFiles_Click(object sender, EventArgs e)
        {
            ShowAllFiles = !ShowAllFiles;
        }

        void addFile_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                Debug.Assert(((FileTreeNode)treeView.SelectedNode).File.Type == FileSystemObjectType.Directory);

                try {
                    AddFileDialog dlg = new AddFileDialog();

                    if (dlg.ShowDialog() == DialogResult.OK) {
                        if (dlg.SelectedTemplate != null) {
                            string name;
                            string fileName;

                            if (dlg.ItemName.EndsWith(dlg.SelectedTemplate.Extension)) {
                                name = dlg.ItemName.Remove(dlg.ItemName.Length - dlg.SelectedTemplate.Extension.Length);
                                fileName = dlg.ItemName;
                            }
                            else {
                                name = dlg.ItemName;
                                fileName = dlg.ItemName + dlg.SelectedTemplate.Extension;
                            }

                            string path = Path.Combine(((FileTreeNode)treeView.SelectedNode).File.Info.FullName, fileName);
                            dlg.SelectedTemplate.Create(path, name);
                        }
                    }
                }
                catch (Exception ex) {
                    Trace.WriteLine("Error creating item from selected template. Exception:\n" + ex.ToString(), "Phoenix");
                    MessageBox.Show("Error creating item from selected template.\nMessage: " + ex.Message, "Error");
                }
            }
        }

        void addExisting_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                Debug.Assert(((FileTreeNode)treeView.SelectedNode).File.Type == FileSystemObjectType.Directory);

                openFileDialog.FileName = "";
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    openFileDialog.InitialDirectory = "";

                    DirectoryInfo dir = (DirectoryInfo)((FileTreeNode)treeView.SelectedNode).File.Info;

                    foreach (string file in openFileDialog.FileNames) {
                        FileInfo source = new FileInfo(file);
                        FileInfo dest = new FileInfo(Path.Combine(dir.FullName, source.Name));

                        if (dest.Exists) {
                            DialogResult result = MessageBox.Show("File " + source.Name + " already exists. Overwrite?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                            switch (result) {
                                case DialogResult.Yes:
                                    dest.Delete();
                                    break;

                                case DialogResult.No:
                                    continue;

                                case DialogResult.Cancel:
                                    return;
                            }
                        }

                        source.CopyTo(dest.FullName, true);
                    }
                }
            }
        }

        void addFolder_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null) {
                Debug.Assert(((FileTreeNode)treeView.SelectedNode).File.Type == FileSystemObjectType.Directory);

                editItem = Path.Combine(treeView.SelectedNode.FullPath, "New Folder");
                ((DirectoryInfo)((FileTreeNode)treeView.SelectedNode).File.Info).CreateSubdirectory("New Folder");
            }
        }

        private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node == rootNode)
                e.CancelEdit = true;
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            e.CancelEdit = true;

            if (e.Label != null) {
                Debug.Assert(e.Node != rootNode);

                if (!e.Node.Parent.Nodes.ContainsKey(e.Label)) {
                    FileTreeNode node = (FileTreeNode)e.Node;

                    if (node.File.Type == FileSystemObjectType.File) {
                        if (String.Compare(Path.GetExtension(e.Label), node.File.Info.Extension, StringComparison.InvariantCultureIgnoreCase) != 0) {
                            DialogResult result = MessageBox.Show("You want to change extension. File could loose its meaning. Do you really want to do that?", "File Extension Change", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (result != DialogResult.OK) {
                                return;
                            }
                        }
                    }

                    node.File.Rename(e.Label);
                }
                else {
                    MessageBox.Show("File or directory of specified name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            DataObject o = new DataObject(e.Data);
            if (o.ContainsFileDropList()) {
                FileTreeNode node = (FileTreeNode)treeView.GetNodeAt(treeView.PointToClient(new Point(e.X, e.Y)));
                if (node != null && node.File.Type == FileSystemObjectType.Directory) {
                    StringCollection files = o.GetFileDropList();

                    Paste(node, files, e.Effect == DragDropEffects.Move ? false : true);
                }
            }
        }

        private void Paste(FileTreeNode node, StringCollection files, bool copy)
        {
            DirectoryInfo destDir = (DirectoryInfo)node.File.Info;

            foreach (string path in files) {
                FileSystemObject dest;
                FileSystemObject src;

                if (File.Exists(path)) {
                    src = (FileSystemObject)new FileInfo(path);
                    dest = (FileSystemObject)new FileInfo(Path.Combine(destDir.FullName, src.Info.Name));
                }
                else if (Directory.Exists(path)) {
                    src = (FileSystemObject)new DirectoryInfo(path);
                    dest = (FileSystemObject)new DirectoryInfo(Path.Combine(destDir.FullName, src.Info.Name));
                }
                else
                    continue;

                if (String.Compare(src.Info.FullName, dest.Info.FullName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    continue;

                if (dest.Info.Exists) {
                    DialogResult result = MessageBox.Show("File or directory " + dest.Info.Name + " already exists. Overwrite?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    switch (result) {
                        case DialogResult.Yes:
                            dest.Delete();
                            break;

                        case DialogResult.No:
                            continue;

                        case DialogResult.Cancel:
                            return;
                    }
                }

                if (copy) {
                    src.CopyTo(dest.Info.FullName);
                }
                else {
                    src.MoveTo(dest.Info.FullName);
                }
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            DataObject o = new DataObject(e.Data);
            if (o.ContainsFileDropList()) {
                FileTreeNode node = (FileTreeNode)treeView.GetNodeAt(treeView.PointToClient(new Point(e.X, e.Y)));
                if (node != null && node.File.Type == FileSystemObjectType.Directory) {
                    if ((e.AllowedEffect & DragDropEffects.Move) != 0 && (e.KeyState & 8) == 0) // Ctrl is pressed
                    {
                        e.Effect = DragDropEffects.Move;
                        return;
                    }
                    else if ((e.AllowedEffect & DragDropEffects.Copy) != 0) {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void treeView_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed) {
                e.Action = DragAction.Cancel;
            }
            else {
                e.Action = DragAction.Continue;
            }
        }

        private FileTreeNode draggedNode = null;
        private Rectangle dragRect = Rectangle.Empty;

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            draggedNode = (FileTreeNode)treeView.GetNodeAt(e.Location);

            if (draggedNode == rootNode)
                draggedNode = null;

            if (draggedNode != null) {
                Size dragSize = SystemInformation.DragSize;
                dragRect = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
        }

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedNode != null && (e.Button & MouseButtons.Left) != 0) {
                if (!dragRect.Contains(e.Location)) {
                    StringCollection files = new StringCollection();
                    files.Add(draggedNode.File.Info.FullName);
                    DataObject o = new DataObject();
                    o.SetFileDropList(files);

                    draggedNode = null;
                    dragRect = Rectangle.Empty;

                    DoDragDrop(o, DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }

        private void treeView_MouseUp(object sender, MouseEventArgs e)
        {
            draggedNode = null;
            dragRect = Rectangle.Empty;
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (((FileTreeNode)e.Node).File.Type == FileSystemObjectType.File) {
                OpenEditor((FileInfo)((FileTreeNode)e.Node).File.Info);
            }
        }

        private void OpenEditor(FileInfo file)
        {
            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(Core.Directory, "SharpEditor.exe"));
            psi.Arguments = "\"" + file.FullName + "\"";
            psi.WorkingDirectory = Phoenix.Runtime.RuntimeCore.ScriptsDirectory;

            Process.Start(psi);
        }

        #region Static functions

        private static string[] SplitPath(string name)
        {
            return name.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void OpenWorker(object parameter)
        {
            try {
                Debug.WriteLine("Opening folder..", "Gui");

                ProcessStartInfo info = new ProcessStartInfo();
                info.UseShellExecute = true;
                info.Verb = "open";
                info.FileName = parameter.ToString();
                Process.Start(info);

                Debug.WriteLine("Folder opened.", "Gui");
            }
            catch (Exception e) {
                Trace.WriteLine("Unable to open explorer. Exception:\n" + e.ToString(), "Gui");
                MessageBox.Show("Unable to open explorer.", "Error");
            }
        }

        #endregion
    }
}
