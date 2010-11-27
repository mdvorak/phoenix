using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;

namespace PhoenixLauncher.Data
{
    internal static class Constants
    {
        public static string Client;
        public static string UltimaDir;
        public readonly static string PhoenixDir;
        public readonly static UOKeysLoader UOKeys = new UOKeysLoader();

        static Constants()
        {
            // Open registry key
            RegistryKey rkey = Registry.LocalMachine.OpenSubKey(@"Software\Origin Worlds Online\Ultima Online Third Dawn\1.0");
            if (rkey == null) rkey = Registry.LocalMachine.OpenSubKey(@"Software\Origin Worlds Online\Ultima Online\1.0");
            if (rkey == null)
            {
                Trace.TraceError("Cannot find Ultima Online key in the registry.");
            }
            else
            {
                // Read exe path from registry value
                string exe = rkey.GetValue("ExePath") as string;
                if (exe == null)
                {
                    Trace.TraceError("Cannot read exe path from the registry.");
                }
                else
                {
                    string path = exe.Remove(exe.LastIndexOf('\\'));

                    Client = exe;
                    UltimaDir = path;
                }
            }

            PhoenixDir = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(Constants)).CodeBase)).LocalPath;

            string uokeysPath = System.IO.Path.Combine(PhoenixDir, "UOKeys.cfg");
            if (!UOKeys.Load(uokeysPath))
            {
                System.Windows.Forms.MessageBox.Show("Unable to load " + uokeysPath, "Error");
            }
        }
    }
}
