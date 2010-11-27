namespace SharpEditor

partial class MainForm(System.Windows.Forms.Form):
    private components as System.ComponentModel.IContainer = null
    
    protected override def Dispose(disposing as bool) as void:
        if disposing:
            if components is not null:
                components.Dispose()
        super(disposing)
    
    // This method is required for Windows Forms designer support.
    // Do not change the method contents inside the source code editor. The Forms designer might
    // not be able to load this method if it was changed manually.
    private def InitializeComponent():
    	self.components = System.ComponentModel.Container()
    	resources as System.ComponentModel.ComponentResourceManager = System.ComponentModel.ComponentResourceManager(typeof(MainForm))
    	self.topMenu = System.Windows.Forms.MenuStrip()
    	self.fileToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.newToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.openToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.saveToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.saveasToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.closeToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.toolStripMenuItem1 = System.Windows.Forms.ToolStripSeparator()
    	self.exitToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.editToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.undoToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.toolStripMenuItem2 = System.Windows.Forms.ToolStripSeparator()
    	self.cutToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.copyToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.pasteToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.deleteToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.toolStripMenuItem3 = System.Windows.Forms.ToolStripSeparator()
    	self.selectAllToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.searchMenuItem = System.Windows.Forms.ToolStripTextBox()
    	self.searchMenuButton = System.Windows.Forms.ToolStripMenuItem()
    	self.replaceMenuButton = System.Windows.Forms.ToolStripMenuItem()
    	self.tabControl = System.Windows.Forms.TabControl()
    	self.tabMenu = System.Windows.Forms.ContextMenuStrip(self.components)
    	self.closeContextMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.openFileDialog = System.Windows.Forms.OpenFileDialog()
    	self.saveFileDialog = System.Windows.Forms.SaveFileDialog()
    	self.findToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()
    	self.toolStripSeparator1 = System.Windows.Forms.ToolStripSeparator()
    	self.topMenu.SuspendLayout()
    	self.tabMenu.SuspendLayout()
    	self.SuspendLayout()
    	# 
    	# topMenu
    	# 
    	self.topMenu.Items.AddRange((of System.Windows.Forms.ToolStripItem: self.fileToolStripMenuItem, self.editToolStripMenuItem, self.searchMenuItem, self.searchMenuButton, self.replaceMenuButton))
    	resources.ApplyResources(self.topMenu, "topMenu")
    	self.topMenu.Name = "topMenu"
    	# 
    	# fileToolStripMenuItem
    	# 
    	self.fileToolStripMenuItem.DropDownItems.AddRange((of System.Windows.Forms.ToolStripItem: self.newToolStripMenuItem, self.openToolStripMenuItem, self.saveToolStripMenuItem, self.saveasToolStripMenuItem, self.closeToolStripMenuItem, self.toolStripMenuItem1, self.exitToolStripMenuItem))
    	self.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
    	resources.ApplyResources(self.fileToolStripMenuItem, "fileToolStripMenuItem")
    	# 
    	# newToolStripMenuItem
    	# 
    	self.newToolStripMenuItem.Name = "newToolStripMenuItem"
    	resources.ApplyResources(self.newToolStripMenuItem, "newToolStripMenuItem")
    	self.newToolStripMenuItem.Click += self.NewToolStripMenuItemClick as System.EventHandler
    	# 
    	# openToolStripMenuItem
    	# 
    	self.openToolStripMenuItem.Name = "openToolStripMenuItem"
    	resources.ApplyResources(self.openToolStripMenuItem, "openToolStripMenuItem")
    	self.openToolStripMenuItem.Click += self.OpenToolStripMenuItemClick as System.EventHandler
    	# 
    	# saveToolStripMenuItem
    	# 
    	self.saveToolStripMenuItem.Name = "saveToolStripMenuItem"
    	resources.ApplyResources(self.saveToolStripMenuItem, "saveToolStripMenuItem")
    	self.saveToolStripMenuItem.Click += self.SaveToolStripMenuItemClick as System.EventHandler
    	# 
    	# saveasToolStripMenuItem
    	# 
    	self.saveasToolStripMenuItem.Name = "saveasToolStripMenuItem"
    	resources.ApplyResources(self.saveasToolStripMenuItem, "saveasToolStripMenuItem")
    	self.saveasToolStripMenuItem.Click += self.SaveasToolStripMenuItemClick as System.EventHandler
    	# 
    	# closeToolStripMenuItem
    	# 
    	self.closeToolStripMenuItem.Name = "closeToolStripMenuItem"
    	resources.ApplyResources(self.closeToolStripMenuItem, "closeToolStripMenuItem")
    	self.closeToolStripMenuItem.Click += self.CloseToolStripMenuItemClick as System.EventHandler
    	# 
    	# toolStripMenuItem1
    	# 
    	self.toolStripMenuItem1.Name = "toolStripMenuItem1"
    	resources.ApplyResources(self.toolStripMenuItem1, "toolStripMenuItem1")
    	# 
    	# exitToolStripMenuItem
    	# 
    	self.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
    	resources.ApplyResources(self.exitToolStripMenuItem, "exitToolStripMenuItem")
    	self.exitToolStripMenuItem.Click += self.ExitToolStripMenuItemClick as System.EventHandler
    	# 
    	# editToolStripMenuItem
    	# 
    	self.editToolStripMenuItem.DropDownItems.AddRange((of System.Windows.Forms.ToolStripItem: self.undoToolStripMenuItem, self.toolStripMenuItem2, self.cutToolStripMenuItem, self.copyToolStripMenuItem, self.pasteToolStripMenuItem, self.deleteToolStripMenuItem, self.toolStripMenuItem3, self.selectAllToolStripMenuItem, self.toolStripSeparator1, self.findToolStripMenuItem))
    	self.editToolStripMenuItem.Name = "editToolStripMenuItem"
    	resources.ApplyResources(self.editToolStripMenuItem, "editToolStripMenuItem")
    	# 
    	# undoToolStripMenuItem
    	# 
    	self.undoToolStripMenuItem.Name = "undoToolStripMenuItem"
    	resources.ApplyResources(self.undoToolStripMenuItem, "undoToolStripMenuItem")
    	self.undoToolStripMenuItem.Click += self.UndoToolStripMenuItemClick as System.EventHandler
    	# 
    	# toolStripMenuItem2
    	# 
    	self.toolStripMenuItem2.Name = "toolStripMenuItem2"
    	resources.ApplyResources(self.toolStripMenuItem2, "toolStripMenuItem2")
    	# 
    	# cutToolStripMenuItem
    	# 
    	self.cutToolStripMenuItem.Name = "cutToolStripMenuItem"
    	resources.ApplyResources(self.cutToolStripMenuItem, "cutToolStripMenuItem")
    	self.cutToolStripMenuItem.Click += self.CutToolStripMenuItemClick as System.EventHandler
    	# 
    	# copyToolStripMenuItem
    	# 
    	self.copyToolStripMenuItem.Name = "copyToolStripMenuItem"
    	resources.ApplyResources(self.copyToolStripMenuItem, "copyToolStripMenuItem")
    	self.copyToolStripMenuItem.Click += self.CopyToolStripMenuItemClick as System.EventHandler
    	# 
    	# pasteToolStripMenuItem
    	# 
    	self.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem"
    	resources.ApplyResources(self.pasteToolStripMenuItem, "pasteToolStripMenuItem")
    	self.pasteToolStripMenuItem.Click += self.PasteToolStripMenuItemClick as System.EventHandler
    	# 
    	# deleteToolStripMenuItem
    	# 
    	self.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem"
    	resources.ApplyResources(self.deleteToolStripMenuItem, "deleteToolStripMenuItem")
    	self.deleteToolStripMenuItem.Click += self.DeleteToolStripMenuItemClick as System.EventHandler
    	# 
    	# toolStripMenuItem3
    	# 
    	self.toolStripMenuItem3.Name = "toolStripMenuItem3"
    	resources.ApplyResources(self.toolStripMenuItem3, "toolStripMenuItem3")
    	# 
    	# selectAllToolStripMenuItem
    	# 
    	self.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem"
    	resources.ApplyResources(self.selectAllToolStripMenuItem, "selectAllToolStripMenuItem")
    	self.selectAllToolStripMenuItem.Click += self.SelectAllToolStripMenuItemClick as System.EventHandler
    	# 
    	# searchMenuItem
    	# 
    	resources.ApplyResources(self.searchMenuItem, "searchMenuItem")
    	self.searchMenuItem.Margin = System.Windows.Forms.Padding(20, 0, 1, 0)
    	self.searchMenuItem.Name = "searchMenuItem"
    	self.searchMenuItem.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
    	# 
    	# searchMenuButton
    	# 
    	self.searchMenuButton.Name = "searchMenuButton"
    	resources.ApplyResources(self.searchMenuButton, "searchMenuButton")
    	self.searchMenuButton.Click += self.SearchMenuButtonClick as System.EventHandler
    	# 
    	# replaceMenuButton
    	# 
    	self.replaceMenuButton.Name = "replaceMenuButton"
    	resources.ApplyResources(self.replaceMenuButton, "replaceMenuButton")
    	self.replaceMenuButton.Click += self.ReplaceMenuButtonClick as System.EventHandler
    	# 
    	# tabControl
    	# 
    	self.tabControl.ContextMenuStrip = self.tabMenu
    	resources.ApplyResources(self.tabControl, "tabControl")
    	self.tabControl.Multiline = true
    	self.tabControl.Name = "tabControl"
    	self.tabControl.SelectedIndex = 0
    	self.tabControl.MouseDown += self.TabControlMouseDown as System.Windows.Forms.MouseEventHandler
    	self.tabControl.SelectedIndexChanged += self.TabControlSelectedIndexChanged as System.EventHandler
    	# 
    	# tabMenu
    	# 
    	self.tabMenu.Items.AddRange((of System.Windows.Forms.ToolStripItem: self.closeContextMenuItem))
    	self.tabMenu.Name = "tabMenu"
    	resources.ApplyResources(self.tabMenu, "tabMenu")
    	# 
    	# closeContextMenuItem
    	# 
    	self.closeContextMenuItem.Name = "closeContextMenuItem"
    	resources.ApplyResources(self.closeContextMenuItem, "closeContextMenuItem")
    	self.closeContextMenuItem.Click += self.CloseToolStripMenuItem1Click as System.EventHandler
    	# 
    	# openFileDialog
    	# 
    	self.openFileDialog.Multiselect = true
    	resources.ApplyResources(self.openFileDialog, "openFileDialog")
    	# 
    	# saveFileDialog
    	# 
    	resources.ApplyResources(self.saveFileDialog, "saveFileDialog")
    	# 
    	# findToolStripMenuItem
    	# 
    	self.findToolStripMenuItem.Name = "findToolStripMenuItem"
    	resources.ApplyResources(self.findToolStripMenuItem, "findToolStripMenuItem")
    	self.findToolStripMenuItem.Click += self.FindToolStripMenuItemClick as System.EventHandler
    	# 
    	# toolStripSeparator1
    	# 
    	self.toolStripSeparator1.Name = "toolStripSeparator1"
    	resources.ApplyResources(self.toolStripSeparator1, "toolStripSeparator1")
    	# 
    	# MainForm
    	# 
    	resources.ApplyResources(self, "$this")
    	self.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    	self.Controls.Add(self.tabControl)
    	self.Controls.Add(self.topMenu)
    	self.MainMenuStrip = self.topMenu
    	self.Name = "MainForm"
    	self.Shown += self.MainFormShown as System.EventHandler
    	self.FormClosing += self.MainFormFormClosing as System.Windows.Forms.FormClosingEventHandler
    	self.topMenu.ResumeLayout(false)
    	self.topMenu.PerformLayout()
    	self.tabMenu.ResumeLayout(false)
    	self.ResumeLayout(false)
    	self.PerformLayout()
    private findToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private toolStripSeparator1 as System.Windows.Forms.ToolStripSeparator
    private replaceMenuButton as System.Windows.Forms.ToolStripMenuItem
    private searchMenuButton as System.Windows.Forms.ToolStripMenuItem
    private searchMenuItem as System.Windows.Forms.ToolStripTextBox
    private closeContextMenuItem as System.Windows.Forms.ToolStripMenuItem
    private saveFileDialog as System.Windows.Forms.SaveFileDialog
    private openFileDialog as System.Windows.Forms.OpenFileDialog
    private selectAllToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private toolStripMenuItem3 as System.Windows.Forms.ToolStripSeparator
    private tabMenu as System.Windows.Forms.ContextMenuStrip
    private tabControl as System.Windows.Forms.TabControl
    private deleteToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private toolStripMenuItem2 as System.Windows.Forms.ToolStripSeparator
    private undoToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private pasteToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private copyToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private cutToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private exitToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private toolStripMenuItem1 as System.Windows.Forms.ToolStripSeparator
    private closeToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private editToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private saveasToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private saveToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private openToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private newToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private fileToolStripMenuItem as System.Windows.Forms.ToolStripMenuItem
    private topMenu as System.Windows.Forms.MenuStrip
   
