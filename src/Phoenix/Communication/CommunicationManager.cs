using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using UOEncryption;

namespace Phoenix.Communication
{
    internal static class CommunicationManager
    {
        private static GameEncryptionType clientEncryption = GameEncryptionType.None;
        private static GameEncryptionType serverEncryption = GameEncryptionType.None;

        public static readonly object SyncRoot = new object();
        private static UltimaSocket ultimaSocket = null;
        private static bool redirecting = false;
        private static uint loginSeed = 0;
        private static uint gameSeed = 0;
        private static string username = null;
        private static string password = null;

        private static BandwidthManager bandwidthManager = new BandwidthManager();

        internal static GameEncryptionType ClientEncryption
        {
            get { return clientEncryption; }
            set { clientEncryption = value; }
        }

        internal static GameEncryptionType ServerEncryption
        {
            get { return serverEncryption; }
            set { serverEncryption = value; }
        }

        internal static bool Redirecting
        {
            get { return redirecting; }
            set { redirecting = value; }
        }

        internal static uint LoginSeed
        {
            get { return loginSeed; }
            set { loginSeed = value; }
        }

        internal static uint GameSeed
        {
            get { return gameSeed; }
            set { gameSeed = value; }
        }

        internal static string Username
        {
            get { return CommunicationManager.username; }
            set { CommunicationManager.username = value; }
        }

        internal static string Password
        {
            get { return CommunicationManager.password; }
            set { CommunicationManager.password = value; }
        }

        /// <summary>
        /// Gets current socket or null.
        /// </summary>
        internal static UltimaSocket Socket
        {
            get { return ultimaSocket; }
        }

        /// <summary>
        /// Gets if client is connected to server.
        /// </summary>
        public static bool Connected
        {
            get { return ultimaSocket != null; }
        }

        /// <summary>
        /// Gets BandwidthManager.
        /// </summary>
        public static BandwidthManager BandwidthManager
        {
            get { return bandwidthManager; }
        }

        static CommunicationManager()
        {
            Core.ShuttingDown += new EventHandler(Core_ShuttingDown);
        }

        internal static void CheckThread()
        {
            if (!Core.IsClientThread) {
                throw new Exception("Function called from wrong thread.");
            }
        }

        /// <summary>
        /// Client is connecting to somewhere.
        /// </summary>
        public static void OnConnect(long s, uint address, int port)
        {
            CheckThread();

            lock (SyncRoot) {
                byte[] ip = ByteConverter.LittleEndian.GetBytes(address);
                Trace.WriteLine(String.Format("Connecting to {1}.{2}.{3}.{4},{5} on socket {0}.", s, ip[0], ip[1], ip[2], ip[3], port), "Communication");

                if (port != 28888) // Translation server? (copied from UOInjection)
                {
                    if (!redirecting) {
                        ultimaSocket = new LoginSocket(s, address, port);
                        redirecting = false;
                    }
                    else {
                        ultimaSocket = new GameSocket(s, address, port);
                        redirecting = false;
                    }
                }
            }
        }

        /// <summary>
        /// Client is closing socket.
        /// </summary>
        public static void OnCloseSocket(long s)
        {
            if (!Core.IsClientThread)
                Trace.WriteLine("Warning: Closing socket from different thread.", "Communication");

            bool callDisconnected = false;

            lock (SyncRoot) {
                Trace.WriteLine(String.Format("Closing socket {0}.", s), "Communication");

                if (ultimaSocket != null && ultimaSocket.Socket == s) {
                    // Client is disconnected.
                    ultimaSocket = null;
                    callDisconnected = !redirecting;
                }
            }

            // Mimo sync kontext
            if (callDisconnected) {
                Core.OnDisconnected(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Client is requesting data.
        /// </summary>
        public static int OnRecv(long s, byte[] buff, int len, int flags)
        {
            CheckThread();

            lock (SyncRoot) {
                if (ultimaSocket != null && ultimaSocket.Socket == s) {
                    return ultimaSocket.Recv(s, buff, len, flags);
                }
                else {
                    return WinSock.recv(s, buff, len, flags);
                }
            }
        }

        /// <summary>
        /// Client is sending data.
        /// </summary>
        public static int OnSend(long s, byte[] buff, int len, int flags)
        {
            CheckThread();

            lock (SyncRoot) {
                if (ultimaSocket != null && ultimaSocket.Socket == s) {
                    if (redirecting) {
                        Trace.WriteLine("Warning: Client is reusing login socket as game socket.", "Communication");
                        ultimaSocket = new GameSocket(s, ultimaSocket.Address, ultimaSocket.Port, loginSeed);
                        redirecting = false;
                    }

                    return ultimaSocket.Send(s, buff, len, flags);
                }
                else {
                    return WinSock.send(s, buff, len, flags);
                }
            }
        }

        #region Functions needed for winsock select function

        public static void SendData()
        {
            CheckThread();

            lock (SyncRoot) {
                if (ultimaSocket != null)
                    ultimaSocket.SendData();
                else
                    throw new ArgumentException("SendPendingData requested at unmanaged socket.");
            }
        }

        public static void ReceiveData(long s)
        {
            CheckThread();

            lock (SyncRoot) {
                if (ultimaSocket != null && ultimaSocket.Socket == s)
                    ultimaSocket.ReceiveData();
                else
                    throw new ArgumentException("ReceiveData requested at unmanaged socket.");
            }
        }

        public static bool DataAvailable(long s)
        {
            CheckThread();

            lock (SyncRoot) {
                if (ultimaSocket != null && ultimaSocket.Socket == s)
                    return ultimaSocket.DataAvailable;
                else
                    return false;
            }
        }

        public static long ManagedSocket()
        {
            lock (SyncRoot) {
                if (ultimaSocket != null)
                    return ultimaSocket.Socket;
                else
                    return 0;
            }
        }

        #endregion

        static void Core_ShuttingDown(object sender, EventArgs e)
        {
            if (ultimaSocket != null) {
                Trace.WriteLine(String.Format("Warning: Exiting with opened socket {0}. Closing..", ultimaSocket.Socket), "Communication");

                WinSock.closesocket(ultimaSocket.Socket);
                ultimaSocket = null;
            }
        }
    }
}
