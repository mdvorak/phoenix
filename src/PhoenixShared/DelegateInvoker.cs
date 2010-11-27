using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using Phoenix.Gui;

namespace Phoenix
{
    public static class DelegateInvoker
    {
        struct AsyncInvokeArgs
        {
            public MethodInfo Method;
            public Object Target;
            public object[] Params;
            public List<Delegate> List;

            public AsyncInvokeArgs(MethodInfo m, object t, object[] p)
            {
                Method = m;
                Target = t;
                Params = p;
                List = null;
            }

            public AsyncInvokeArgs(List<Delegate> list, object[] p)
            {
                Method = null;
                Target = null;
                Params = p;
                List = list;
            }
        }

        public static void Invoke(Delegate handler, params object[] parameters)
        {
            if (handler != null) {
                foreach (Delegate d in handler.GetInvocationList()) {
                    ISynchronizeInvoke sync = d.Target as ISynchronizeInvoke;

                    if (sync != null && sync.InvokeRequired) {
                        // I need to call this asynchrounously because it caused deadlocks
                        sync.BeginInvoke(d, parameters);
                    }
                    else {
                        d.Method.Invoke(d.Target, parameters);
                    }
                }
            }
        }

        /// <summary>
        /// Invokes all nonsynchronized handlers on new thread (all on one).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="parameters"></param>
        public static void BeginInvoke(Delegate handler, params object[] parameters)
        {
            if (handler != null) {
                List<Delegate> list = new List<Delegate>();

                foreach (Delegate d in handler.GetInvocationList()) {
                    ISynchronizeInvoke sync = d.Target as ISynchronizeInvoke;
                    if (sync != null) {
                        sync.BeginInvoke(d, parameters);
                    }
                    else {
                        list.Add(d);
                    }
                }

                if (list.Count > 0) {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInvokeListProc), new AsyncInvokeArgs(list, parameters));
                }
            }
        }

        /// <summary>
        /// Invokes every delegate method in new thread (except synchronized handlers).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="parameters"></param>
        public static void InvokeAsync(Delegate handler, params object[] parameters)
        {
            if (handler != null) {
                foreach (Delegate d in handler.GetInvocationList()) {
                    ISynchronizeInvoke sync = d.Target as ISynchronizeInvoke;
                    if (sync != null) {
                        sync.BeginInvoke(d, parameters);
                    }
                    else {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncInvokeProc), new AsyncInvokeArgs(d.Method, d.Target, parameters));
                    }
                }
            }
        }

        private static void AsyncInvokeListProc(object arg)
        {
            try {
                AsyncInvokeArgs aia = (AsyncInvokeArgs)arg;
                Debug.Assert(aia.List != null, "aia.List != null");

                foreach (Delegate d in aia.List) {
                    d.Method.Invoke(d.Target, aia.Params);
                }
            }
            catch (Exception e) {
                if (e is TargetInvocationException && e.InnerException != null)
                    e = e.InnerException;

                Trace.WriteLine("Unhandled exception during async invoke. Details:\n" + e.ToString(), "Phoenix");
                ExceptionDialog.Show(e, "Unhandled exception during async invoke.");
            }
        }

        private static void AsyncInvokeProc(object arg)
        {
            try {
                AsyncInvokeArgs aia = (AsyncInvokeArgs)arg;
                Debug.Assert(aia.Method != null, "aia.Method != null");
                aia.Method.Invoke(aia.Target, aia.Params);
            }
            catch (Exception e) {
                if (e is TargetInvocationException && e.InnerException != null)
                    e = e.InnerException;

                Trace.WriteLine("Unhandled exception during async invoke. Details:\n" + e.ToString(), "Phoenix");
                ExceptionDialog.Show(e, "Unhandled exception during async invoke.");
            }
        }
    }
}
