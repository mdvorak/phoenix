namespace SharpEditor

import System
import System.IO
import System.ServiceModel

[ServiceBehavior(InstanceContextMode: InstanceContextMode.Single)]
class SharpEditorHost(ISharpEditor):

    public static final Name = "Window" 
    public static final Address = "net.pipe://localhost/SharpEditor" 

    _window as MainForm

    public def constructor(window as MainForm):
        raise ArgumentNullException("window") if window is null
        
        _window = window
        
    public def OpenFile(file as string) as void:
        _window.Invoke(_window.OpenFile, FileInfo(file))
        
    public def BringToFront() as void:
        _window.Invoke(_window.BringToFront)

