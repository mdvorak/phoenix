using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Globalization;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;
using Phoenix.Configuration;
using Phoenix.Communication;
using Phoenix.Gui;
using Phoenix.WorldData;
using Phoenix.Logging;
using System.Windows.Forms;
using Phoenix.Gui.Controls;

namespace Phoenix
{
    public static class Core
    {
        /// <summary>
        /// When log exceeds this size, it is compressed next launch.
        /// </summary>
        /// <value>5MB</value>
        public const int MaxLogSize = 1024 * 1024 * 5;
        public const int MaxSpeechLenght = 100;
        public const string SettingsFile = "Phoenix.xml";
        public const string VersionString = "Phoenix";
        public const string HomepageUrl = "http://www.amonthia.com/wiki/index.php/Phoenix";
        private const int WM_SETFPS = 0x3FFF;

        private static bool initialized;
        private static LaunchData launchData;
        private static ClientKeys clientKeys = new ClientKeys();
        private static SpellList spellList;
        private static CultureInfo culture;

        private static int clientThreadId;
        private static PhoenixGuiThread guiThread;
        private static IntPtr clientHWND;

        private static bool loggedIn = false;
        private static bool charListSent = false;

        #region Private functions

        private static void SetInitEvent()
        {
            if (launchData.LaunchEventId == null)
                throw new ArgumentNullException("LaunchData.LaunchEventId", "No event id specified.");

            EventWaitHandle hEvent = EventWaitHandle.OpenExisting(launchData.LaunchEventId);
            if (hEvent != null) {
                hEvent.Set();
            }
        }

        private static void InitLogging()
        {
            try {
                DirectoryInfo archiveDir = new DirectoryInfo(Path.Combine(launchData.PhoenixDir, "LogArchive"));

                if (!archiveDir.Exists)
                    archiveDir.Create();

                DateTime time = DateTime.Now;
                string archiveFileName = String.Format("phoenix_log {0}.{1}.{2} {3}.{4}.gzip",
                                                       time.Day, time.Month, time.Year.ToString().Substring(2), time.Hour, time.Minute);

                string archiveFile = Path.Combine(archiveDir.FullName, archiveFileName);
                if (File.Exists(archiveFile))
                    archiveFile = null;

                string logFile = Path.Combine(launchData.PhoenixDir, "phoenix_log.txt");

                Stream logStream = Phoenix.Logging.Log.OpenFile(logFile, true, archiveFile, MaxLogSize);
                Trace.Listeners.Add(new TextFileTraceListener(logStream));

                Log.AutoFlushInterval = 5000;
            }
            catch (Exception e) {
                Trace.WriteLine("Unhandled exception during log initialization. Log probably won't be saved. Details:\n" + e.ToString(), "Phoenix");
                ExceptionDialog.Show(e, "Unhandled exception during log initialization. Log probably won't be saved.");
            }
        }

        private delegate System.Windows.Forms.DialogResult MessageBoxShowDelegate(string text, string title);

        internal static void ShowMessageBoxAsync(string text, string title)
        {
            Phoenix.Runtime.ThreadStarter.Start(new MessageBoxShowDelegate(System.Windows.Forms.MessageBox.Show), text, title);
        }

        #endregion

        #region ComInterop events

