using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Phoenix.Configuration;
using PhoenixLauncher.Data;
using PhoenixLauncher.Properties;
using Phoenix.Utils;

namespace PhoenixLauncher
{
    public partial class Launcher : Form
    {
        private Server server;
        private Account account;
        private Thread worker;
        private bool success;
        private bool forceRegistryUpdate;

        public Launcher()
        {
            InitializeComponent();
            resultTextBox.SelectionAlignment = HorizontalAlignment.Right;
            progressBar.Minimum = 0;
            progressBar.Maximum = 8;

            worker = new Thread(new ThreadStart(Worker));
            success = false;
            forceRegistryUpdate = false;
        }

        [Browsable(false)]
        public Server Server
        {
            get { return server; }
            set { server = value; }
        }

        [Browsable(false)]
        public Account Account
        {
            get { return account; }
            set { account = value; }
        }

        [Browsable(false)]
        public bool Success
        {
            get { return success; }
        }

        [Browsable(false)]
        public bool ForceRegistryUpdate
        {
            get { return forceRegistryUpdate; }
            set { forceRegistryUpdate = value; }
        }

        private void Launcher_Load(object sender, EventArgs e)
        {
            worker.Start();
        }

        private void ResetProgress()
        {
            Trace.WriteLine("Resetting progress");
            Safe.ResetText(progressTextBox);
            Safe.ResetText(resultTextBox);
            Safe.SetValue(progressBar, 0);
        }

        private void PrintEvent(string text)
        {
            Trace.WriteLine(text, "Event");
            Safe.SetSelectionColor(progressTextBox, System.Drawing.Color.Black);
            Safe.AppendText(progressTextBox, text + "\n");
        }

        private void PrintResult(string text, System.Drawing.Color color)
        {
            Trace.WriteLine(text, "Result");
            Safe.SetSelectionColor(resultTextBox, color);
            Safe.AppendText(resultTextBox, text + "\n");
            Safe.SetValue(progressBar, progressBar.Value + 1);
        }

        private void PrintError()
        {
            Trace.WriteLine("Error");
            Safe.SetSelectionColor(resultTextBox, System.Drawing.Color.Red);
            Safe.AppendText(resultTextBox, Resources.Launcher_Error + Environment.NewLine);
        }

