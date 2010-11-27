namespace SharpEditor

import System
import System.IO
import System.Collections
import System.Drawing
import System.Windows.Forms
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document

partial class MainForm:

    _newIndex = 0
    
    public def constructor():        
        // The InitializeComponent() call is required for Windows Forms designer support.
        InitializeComponent()
    
    #region Properties
    
    public ActiveEditor as FileEditor:
        get:
            assert tabControl.SelectedIndex > -1
            return tabControl.SelectedTab.Tag
            
    public HasActiveEditor as bool:
        get:
            return tabControl.SelectedIndex > -1
    
    [Property(FileModifiedText)]
    fileModifiedText = "File \"{0}\" has been modified. Save?"

    #endregion
    
    #region Editor methods
    
    public def BringToFront():
        // Ugly but functional
        old = TopMost
        TopMost = true
        TopMost = old

    public def OpenFile(file as FileInfo):
        assert file is not null
        
        // Find, whether we doesn't have file already opened
        for tab as TabPage in tabControl.TabPages:
            editor as FileEditor = tab.Tag
            assert editor is not null
            
            if editor.File is not null and editor.File.FullName == file.FullName:
                // Select tab and exit
                tabControl.SelectTab(tab)
                return
        
        // Open new tab
        AddEditorTab(file.Name, file)
        
    protected def AddEditorTab(name as string, file as FileInfo) as FileEditor:
        assert name is not null 
        
        // Prepare tab
        tab = TabPage()
        tab.Text = name
        
        // Create editor
        editor = FileEditor(file)
        
        editor.ModifiedChanged += def:
            tab.Text = name
            tab.Text += "*" if editor.IsModified
        
        tab.Controls.Add(editor.Control)
        tab.Tag = editor
        tab.ToolTipText = file.FullName if file is not null        
        
        // Add to list and tab control
        tabControl.TabPages.Add(tab)
        
        // Load content
        editor.Reload() if file is not null
        
        // Select
        tabControl.Refresh()
        tabControl.SelectTab(tab)
        UpdateEnabled()
        
    protected def CloseEditorTab(tab as TabPage):
        assert tab is not null
                
        // Save first
        SaveFile(tab, false)
        
        // Close
        tabControl.TabPages.Remove(tab)
        tabControl.Refresh()
        
    protected def PrepareClose(tab as TabPage) as bool:
        assert tab is not null
        
        editor as FileEditor = tabControl.SelectedTab.Tag
        assert editor is not null
        
        if editor.IsModified:
            name = (editor.File.FullName if editor.File is not null else tab.Text.Replace("*", ""))
            result = MessageBox.Show(String.Format(FileModifiedText, name), Text, MessageBoxButtons.YesNoCancel)
            
            if result == DialogResult.Cancel:
                // If user selected cancel, return false
                return false
            elif result == DialogResult.Yes:
                // Save file
                SaveFile(tab, false)
        
        // Either files was saved or not modified
        return true
        
    protected def SaveFile(tab as TabPage, asNew as bool):
        assert tab is not null
        
        editor as FileEditor = tabControl.SelectedTab.Tag
        assert editor is not null
        
        // For new file, show dialog
        if asNew or editor.File is null:
            // Show dialog
            if editor.File is not null:
                saveFileDialog.FileName = editor.File.FullName
                saveFileDialog.DefaultExt = editor.File.Extension
            else:
                saveFileDialog.FileName = String.Empty
                saveFileDialog.DefaultExt = String.Empty
            
            result = saveFileDialog.ShowDialog()
            return if result != DialogResult.OK
            
            // Save file
            editor.SaveAs(FileInfo(saveFileDialog.FileName))
            
            // Update tab title
            tab.Text = editor.File.Name
            tab.ToolTipText = editor.File.FullName
            tabControl.Refresh()
        else:
            // Ignore if not modified
            return if not editor.IsModified
            
            // Simply save
            editor.Save()
        
    #endregion
    
    #region Menu event handlers
    
    private def ExitToolStripMenuItemClick(sender as object, e as System.EventArgs):
        Close()
    
    private def NewToolStripMenuItemClick(sender as object, e as System.EventArgs):
        AddEditorTab("New File " + (++_newIndex), null)
        
    private def OpenToolStripMenuItemClick(sender as object, e as System.EventArgs):
        openFileDialog.Multiselect = true
        
        result = openFileDialog.ShowDialog()
        return if result != DialogResult.OK
        
        for filename in openFileDialog.FileNames:
            file = FileInfo(filename)
            AddEditorTab(file.Name, file)
        
    private def SaveToolStripMenuItemClick(sender as object, e as System.EventArgs):
        SaveFile(tabControl.SelectedTab, false)
        
    private def SaveasToolStripMenuItemClick(sender as object, e as System.EventArgs):
        SaveFile(tabControl.SelectedTab, true)
    
    private def CloseToolStripMenuItemClick(sender as object, e as System.EventArgs):
        tab = tabControl.SelectedTab
        
        if PrepareClose(tab):        
            CloseEditorTab(tabControl.SelectedTab)
        
    private def CutToolStripMenuItemClick(sender as object, e as System.EventArgs):
        return if not ActiveEditor.TextArea.Focused
        ActiveEditor.TextArea.ClipboardHandler.Cut(sender, e)
    
    private def CopyToolStripMenuItemClick(sender as object, e as System.EventArgs):
        return if not ActiveEditor.TextArea.Focused
        ActiveEditor.TextArea.ClipboardHandler.Copy(sender, e)
    
    private def PasteToolStripMenuItemClick(sender as object, e as System.EventArgs):
        return if not ActiveEditor.TextArea.Focused
        ActiveEditor.TextArea.ClipboardHandler.Paste(sender, e)
    
    private def DeleteToolStripMenuItemClick(sender as object, e as System.EventArgs):
        return if not ActiveEditor.TextArea.Focused
        ActiveEditor.TextArea.ClipboardHandler.Delete(sender, e)

    private def SelectAllToolStripMenuItemClick(sender as object, e as System.EventArgs):
        return if not ActiveEditor.TextArea.Focused
        ActiveEditor.TextArea.ClipboardHandler.SelectAll(sender, e)
        
    private def UndoToolStripMenuItemClick(sender as object, e as System.EventArgs):
        return if not ActiveEditor.TextArea.Focused
        ActiveEditor.Control.Undo()

    private def CloseToolStripMenuItem1Click(sender as object, e as System.EventArgs):
        CloseEditorTab(tabControl.SelectedTab)
        
    private def FindToolStripMenuItemClick(sender as object, e as System.EventArgs):
    	searchMenuItem.TextBox.Focus()
    
    #endregion
    
    #region Other event handlers

    private def MainFormFormClosing(sender as object, e as System.Windows.Forms.FormClosingEventArgs):
        return if e.Cancel // Dont process if action was already cancelled
        
        // Iterate over all opened files
        for tab as TabPage in tabControl.TabPages:
            if not PrepareClose(tab):
                // User didnt want to exit
                e.Cancel = true
                return
    
    private def TabControlMouseDown(sender as object, e as System.Windows.Forms.MouseEventArgs):
        return if not e.Button == MouseButtons.Right

        for i in range(tabControl.TabCount):
            if tabControl.GetTabRect(i).Contains(e.Location):
                tabControl.SelectTab(i)
                return
                
    private def TabControlSelectedIndexChanged(sender as object, e as System.EventArgs):
        UpdateEnabled()
        
    private def MainFormShown(sender as object, e as System.EventArgs):
    	UpdateEnabled()
    	
    	searchMenuItem.TextBox.KeyUp += def(s, e as KeyEventArgs):
            SearchMenuButtonClick(s, e) if e.KeyValue == Keys.Enter
            
    private def UpdateEnabled():
        enabled = HasActiveEditor
        
        // File
        saveToolStripMenuItem.Enabled = enabled
        saveasToolStripMenuItem.Enabled = enabled
        closeToolStripMenuItem.Enabled = enabled
        
        // Edit
        undoToolStripMenuItem.Enabled = enabled
        cutToolStripMenuItem.Enabled = enabled
        copyToolStripMenuItem.Enabled = enabled
        pasteToolStripMenuItem.Enabled = enabled
        deleteToolStripMenuItem.Enabled = enabled
        selectAllToolStripMenuItem.Enabled = enabled
        findToolStripMenuItem.Enabled = enabled
        
        // Search
        searchMenuButton.Enabled = enabled
        replaceMenuButton.Enabled = enabled
        searchMenuItem.Enabled = enabled
        searchMenuItem.Text = String.Empty if not enabled
    
    private def SearchMenuButtonClick(sender as object, e as System.EventArgs):
        editor as FileEditor = tabControl.SelectedTab.Tag
        assert editor is not null
        
        SearchAndReplace(editor.TextArea, searchMenuItem.Text, true) if searchMenuItem.Text.Length > 0

    private def ReplaceMenuButtonClick(sender as object, e as System.EventArgs):
        editor as FileEditor = tabControl.SelectedTab.Tag
        assert editor is not null
        
        // TODO
        
    private def SearchAndReplace(textArea as TextArea, text as string, ignoreCase as bool):
        // First we determine initial search offset
        startOffset = textArea.Caret.Offset
        startOffset += 1 if textArea.SelectionManager.HasSomethingSelected
        startOffset = 0 if startOffset >= textArea.Document.TextLength
        
        if ignoreCase:
            comparsion = StringComparison.CurrentCultureIgnoreCase
        else:
            comparsion = StringComparison.CurrentCulture
        
        // Perform search
        content = textArea.Document.GetText(startOffset, textArea.Document.TextLength - startOffset)
        index = content.IndexOf(text, comparsion)
        
        // Again from start
        if index < 0 and startOffset > 0:
            content = textArea.Document.GetText(0, startOffset)
            startOffset = 0
            
            index = content.IndexOf(text, comparsion)
            
        // Exit if not found
        return if index < 0
        
        // Mark search result
        line = textArea.Document.GetLineSegmentForOffset(startOffset + index)
        column = startOffset + index - line.Offset
        
        textArea.Caret.Line = line.LineNumber
        textArea.Caret.Column = column
        textArea.Caret.UpdateCaretPosition()
        textArea.SelectionManager.SetSelection(TextLocation(column, line.LineNumber), TextLocation(column + text.Length, line.LineNumber))
    
    #endregion
