using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace Phoenix.Gui
{
    public static class Helper
    {
        public static void CopyObjectToClipboardSafe(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA) {
                Thread t = new Thread(new ParameterizedThreadStart(ClipboardWorker));
                t.SetApartmentState(ApartmentState.STA);
                t.Start(obj);
            }
            else {
                Clipboard.SetDataObject(obj, true);
            }
        }

        private static void ClipboardWorker(object obj)
        {
            Clipboard.Clear();
            Clipboard.SetDataObject(obj, true);
        }

        public static void OpenBroswer(string address)
        {
            Phoenix.Runtime.ThreadStarter.StartBackround(new ParameterizedThreadStart(OpenBroswerWorker), address);
        }

        private static void OpenBroswerWorker(object parameter)
        {
            try {
                Debug.WriteLine("Opening web broswer..", "Gui");

                ProcessStartInfo info = new ProcessStartInfo();
                info.UseShellExecute = true;
                info.Verb = "open";
                info.FileName = parameter.ToString();
                Process.Start(info);

                Debug.WriteLine("Broswer opened", "Gui");
            }
            catch (Exception e) {
                Trace.WriteLine("Unable to open broswer. Exception:\n" + e.ToString(), "Gui");
                MessageBox.Show("Unable to open broswer.", "Error");
            }
        }

        public static System.Drawing.Color GetStatusColor(float coeficient, int brightness)
        {
            int r = Math.Max(Math.Min((int)(coeficient * brightness), brightness), 0);
            int g = Math.Max(Math.Min((int)((-coeficient + 2) * brightness), brightness), 0);
            return System.Drawing.Color.FromArgb(r, g, 0);
        }

        public static AppCommand GET_APPCOMMAND_LPARAM(int lParam)
        {
            return (AppCommand)((lParam >> 16) & ~0xf000);
        }
    }
}
