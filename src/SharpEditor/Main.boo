namespace SharpEditor

import System
import System.Threading
import System.ServiceModel
import System.IO
import System.Collections.Generic
import System.Windows.Forms

[STAThread]
public def Main(argv as (string)) as void:
    // WinForms init
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(false)
    
    // Convert arguments to file names
    files = List of FileInfo()
    
    // This is compatibility, so it can be used using windows explorer
    if (File.Exists(String.Join(" ", argv))):
        files.Add(FileInfo(String.Join(" ", argv)))
    else:
        for arg in argv:
            files.Add(FileInfo(arg))
    
    // Locking mechanism
    globalLock = Mutex(false, "{AFDDE33E-41F4-4996-B5DC-8C7F5A67DB97}")
    
    if globalLock.WaitOne(0, false):
        try:
            RunLocal(files)
        ensure:
            globalLock.ReleaseMutex()
    else:
        RunRemote(files)
        

def RunLocal(files as List of FileInfo):
    // Create window
    window = MainForm()
    
    // Create WCF service
    using host = ServiceHost(SharpEditorHost(window), Uri(SharpEditorHost.Address)):
        // Endpoint
        host.AddServiceEndpoint(ISharpEditor, NetNamedPipeBinding(), SharpEditorHost.Name)
        
        // Start service when window is opened
        window.Load += def:
            // Open files from cmdline
            for f in files:
                window.OpenFile(f)
            
            // Start listening
            host.Open()
        
        // Run the application
        Application.Run(window)

def RunRemote(files as List of FileInfo):
    // Create WCF client
    ep = EndpointAddress("${SharpEditorHost.Address}/${SharpEditorHost.Name}")
    proxy = ChannelFactory[of ISharpEditor].CreateChannel(NetNamedPipeBinding(), ep)
    
    // Activate window
    proxy.BringToFront()
    
    // Open files
    for f in files:
        proxy.OpenFile(f.FullName)
