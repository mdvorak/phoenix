using System;
using System.Collections;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Phoenix.Gui
{
    public class GuiThread
    {
        #region Native functions

        [StructLayout(LayoutKind.Sequential)]
        struct MSG
        {
            IntPtr hwnd;
            uint message;
            IntPtr wParam;
            IntPtr lParam;
            uint time;
            int ptX;
            int ptY;
        }

        const int PM_NOREMOVE = 0;
        const int PM_REMOVE = 1;


        [DllImport("user32.dll")]
        static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, int wRemoveMsg);

        [DllImport("user32.dll")]
        static extern bool TranslateMessage(ref MSG lpmsg);

        [DllImport("user32.dll")]
        static extern IntPtr DispatchMessage(ref MSG lpmsg);

        #endregion

        private Thread thread;
        private ManualResetEvent windowCreatedEvent;
        private Form window;

        public GuiThread(Type formType)
        {
            if (!formType.IsSubclassOf(typeof(Form)) && formType != typeof(Form))
                throw new ArgumentException("formType must be inherited from System.Windows.Forms.Form.");

            thread = new Thread(new ParameterizedThreadStart(MessagePump));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Name = "GuiThread";
            windowCreatedEvent = new ManualResetEvent(false);

            thread.Start(formType);
            windowCreatedEvent.WaitOne();
            windowCreatedEvent.Close();
            windowCreatedEvent = null;
        }

        public Form Window
        {
            get { return window; }
        }

        protected Thread Thread
        {
            get { return thread; }
        }

        public bool InvokeRequired
        {
            get { return Thread.CurrentThread != thread; }
        }

        private delegate object CreateObjectDelegate(Type type);
        /// <summary>
        /// Creates object on gui thread.
        /// </summary>
        /// <param name="type">Object type.</param>
        public object CreateObject(Type type)
        {
            if (InvokeRequired) {
                return Invoke(new CreateObjectDelegate(CreateObject), type);
            }
            else {
                return type.Assembly.CreateInstance(type.FullName);
            }
        }

        protected virtual void OnThreadStarted()
        {
        }

        public object Invoke(Delegate method, params object[] args)
        {
            if (window.InvokeRequired) {
                return window.Invoke(method, args);
            }
            else {
                return method.Method.Invoke(method.Target, args);
            }
        }

        /// <summary>
        /// If InvokeRequired is true, Delegate is executed via BeginInvoke; otherwise is executed directly.
        /// </summary>
        public void InvokeFast(Delegate method, params object[] args)
        {
            if (window.InvokeRequired) {
                window.BeginInvoke(method, args);
            }
            else {
                method.Method.Invoke(method.Target, args);
            }
        }

        private void MessagePump(object arg)
        {
            try {
                OnThreadStarted();

                Type windowType = (Type)arg;

                window = (Form)windowType.Assembly.CreateInstance(windowType.FullName);

                MSG m;

                // Create message queue
                PeekMessage(out m, IntPtr.Zero, 0, 0, PM_NOREMOVE);

                // Create window and obtain its handle
                window.CreateControl();
                IntPtr handle = window.Handle;

                // Set created event
                windowCreatedEvent.Set();

                // Message pump
                while (GetMessage(out m, IntPtr.Zero, 0, 0)) {
                    TranslateMessage(ref m);
                    DispatchMessage(ref m);
                }
            }
            catch (Exception e) {
                FatalExceptionDialog dlg = new FatalExceptionDialog();
                dlg.Exception = e;
                dlg.TryToContinueEnabled = false;
                dlg.ShowDialog();
                dlg.Dispose();
                Environment.Exit(-1);
            }
            finally {
                // Dispose closed window
                if (window != null)
                    window.Dispose();
            }
        }
    }
}
