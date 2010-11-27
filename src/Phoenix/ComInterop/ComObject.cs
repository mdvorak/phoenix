using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Phoenix;
using Phoenix.Communication;
using Phoenix.Gui;

namespace ComInterop
{
    [ComVisible(true)]
    [Guid("96514A6D-7E86-45d9-AEE2-6FAC98C2502B")]
    public interface IComObject
    {
        void Init(IWinSock winSock);

        void OnWindowCreated(IntPtr hwnd);
        void OnFocusChanged(bool focused);
        bool OnKeyDown(uint keyCode, uint repCount, bool previousState);
        bool OnAppCommand(ushort keyCode);
        bool OnMouseWheel(short delta);
        void OnTextChanged(string text);

        // Communication
        void OnConnect(long socket, uint address, int port);
        void OnCloseSocket(long socket);

        void SendPendingData();
        void ReceiveData(long socket);
        bool DataAvailable(long socket);
        long ManagedSocket();

        int OnRecv(long socket, [Out] byte[] buff, int len, int flags);
        int OnSend(long socket, [In] byte[] buff, int len, int flags);

        bool HasBeenCharListSent();
        void OnExitProcess(int exitCode);

        void WriteToLog(string message);
    }

    [ComVisible(true)]
    [Guid("7E57909D-C2D1-42ef-9489-46183B598D56")]
    [ClassInterface(ClassInterfaceType.None)]
    public class ComObject : IComObject
    {
#if TIMEDEBUG
        Stopwatch watch = new Stopwatch();
#endif

        public ComObject()
        {
        }

        private void Exit(Exception e)
        {
            Trace.TraceError(e.ToString());
            Trace.Flush();

            FatalExceptionDialog dlg = new FatalExceptionDialog();
            dlg.Exception = e;

            if (dlg.ShowDialog() != DialogResult.Retry) {
                Core.Terminate();
            }
            else {
                Trace.TraceInformation("Trying to continue.");
                Trace.Flush();
            }

            dlg.Dispose();
        }

        void IComObject.Init(IWinSock winSock)
        {
            try {
                if (winSock == null) return;

                WinSock.Initialize(winSock);
                Core.Initialize();
            }
            catch (Exception e) {
                Exit(e);
            }
        }

        void IComObject.OnConnect(long socket, uint address, int port)
        {
            try {
                CommunicationManager.OnConnect(socket, address, port);
            }
            catch (Exception e) {
                Exit(e);
            }
        }

        void IComObject.OnCloseSocket(long socket)
        {
            try {
                CommunicationManager.OnCloseSocket(socket);
            }
            catch (Exception e) {
                Exit(e);
            }
        }

        int IComObject.OnRecv(long socket, [Out] byte[] buff, int len, int flags)
        {
#if TIMEDEBUG
            watch.Reset();
            watch.Start();
#endif

            try {
                return CommunicationManager.OnRecv(socket, buff, len, flags);
            }
            catch (Exception e) {
                Exit(e);
                return -1;
            }

#if TIMEDEBUG
            finally
            {
                watch.Stop();
                Debug.WriteLine("\t" + watch.Elapsed.TotalMilliseconds, "IComObject.OnRecv");
            }
#endif
        }

        int IComObject.OnSend(long socket, [In] byte[] buff, int len, int flags)
        {
#if TIMEDEBUG
            watch.Reset();
            watch.Start();
#endif

            try {
                return CommunicationManager.OnSend(socket, buff, len, flags);
            }
            catch (Exception e) {
                Exit(e);
                return -1;
            }

#if TIMEDEBUG
            finally
            {
                watch.Stop();
                Debug.WriteLine("\t" + watch.Elapsed.TotalMilliseconds, "IComObject.OnSend");
            }
#endif
        }


        void IComObject.SendPendingData()
        {
            try {
                CommunicationManager.SendData();
            }
            catch (Exception e) {
                Exit(e);
            }
        }


        void IComObject.ReceiveData(long socket)
        {
#if TIMEDEBUG
            watch.Reset();
            watch.Start();
#endif

            try {
                CommunicationManager.ReceiveData(socket);
            }
            catch (Exception e) {
                Exit(e);
            }

#if TIMEDEBUG
            finally
            {
                watch.Stop();
                Debug.WriteLine(watch.Elapsed.TotalMilliseconds, "IComObject.ReceiveData");
            }
#endif
        }


        bool IComObject.DataAvailable(long socket)
        {
            try {
                return CommunicationManager.DataAvailable(socket);
            }
            catch (Exception e) {
                Exit(e);
                return false;
            }
        }

        long IComObject.ManagedSocket()
        {
            try {
                return CommunicationManager.ManagedSocket();
            }
            catch (Exception e) {
                Exit(e);
                return 0;
            }
        }

        void IComObject.OnWindowCreated(IntPtr hwnd)
        {
            try {
                Core.OnWindowCreated(hwnd);
            }
            catch (Exception e) {
                Exit(e);
            }
        }

        void IComObject.OnFocusChanged(bool focused)
        {
            try {
                Core.OnFocusChanged(focused);
            }
            catch (Exception e) {
                Exit(e);
            }
        }

        bool IComObject.OnKeyDown(uint keyCode, uint repCount, bool prevState)
        {
            try {
                return Core.OnKeyDown((Keys)keyCode, repCount, prevState);
            }
            catch (Exception e) {
                Exit(e);
                return false;
            }
        }

        void IComObject.OnTextChanged(string text)
        {
            try {
                Client.OnTextChanged(text);
            }
            catch (Exception e) {
                Exit(e);
            }
        }

        bool IComObject.HasBeenCharListSent()
        {
            try {
                return Core.HasBeenCharListSent();
            }
            catch (Exception e) {
                Exit(e);
                return true;
            }
        }

        void IComObject.OnExitProcess(int exitCode)
        {
            try {
                Core.OnExitProcess(exitCode);
            }
            catch (Exception e) {
                Exit(e);
            }
        }

        void IComObject.WriteToLog(string message)
        {
            Trace.WriteLine(message, "Native");
        }

        #region IComObject Members


        bool IComObject.OnAppCommand(ushort keyCode)
        {
            return Core.OnAppCommand(keyCode);
        }

        bool IComObject.OnMouseWheel(short delta)
        {
            return Core.OnMouseWheel(delta);
        }

        #endregion
    }
}
