using System;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace PhoenixLauncher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Stream logStream = null;

            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
                try {
                    DateTime time = DateTime.Now;
                    string archiveFileName = String.Format("plauncher_log {0}.{1}.{2} {3}.{4}.gzip",
                                                            time.Day, time.Month, time.Year.ToString().Substring(2), time.Hour, time.Minute);


                    logStream = Phoenix.Logging.Log.OpenFile("plauncher_log.txt", true, "LogArchive\\" + archiveFileName, 1024 * 1024);
                    Debug.Listeners.Add(new TextWriterTraceListener(logStream));
                }
                catch (Exception e) {
                    Trace.TraceWarning("Unhandled exception during launcher log opening. Log probably won't be saved. Details:\n" + e.ToString());
                    Phoenix.Gui.ExceptionDialog.Show(e, "Unhandled exception during launcher log opening. Log probably won't be saved.");
                }
#endif

                Phoenix.Configuration.Settings settings = new Phoenix.Configuration.Settings("Launcher");
                settings.Path = MainWindow.PhoenixLauncherCfg;
                settings.Load();

                string culture = settings.GetElement("en-US", "PhoenixCulture");
                if (culture.Length == 0)
                    culture = "en-US";

                try {
                    CultureInfo c = CultureInfo.GetCultureInfo(culture);

                    System.Threading.Thread.CurrentThread.CurrentCulture = c;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = c;
                }
                catch (Exception e) {
                    Trace.TraceWarning("Invalid culture in settings. Details:\n{0}", e);
                    Phoenix.Gui.ExceptionDialog.Show(e, "Invalid culture in settings.");
                }
        
                MainWindow window = new MainWindow();
                window.Settings = settings;

                Application.Run(window);

                window.Dispose();
            }
            finally {
                Trace.Close();

                if (logStream != null)
                    logStream.Close();
            }
        }
    }
}