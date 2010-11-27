namespace SharpEditor

import System
import System.IO
import System.Windows.Forms
import System.Reflection
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document

class FileEditor:
"""Represents opened file in the editor"""

    [Getter(Control)]
    final _control as TextEditorControl
    
    [Getter(File)]
    _file as FileInfo
    
    [Getter(IsModified)]
    _modified as bool
    
    event ModifiedChanged as EventHandler
    
    static def constructor():
        file = FileInfo(Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath)
        provider = FileSyntaxModeProvider(file.Directory.FullName)
        //SharpEditor.ResourceSyntaxModeProvider(typeof(FileEditor).Assembly, 'SharpEditor.Resources')
        HighlightingManager.Manager.AddSyntaxModeFileProvider(provider)

    public def constructor(file as FileInfo):
        _file = file
        
        _control = TextEditorControl()
        _control.Dock = System.Windows.Forms.DockStyle.Fill
        _control.TextEditorProperties.CutCopyWholeLine = true
        _control.TextEditorProperties.ConvertTabsToSpaces = true
        
        _control.Document.DocumentChanged += DocumentChanged
        
    public TextArea:
        get:
            return _control.ActiveTextAreaControl.TextArea

    public def Reload():
        raise InvalidOperationException("File path not specified.") if _file is null
        
        // Load
        _control.Text = String.Empty
        _control.LoadFile(_file.FullName) if _file.Exists
        
        // Set modified flag
        _modified = false
        OnModifiedChanged()

    public def Save():
        raise InvalidOperationException("File path not specified.") if _file is null
        
        SaveAs(_file)
        
    public def SaveAs(file as FileInfo):
        assert file is not null 
        
        // Save
        _control.SaveFile(file.FullName)
        
        if file != _file:
            _file = file
            try:
                _control.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(file.FullName);
            except ex as HighlightingDefinitionInvalidException:
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        
        // Set modified flag
        _modified = false
        OnModifiedChanged()
        
    private def DocumentChanged(sender as object, e as DocumentEventArgs):
        if not _modified:
            _modified = true
            OnModifiedChanged()
        
    protected virtual def OnModifiedChanged():
        h = ModifiedChanged
        h(self, EventArgs.Empty) if h is not null
    
