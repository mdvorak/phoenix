using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.CodeDom.Compiler;
using System.Threading;
using Phoenix.Runtime.Reflection;
using Phoenix.Gui;
using Phoenix.Gui.Editor;
using Phoenix.Collections;
using Phoenix.Macros;

namespace Phoenix.Runtime
{
    public static class RuntimeCore
    {
        private static readonly object objectsSync = new object();
        private static MethodCollection commandList = new MethodCollection();
        private static MethodCollection executableList = new MethodCollection();
        private static Executions executions = new Executions();
        private static Hotkeys hotkeys = new Hotkeys();
        private static MacrosCollection macros = (MacrosCollection)new MacrosCollection().CreateSynchronized();

        private static ListEx<Assembly> scriptAssemblies = new ListEx<Assembly>().CreateSynchronized();
        private static ListEx<Assembly> loadedPlugins = new ListEx<Assembly>().CreateSynchronized();
        private static ListEx<RuntimeObjectsLoaderReport> reports = new ListEx<RuntimeObjectsLoaderReport>().CreateSynchronized();

        private static UnregisteringAssemblyPublicEvent unregisteringAssembly = new UnregisteringAssemblyPublicEvent();

        static RuntimeCore()
        {
            Core.Disconnected += new EventHandler(Core_Disconnected);
        }

        #region AssemblyObject register

        class AssemblyObject
        {
            public IAssemblyObject Object;
            public IAssemblyObjectList List;

            public AssemblyObject(IAssemblyObject obj, IAssemblyObjectList list)
            {
                Object = obj;
                List = list;
            }
        }

        private static List<AssemblyObject> objectList = new List<AssemblyObject>();
        private delegate void RemoveObjectDelegate(IAssemblyObject obj);

        /// <summary>
        /// Remove all registered assembly object from their lists.
        /// </summary>
        /// <param name="assembly">Assembly where these objects are declared.</param>
        public static void UnregisterAssembly(Assembly assembly)
        {
            try {
                if (assembly == null)
                    return;

                lock (objectsSync) {
                    unregisteringAssembly.Invoke(null, new UnregisteringAssemblyEventArgs(assembly));

                    List<AssemblyObject> removeList = new List<AssemblyObject>();

                    foreach (AssemblyObject obj in objectList) {
                        Debug.Assert(obj.Object.Assembly != null);

                        if (obj.Object.Assembly.FullName == assembly.FullName) {
                            removeList.Add(obj);
                        }
                    }

                    foreach (AssemblyObject obj in removeList) {
                        System.ComponentModel.ISynchronizeInvoke sync = obj.List as System.ComponentModel.ISynchronizeInvoke;

                        if (sync != null && sync.InvokeRequired)
                            sync.BeginInvoke(new RemoveObjectDelegate(obj.List.Remove), new object[] { obj.Object });
                        else
                            obj.List.Remove(obj.Object);

                        objectList.Remove(obj);
                    }

                    GC.Collect();
                }
            }
            catch (Exception e) {
                string message = String.Format("Error unregistering assembly {0}. Some objects may be still registered and could cause conficts.", assembly.FullName);
                Trace.WriteLine(message + " Exception:\n" + e.ToString(), "Runtime");
                System.Windows.Forms.MessageBox.Show(message, "Waring");
            }
        }

        /// <summary>
        /// Used only by Add methods of calling IAssemblyObjectList.
        /// </summary>
        internal static void AddAssemblyObject(IAssemblyObject obj, IAssemblyObjectList list)
        {
            lock (objectsSync) {
                if (obj == null) return;
                if (list == null) throw new ArgumentNullException("list");

                objectList.Add(new AssemblyObject(obj, list));
            }
        }