        internal static void Initialize()
        {
            if (initialized)
                throw new InvalidProgramException();
            initialized = true;

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            Core.LoginComplete += new EventHandler(Core_LoginComplete);
            Core.Disconnected += new EventHandler(Core_Disconnected);

            launchData = LaunchData.FromCmdLine(System.Environment.CommandLine);

            clientThreadId = Thread.CurrentThread.ManagedThreadId;

            SetInitEvent();
            InitLogging();

            // TODO: Set security

            try {
                culture = CultureInfo.GetCultureInfo(launchData.Culture);
            }
            catch (Exception e) {
                culture = CultureInfo.GetCultureInfo("en-US");
                ExceptionDialog.Show(e, "Error creating requested culture.");
            }

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;

            if (!clientKeys.Load()) {
                ShowMessageBoxAsync("Unknown Client. Please use the same account and password you specified in launcher.", "Information");
            }
            spellList = new SpellList();

            Config.InternalSettings.Path = Path.Combine(launchData.PhoenixDir, SettingsFile);

            Config.Init();
            Config.Profile.ProfileChanged += new EventHandler(Profile_ProfileChanged);

            guiThread = new PhoenixGuiThread();

            // Run DataFiles.Load in different thread.
            Thread loadDataFilesThread = new Thread(new ParameterizedThreadStart(DataFiles.Load));
            loadDataFilesThread.Start(launchData.UltimaDir);

            Phoenix.Logging.JournalHandler.Init();
            Notepad.Init();
            Phoenix.Runtime.ReportViewer.Init();
            Journal.Init();
            WorldPacketHandler.Init();
            Aliases.Init();
            LoginInfo.Init();
            UIManager.Init();
            SpeechColorOverride.Init();
            Phoenix.Runtime.ClientMessageHandler.Init();
            Phoenix.Runtime.RuntimeCore.Hotkeys.Init();
            LatencyMeasurement.Init();

            // Create scripts and plugins dir if doesn't exists
            DirectoryInfo scriptsDir = new DirectoryInfo(Phoenix.Runtime.RuntimeCore.ScriptsDirectory);
            if (!scriptsDir.Exists) scriptsDir.Create();

            DirectoryInfo pluginsDir = new DirectoryInfo(Phoenix.Runtime.RuntimeCore.PluginsDirectory);
            if (!pluginsDir.Exists) pluginsDir.Create();

            Core.RegisterServerMessageCallback(0xA9, new MessageCallback(OnCharacterList), CallbackPriority.Highest);
            Core.RegisterServerMessageCallback(0x1B, new MessageCallback(OnLoginConfirmedPacket), CallbackPriority.Normal);
            Core.RegisterServerMessageCallback(0x55, new MessageCallback(OnLoginCompletePacket), CallbackPriority.Highest);

            loadDataFilesThread.Join();

            PlayerSkills.Init();

            Config.Profile.FpsLimit.Changed += new EventHandler(FrameLimit_Changed);

            Config.InternalSettings.Load();
            Config.Profile.LoadDefault();

            Phoenix.Runtime.RuntimeCore.RegisterPhoenix();
            Phoenix.Runtime.RuntimeCore.ReloadPlugins();
            Phoenix.Runtime.RuntimeCore.CompileScripts(true);

            Trace.WriteLine("Phoenix initialized.", DateTime.Now.ToString());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string message;
            if (e.ExceptionObject != null)
                message = "Unhandled exception:\n" + e.ExceptionObject.ToString();
            else
                message = "Undefined unhandled exception occurred in application.";

            Trace.WriteLine(message, "Phoenix");

            if (e.ExceptionObject is Exception) {
                FatalExceptionDialog dlg = new FatalExceptionDialog();
                dlg.Exception = (Exception)e.ExceptionObject;
                dlg.TryToContinueEnabled = !e.IsTerminating;

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return;
            }
            else {
                System.Windows.Forms.MessageBox.Show(message, "Fatal Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            Core.Terminate();
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < loadedAssemblies.Length; i++) {
                if (loadedAssemblies[i].FullName == args.Name)
                    return loadedAssemblies[i];
            }

            return null;
        }

        internal static void OnWindowCreated(IntPtr hwnd)
        {
            clientHWND = hwnd;
            Client.OnWindowCreated(hwnd);

            Client.PostMessage(WM_SETFPS, Config.Profile.FpsLimit.Value, 0);
            guiThread.ShowPhoenixWindow();
        }


        [DllImport("user32.dll")]
        [SuppressUnmanagedCodeSecurity]
        static extern IntPtr GetForegroundWindow();

        internal static void OnFocusChanged(bool focused)
        {
            // This value lies on faked clicks
            focused = GetForegroundWindow() == clientHWND;

            if (!Config.Profile.Window.TopMost) {
                guiThread.SetWindowTopMost(focused && Config.Profile.Window.StayOnTop);
            }

            Client.WindowFocus = focused;
        }

        internal static bool OnKeyDown(System.Windows.Forms.Keys keyCode, uint repCount, bool prevState)
        {
            if (Phoenix.Runtime.RuntimeCore.Hotkeys.Contains(keyCode.ToString())) {
                if (!prevState) {
                    Phoenix.Runtime.RuntimeCore.Hotkeys.Exec(keyCode.ToString());
                }
                return true;
            }
            else {
                return false;
            }
        }

        internal static bool OnAppCommand(ushort cmdCode)
        {
            string cmd = ((AppCommand)cmdCode).ToString();
            if (Phoenix.Runtime.RuntimeCore.Hotkeys.Contains(cmd)) {
                Phoenix.Runtime.RuntimeCore.Hotkeys.Exec(cmd);
                return true;
            }
            else {
                return false;
            }
        }

        internal static bool OnMouseWheel(short delta)
        {
            string cmd = delta > 0 ? "WheelUp" : "WheelDown";

            Keys mods = Control.ModifierKeys;
            if (mods != Keys.None)
                cmd += ", " + mods.ToString();

            if (Phoenix.Runtime.RuntimeCore.Hotkeys.Contains(cmd)) {
                Phoenix.Runtime.RuntimeCore.Hotkeys.Exec(cmd);
                return true;
            }
            else {
                return false;
            }
        }

        internal static bool HasBeenCharListSent()
        {
            return charListSent;
        }

        internal static void OnExitProcess(int exitCode)
        {
            OnShuttingDown(EventArgs.Empty);

            Config.Save();

            guiThread.ClosePhoenixWindow();

            DataFiles.Dispose();

            Trace.Close();
        }

        #endregion

        #region Event handling

        private static CallbackResult OnCharacterList(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0xA9)
                throw new ArgumentException("Invalid packet passed to OnCharacterList.");

            charListSent = true;

            return CallbackResult.Normal;
        }