        private void Worker()
        {
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            try {
                Safe.SetEnabled(abortButton, true);
                Safe.SetEnabled(exitButton, false);
                ResetProgress();

                LaunchData info = new LaunchData();
                info.LaunchEventId = "phoenix_load_" + new Random().Next(0xFFFF).ToString();
                info.ClientExe = server.ClientExe;
                info.UltimaDir = server.UltimaDir;
                info.PhoenixDir = Constants.PhoenixDir;
                info.Address = server.Address;
                info.ServerEncName = server.Encryption;
                info.Username = account.Name;
                info.Password = account.Password;

                // Read encryption
                UOKey keys;
                if (Constants.UOKeys.List.TryGetValue(server.Encryption, out keys)) {
                    info.ServerEncryption = keys.GameEncryption.ToString();
                    info.ServerKey1 = keys.Key1;
                    info.ServerKey2 = keys.Key2;
                }
                else {
                    PrintEvent(Resources.Launcher_LoadingEncryption + "..");
                    throw new Exception(Resources.Launcher_CannotFindEncryption + " UOKeys.cfg");
                }

                // Check client list for selected client
                PrintEvent(Resources.Launcher_CheckingClient + "..");
                bool clientCheck = CheckClient(out info.ClientHash);
                if (clientCheck)
                    PrintResult(Resources.Launcher_Known, System.Drawing.Color.Green);
                else
                    PrintResult(Resources.Launcher_Unknown, System.Drawing.Color.Black);

                // Save config files
                PrintEvent(Resources.Launcher_Saving + " login.cfg..");
                LaunchEvents.SaveLoginCfg(server.UltimaDir, server.Address);
                PrintResult(Resources.Launcher_Done, System.Drawing.Color.Green);

                PrintEvent(Resources.Launcher_Saving + " uo.cfg..");
                LaunchEvents.SaveUoCfg(server, account);
                PrintResult(Resources.Launcher_Done, System.Drawing.Color.Green);

                // Update registry
                PrintEvent(Resources.Launcher_UpdatingRegistry + "..");
                if (LaunchEvents.UpdateRegistry(server.UltimaDir, forceRegistryUpdate))
                    PrintResult(Resources.Launcher_Done, System.Drawing.Color.Green);
                else
                    PrintResult(Resources.Launcher_Skipped, Color.Black);

                // Start suspended client
                PrintEvent(Resources.Launcher_StartingClient + "..");
                STARTUPINFO si = new STARTUPINFO();
                si.cb = Marshal.SizeOf(si);

                if (!Api.CreateProcess(null, info.ToString(), IntPtr.Zero, IntPtr.Zero, false,
                          CreationFlags.CREATE_SUSPENDED, IntPtr.Zero, info.UltimaDir, ref si, out pi)) {
                    uint err = Api.GetLastError();
                    throw new Exception(Resources.Launcher_UnableToStartClient + " " + Resources.Launcher_ErrorNumber + " = 0x" + err.ToString("X"));
                }
                PrintResult(Resources.Launcher_Done, System.Drawing.Color.Green);

                // Patch client
                PrintEvent(Resources.Launcher_PatchingClient + "..");
                LaunchEvents.PatchClient(pi.hProcess, pi.hThread);
                PrintResult(Resources.Launcher_Done, System.Drawing.Color.Green);

                // Resume client execution
                PrintEvent(Resources.Launcher_RunningClient + "..");

                EventWaitHandle hEvent = new EventWaitHandle(false, EventResetMode.AutoReset, info.LaunchEventId);
                if (((int)Api.ResumeThread(pi.hThread)) < 0) {
                    uint err = Api.GetLastError();
                    throw new Exception(Resources.Launcher_UnableToResumeClient + " " + Resources.Launcher_ErrorNumber + " = 0x" + err.ToString("X"));
                }

                if (!hEvent.WaitOne(8000, false))
                    throw new Exception(Resources.Launcher_UnableToDetectPhoenix);
                else
                    PrintResult(Resources.Launcher_Done, System.Drawing.Color.Green);

                success = true;
                PrintEvent(Resources.Launcher_Finished);
                Safe.SetValue(progressBar, progressBar.Value + 1);

                Safe.SetEnabled(abortButton, false);
                Safe.SetText(exitButton, Resources.Launcher_Done);
                Safe.SetEnabled(exitButton, true);

                Thread.Sleep(2000);
                Safe.Close(this);
            }
            catch (Exception e) {
                PrintError();

                Safe.SetEnabled(abortButton, false);
                Safe.SetText(exitButton, Resources.Launcher_Exit);
                Safe.SetEnabled(exitButton, true);

                success = false;
                if (pi.hProcess != null) Api.TerminateProcess(pi.hProcess, uint.MaxValue);

                MessageBox.Show(e.Message, Resources.Launcher_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckClient(out string clientHash)
        {
            if (!File.Exists(server.ClientExe))
                throw new Exception("Client executable not found.");

            try {
                clientHash = MD5.ComputeHash(server.ClientExe);

                ClientList clientList = new ClientList();
                clientList.Path = Path.Combine(Constants.PhoenixDir, ClientList.Filename);
                clientList.Load();

                ClientInfo clientInfo = clientList.FindClient(clientHash);
                if (clientInfo.Hash != null && clientInfo.Key1 != null && clientInfo.Key2 != null)
                    return true;
                else
                    return false;
            }
            catch (Exception e) {
                throw new Exception(Resources.Launcher_ErrorCalculatingMD5, e);
            }
        }

        private void CheckAccessRights(DirectoryInfo dir)
        {
            /*       dir.GetAccessControl().GetOwner(

                   DirectoryInfo[] dirs = dir.GetDirectories();

                   for (int i = 0; i < dirs.Length; i++)
                   {
                       CheckAccessRights(dirs[i]);
            

                   FileInfo[] files = dir.GetFiles();*/
        }

        private void abortButton_Click(object sender, EventArgs e)
        {
            worker.Abort();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}