        /// <summary>
        /// Used only by Remove methods of calling IAssemblyObjectList. IAssemblyObjectList.Remove is not called.
        /// </summary>
        internal static void RemoveAssemblyObject(IAssemblyObject obj)
        {
            lock (objectsSync) {
                if (obj == null) return;

                foreach (AssemblyObject o in objectList) {
                    if (o.Object == obj) {
                        objectList.Remove(o);
                        return;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Fired before assembly is unregistered.
        /// </summary>
        public static event UnregisteringAssemblyEventHandler UnregisteringAssembly
        {
            add { unregisteringAssembly.AddHandler(value); }
            remove { unregisteringAssembly.RemoveHandler(value); }
        }

        /// <summary>
        /// Gets path to scripts directory
        /// </summary>
        public static string ScriptsDirectory
        {
            get { return Path.Combine(Core.Directory, "Scripts"); }
        }

        /// <summary>
        /// Gets path to plugins directory
        /// </summary>
        public static string PluginsDirectory
        {
            get { return Path.Combine(Core.Directory, "Plugins"); }
        }

        public static MethodCollection CommandList
        {
            get { return RuntimeCore.commandList; }
        }

        public static MethodCollection ExecutableList
        {
            get { return RuntimeCore.executableList; }
        }

        public static Executions Executions
        {
            get { return RuntimeCore.executions; }
        }

        public static Hotkeys Hotkeys
        {
            get { return RuntimeCore.hotkeys; }
        }

        public static MacrosCollection Macros
        {
            get { return RuntimeCore.macros; }
        }

        public static ListEx<RuntimeObjectsLoaderReport> Reports
        {
            get { return RuntimeCore.reports; }
        }

        private static string GetCurrentTimeString()
        {
            DateTime now = DateTime.Now;
            return String.Format("{0}:{1:00}:{2:00}", now.Hour, now.Minute, now.Second);
        }

        internal static void RegisterPhoenix()
        {
            RuntimeObjectsLoader rol = new RuntimeObjectsLoader();
            rol.AssemblyList.Add(typeof(Core).Assembly);
            rol.Report.Name = "Phoenix " + GetCurrentTimeString();
            reports.Add(rol.Report);
            rol.Run();

            if (rol.Report.AnalyzerErrors.Count > 0) {
                using (ReportViewerDialog viewer = new ReportViewerDialog(reports)) {
                    viewer.SelectedReport = rol.Report;
                    viewer.ShowDialog();
                }
            }
        }

        public static void ReloadPlugins()
        {
            lock (loadedPlugins.SyncRoot) {
                foreach (Assembly a in loadedPlugins) {
                    UnregisterAssembly(a);
                    // AssemblyDisposer.ClearAssembly(a); See in scripts
                }
                loadedPlugins.Clear();
            }

            DirectoryInfo pluginsDir = new DirectoryInfo(PluginsDirectory);
            FileInfo[] libs = pluginsDir.GetFiles("*.dll", SearchOption.AllDirectories);

            RuntimeObjectsLoader rol = new RuntimeObjectsLoader();
            rol.Report.Name = "Plugins " + GetCurrentTimeString();
            reports.Add(rol.Report);

            // Load assemblies
            foreach (FileInfo f in libs) {
                try {
                    Assembly a = Assembly.LoadFile(f.FullName);
                    rol.AssemblyList.Add(a);
                }
                catch (Exception e) {
                    Trace.WriteLine("Error loading plugin " + f.Name + ". Exception:\n" + e.ToString(), "Runtime");
                }
            }

            // Run analyzation
            rol.Run();

            // Add processed assemblies to list
            foreach (Assembly a in rol.Report.LoadedAssemblies) {
                loadedPlugins.Add(a);
            }

            if (rol.Report.AnalyzerErrors.Count > 0) {
                ReportViewer.Show(rol.Report);
            }
        }

        public static void CompileScripts()
        {
            CompileScripts(false);
        }

        public static void CompileScripts(bool silent)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(CompileScriptsInternal), silent);
        }

        private static void CompileScriptsInternal(object param)
        {
            Trace.WriteLine("Scripts compilation started.", "Runtime");

            try {
                RuntimeObjectsLoader rol = new RuntimeObjectsLoader();
                rol.Report.Name = "Scripts " + GetCurrentTimeString();
                rol.Report.Output.Add("Scripts directory: " + ScriptsDirectory);

                reports.Add(rol.Report);

                // Read file list
                DirectoryInfo scriptsDir = new DirectoryInfo(RuntimeCore.ScriptsDirectory);
                List<FileInfo> fileList = new List<FileInfo>(64);
                fileList.AddRange(scriptsDir.GetFiles("*.cs", SearchOption.AllDirectories));
                fileList.AddRange(scriptsDir.GetFiles("*.vb", SearchOption.AllDirectories));
                fileList.AddRange(scriptsDir.GetFiles("*.boo", SearchOption.AllDirectories));
                fileList.AddRange(scriptsDir.GetFiles("*.resx", SearchOption.AllDirectories));

                for (int i = 0; i < fileList.Count; i++) {
                    rol.FileList.Add(fileList[i].FullName);
                }

                rol.Report.Output.Add(rol.FileList.Count.ToString() + " files found.");
                rol.Report.Output.Add("");

                // Create reference list
                // - Phoenix.dll
                rol.ReferenceList.Add(Helper.GetAssemblyFileInfo(typeof(Core).Assembly).FullName);
                // - PhoenixShared.dll
                rol.ReferenceList.Add(Helper.GetAssemblyFileInfo(typeof(RuntimeAttribute).Assembly).FullName);
                // - MulLib.dll
                rol.ReferenceList.Add(Helper.GetAssemblyFileInfo(typeof(MulLib.Ultima).Assembly).FullName);
                // - MagicLibrary.dll
                rol.ReferenceList.Add(Helper.GetAssemblyFileInfo(typeof(Crownwood.Magic.Menus.PopupMenu).Assembly).FullName);

                lock (loadedPlugins.SyncRoot) {
                    foreach (Assembly a in loadedPlugins) {
                        rol.ReferenceList.Add(Helper.GetAssemblyFileInfo(a).FullName);
                    }
                }

                // Unregister old scripts
                lock (scriptAssemblies.SyncRoot) {
                    // First unregister registered script assemblies
                    foreach (Assembly a in scriptAssemblies) {
                        Debug.WriteLine("Unregistering assembly " + a.GetName().Name, "Runtime");
                        UnregisterAssembly(a);
                        // AssemblyDisposer.ClearAssembly(a);
                        // Caused problems that static classes was initialized after this was called and
                        // they registered some things when assembly should be unregistred.
                    }
                    scriptAssemblies.Clear();
                }

                // Compile and register

                bool silent = (bool)param;
                if (!silent)
                    ReportViewer.Show(rol.Report);

                rol.Run();

                if (silent && rol.Report.HasErrors)
                    ReportViewer.Show(rol.Report);

                // Add processed assemblies to list
                foreach (Assembly a in rol.Report.LoadedAssemblies) {
                    scriptAssemblies.Add(a);
                }
            }
            catch (Exception e) {
                string msg = "Unhandled exception during scripts compilation. Exception:\n" + e.ToString();
                Trace.WriteLine(msg, "Runtime");
                System.Windows.Forms.MessageBox.Show(msg, "Runtime Error");
            }
            finally {
                Trace.WriteLine("Scipts compilation finished.", "Runtime");
            }
        }

        static void Core_Disconnected(object sender, EventArgs e)
        {
            executions.TerminateAll();
        }
    }
}