        private static CallbackResult OnLoginConfirmedPacket(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0x1B)
                throw new ArgumentException("Invalid packet passed to OnLoginConfirmed.");

            uint playerSerial = ByteConverter.BigEndian.ToUInt32(data, 1);

            if (!Config.InternalSettings.ElementExists("Characters", new ElementInfo("Character", new AttributeInfo("serial", playerSerial.ToString("X8"))))) {
                SelectProfile("NewProfileName", false);
            }
            else {
                Config.Profile.ChangeProfile(Config.PlayerProfile);
            }

            return CallbackResult.Normal;
        }

        private static CallbackResult OnLoginCompletePacket(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0x55)
                throw new ArgumentException("Invalid packet passed to OnLoginCompletePacket.");

            if (!loggedIn) {
                loggedIn = true;
                OnLoginComplete(EventArgs.Empty);

                return CallbackResult.Normal;
            }
            else {
                Debug.WriteLine("Unexpected LoginComplete packet dropped.", "Communication");
                return CallbackResult.Eat;
            }
        }

        static void Core_LoginComplete(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(PrintWelcomeMessage)).Start();
        }

        static void PrintWelcomeMessage()
        {
            using (ObjectChangedEventWaiter ew = new ObjectChangedEventWaiter(World.PlayerSerial)) {
                Thread.Sleep(1000);
                ew.Wait(1000);
                UO.Print(SpeechFont.Bold, 0x0434, "Phoenix loaded.");
            }
        }

        static void Core_Disconnected(object sender, EventArgs e)
        {
            charListSent = false;
            Config.Profile.ChangeProfile("Default");

            Trace.WriteLine(String.Format("Client disconnected."), "Information");
            Trace.Flush();
        }

        static void Profile_ProfileChanged(object sender, EventArgs e)
        {
            UO.Print("Profile changed to '{0}'", Config.Profile.ProfileName);
        }

        static void FrameLimit_Changed(object sender, EventArgs e)
        {
            Client.PostMessage(WM_SETFPS, Config.Profile.FpsLimit.Value, 0);
        }

        #endregion

        #region Events

        private static DefaultPublicEvent disconnected = new DefaultPublicEvent();
        private static DefaultPublicEvent loginComplete = new DefaultPublicEvent();
        internal static event EventHandler ShuttingDown;

        public static event EventHandler Disconnected
        {
            add { disconnected.AddHandler(value); }
            remove { disconnected.RemoveHandler(value); }
        }

        public static event EventHandler LoginComplete
        {
            add { loginComplete.AddHandler(value); }
            remove { loginComplete.RemoveHandler(value); }
        }

        internal static void OnLoginComplete(EventArgs e)
        {
            try {
                loginComplete.Invoke(null, e);
            }
            catch (Exception e0) {
                Trace.WriteLine(String.Format("Error in LoginComplete event. Exception:\r\n{0}", e0), "Phoenix");
            }
        }

        internal static void OnDisconnected(EventArgs e)
        {
            loggedIn = false;
            Config.Save();

            try {
                disconnected.Invoke(null, e);
            }
            catch (Exception e0) {
                Trace.WriteLine(String.Format("Error in Disconnected event. Exception:\r\n{0}", e0), "Phoenix");
            }
        }

        internal static void OnShuttingDown(EventArgs e)
        {
            SyncEvent.Invoke(ShuttingDown, null, e);
        }

        #endregion

        internal static LaunchData LaunchData
        {
            get { return Core.launchData; }
        }

        internal static ClientKeys ClientKeys
        {
            get { return Core.clientKeys; }
        }

        internal static bool CharListSent
        {
            get { return Core.charListSent; }
            set { Core.charListSent = value; }
        }

        /// <summary>
        /// Gets phoenix gui thread.
        /// </summary>
        internal static PhoenixGuiThread GuiThread
        {
            get { return guiThread; }
        }

        public static Form Window
        {
            get { return guiThread.Window; }
        }

        /// <summary>
        /// Gets Phoenix culture.
        /// </summary>
        public static CultureInfo Culture
        {
            get { return Core.culture; }
        }

        /// <summary>
        /// Gets id of communication and gui thread.
        /// </summary>
        public static int ClientThreadId
        {
            get { return Core.clientThreadId; }
        }

        /// <summary>
        /// Gets wheter this is main thread.
        /// </summary>
        public static bool IsClientThread
        {
            get { return Thread.CurrentThread.ManagedThreadId == clientThreadId; }
        }

        /// <summary>
        /// Gets client window handle.
        /// </summary>
        [Obsolete("Use Client.HWND instead.")]
        public static IntPtr ClientHWND
        {
            get { return Core.clientHWND; }
        }

        /// <summary>
        /// Gets Phoenix assembly version.
        /// </summary>
        public static Version Version
        {
            get { return System.Reflection.Assembly.GetAssembly(typeof(Core)).GetName().Version; }
        }

        /// <summary>
        /// Gets Phoenix directory.
        /// </summary>
        public static string Directory
        {
            get { return Core.LaunchData.PhoenixDir; }
        }

        /// <summary>
        /// Gets wheter player is logged in.
        /// </summary>
        public static bool LoggedIn
        {
            get { return Core.loggedIn; }
        }

        /// <summary>
        /// Gets spell list.
        /// </summary>
        public static SpellList SpellList
        {
            get { return Core.spellList; }
        }

        /// <summary>
        /// Gets average server latency in milliseconds.
        /// </summary>
        public static int Latency
        {
            get { return Phoenix.Communication.LatencyMeasurement.AverageLatency; }
        }

        /// <summary>
        /// Gets latest server latency in milliseconds.
        /// </summary>
        public static int CurrentLatency
        {
            get { return Phoenix.Communication.LatencyMeasurement.CurrentLatency; }
        }

        public static void Terminate()
        {
            Core.OnExitProcess(-1);
            Environment.Exit(-1);
        }

        internal static void SelectProfile(string selectedProfile, bool cancelEnabled)
        {
            guiThread.SelectProfile(selectedProfile, cancelEnabled);
        }

        #region Message handling

        private static MessageCallbacksCollection serverMsgCallbacks = new MessageCallbacksCollection("ServerMessage");
        private static MessageCallbacksCollection clientMsgCallbacks = new MessageCallbacksCollection("ClientMessage");

        public static void RegisterServerMessageCallback(byte id, MessageCallback callback)
        {
            serverMsgCallbacks.Add(id, callback, CallbackPriority.Normal);
        }

        public static void RegisterServerMessageCallback(byte id, MessageCallback callback, CallbackPriority priority)
        {
            serverMsgCallbacks.Add(id, callback, priority);
        }

        public static void UnregisterServerMessageCallback(byte id, MessageCallback callback)
        {
            serverMsgCallbacks.Remove(id, callback);
        }

        public static void RegisterClientMessageCallback(byte id, MessageCallback callback)
        {
            clientMsgCallbacks.Add(id, callback, CallbackPriority.Normal);
        }

        public static void RegisterClientMessageCallback(byte id, MessageCallback callback, CallbackPriority priority)
        {
            clientMsgCallbacks.Add(id, callback, priority);
        }

        public static void UnregisterClientMessageCallback(byte id, MessageCallback callback)
        {
            clientMsgCallbacks.Remove(id, callback);
        }

        internal static CallbackResult OnServerMessage(byte[] data, CallbackResult prevResult)
        {
            return serverMsgCallbacks.ProcessMessage(data, prevResult);
        }

        internal static CallbackResult OnClientMessage(byte[] data, CallbackResult prevResult)
        {
            return clientMsgCallbacks.ProcessMessage(data, prevResult);
        }

        #endregion

        private static GenericSendLimiter serverSendLimiter = new GenericSendLimiter(5);

        /// <summary>
        /// Sends data directly to the client.
        /// </summary>
        /// <param name="data">Buffer to send.</param>
        public static void SendToClient(byte[] data)
        {
            UltimaSocket socket = CommunicationManager.Socket;
            if (socket != null) {
                socket.SendToClient(data);
            }
        }

        /// <summary>
        /// If <paramref name="callHandlers"/> is true, OnServerMessage handlers are called and
        /// if result is <see cref="CallbackResult.Normal"/> data are sent to the client.
        /// </summary>
        /// <param name="data">Buffer to send.</param>
        /// <param name="callHandlers">True if OnServerMessage handlers should be called; otherwise false.</param>
        public static void SendToClient(byte[] data, bool callHandlers)
        {
            UltimaSocket socket = CommunicationManager.Socket;
            if (socket != null) {
                CallbackResult result = CallbackResult.Normal;

                if (callHandlers)
                    result = OnServerMessage(data, CallbackResult.Normal);

                if (result == CallbackResult.Normal)
                    socket.SendToClient(data);
            }
        }

        /// <summary>
        /// Sends data directly to the server.
        /// </summary>
        /// <param name="data">Buffer to send.</param>
        /// <remarks>
        /// Message is not sent immediatly, instead it is queued and sent from client thread.
        /// </remarks>
        public static void SendToServer(byte[] data)
        {
            UltimaSocket socket = CommunicationManager.Socket;
            if (socket != null) {
                serverSendLimiter.Send();
                socket.SendToServer(data);
            }
        }

        /// <summary>
        /// If <paramref name="callHandlers"/> is true, OnClientMessage handlers are called and
        /// if result is <see cref="CallbackResult.Normal"/> data are sent to the client.
        /// </summary>
        /// <param name="data">Buffer to send.</param>
        /// <param name="callHandlers">True if OnClientMessage handlers should be called; otherwise false.</param>
        /// <remarks>
        /// Message is not sent immediatly, instead it is queued and sent from client thread.
        /// </remarks>
        public static void SendToServer(byte[] data, bool callHandlers)
        {
            UltimaSocket socket = CommunicationManager.Socket;
            if (socket != null) {
                CallbackResult result = CallbackResult.Normal;

                if (callHandlers)
                    result = OnClientMessage(data, CallbackResult.Normal);

                if (result == CallbackResult.Normal) {
                    serverSendLimiter.Send();
                    socket.SendToServer(data);
                }
            }
        }
    }
}
