using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Security.Permissions;
using Phoenix.Runtime.Reflection;

namespace Phoenix.Runtime
{
    public class ExecutionsChangedEventArgs : EventArgs
    {
        private ExecutionInfo executionInfo;

        public ExecutionsChangedEventArgs(ExecutionInfo ei)
        {
            executionInfo = ei;
        }

        public ExecutionInfo ExecutionInfo
        {
            get { return executionInfo; }
        }
    }

    public delegate void ExecutionsChangedEventHandler(object sender, ExecutionsChangedEventArgs e);
    /*
        [SecurityPermission(SecurityAction.Deny,
           BindingRedirects = true,
           ControlAppDomain = true,
           ControlDomainPolicy = true,
           ControlEvidence = true,
           ControlPolicy = true,
           ControlThread = true,
           SkipVerification = true,
           Infrastructure = true)]*/
    public sealed class Executions : IAssemblyObjectList
    {
        public const int MaxExecutions = 15;

        private readonly object syncRoot;
        private List<ExecutionInfo> runningExecutions;
        [ThreadStatic]
        private static bool threadInitialized;

        private PublicEvent<ExecutionsChangedEventHandler, ExecutionsChangedEventArgs> executionStarted;
        private PublicEvent<ExecutionsChangedEventHandler, ExecutionsChangedEventArgs> executionFinished;

        public event ExecutionsChangedEventHandler ExecutionStarted
        {
            add { executionStarted.AddHandler(value); }
            remove { executionStarted.RemoveHandler(value); }
        }

        public event ExecutionsChangedEventHandler ExecutionFinished
        {
            add { executionFinished.AddHandler(value); }
            remove { executionFinished.RemoveHandler(value); }
        }

        internal Executions()
        {
            syncRoot = new object();
            runningExecutions = new List<ExecutionInfo>(MaxExecutions);

            executionStarted = new PublicEvent<ExecutionsChangedEventHandler, ExecutionsChangedEventArgs>();
            executionFinished = new PublicEvent<ExecutionsChangedEventHandler, ExecutionsChangedEventArgs>();
        }

        private object RunInternal(MethodOverloads methodOverloads, object[] rawParams)
        {
            // TODO: Security
            FileIOPermission phoenixLauncherFile = new FileIOPermission(FileIOPermissionAccess.AllAccess, System.IO.Path.Combine(Core.Directory, "PhoenixLauncher.xml"));
            phoenixLauncherFile.Deny();

            // First create ParameterData array
            ParameterData[] parameters = new ParameterData[rawParams.Length];
            for (int i = 0; i < rawParams.Length; i++) {
                parameters[i] = new ParameterData(rawParams[i]);
            }

            // Get valid overloads (array is never empty)
            Method[] methods = methodOverloads.FindOverloads(parameters);

            Exception exception = null;

            foreach (Method m in methods) {
                ExecutionInfo info = new ExecutionInfo(m);

                ExecutionAttribute[] execAttributes = (ExecutionAttribute[])m.MethodInfo.GetCustomAttributes(typeof(ExecutionAttribute), false);

                try {
                    // Call all Execution attributes
                    for (int i = 0; i < execAttributes.Length; i++) {
                        execAttributes[i].Starting(m);
                    }

                    // Add execution to running list
                    lock (syncRoot) {
                        if (runningExecutions.Count >= Executions.MaxExecutions) {
                            throw new RuntimeException("Executions limit exceeded.");
                        }

                        RuntimeCore.AddAssemblyObject(info, this);
                        runningExecutions.Add(info);

                        try {
                            // Raise Started event
                            executionStarted.Invoke(this, new ExecutionsChangedEventArgs(info));
                        }
                        catch (Exception e) {
                            Core.ShowMessageBoxAsync("Unhandled exception in Executions.ExecutionStarted event handler.\r\nMessage: " + e.Message, "Warning");
                        }
                    }

                    // Init thread-dependent classes
                    if (!threadInitialized) {
                        ScriptErrorException.ThreadInit();
                        WorldData.World.ThreadInit();
                        Journal.ThreadInit();
                        threadInitialized = true;
                    }

                    // Invoke
                    try {
                        return m.Invoke(parameters);
                    }
                    catch (System.Reflection.TargetInvocationException e) {
                        // Im interested only in exception thrown by code
                        throw e.InnerException;
                    }
                }
                catch (ParameterException e) {
                    exception = e;
                }
                catch (ExecutionBlockedException e) {
                    exception = e;
                }
                finally {
                    // Remove execution from running list
                    lock (syncRoot) {
                        runningExecutions.Remove(info);
                        RuntimeCore.RemoveAssemblyObject(info);

                        try {
                            // Raise Finished event
                            executionFinished.Invoke(this, new ExecutionsChangedEventArgs(info));
                        }
                        catch (Exception e) {
                            Core.ShowMessageBoxAsync("Unhandled exception in Executions.ExecutionFinished event handler.\r\nMessage: " + e.Message, "Warning");
                        }
                    }

                    // Call all Execution attributes
                    for (int i = 0; i < execAttributes.Length; i++) {
                        execAttributes[i].Finished(m);
                    }
                }
            }

            if (exception != null) {
                throw exception;
            }
            else {
                throw new InternalErrorException();
            }
        }

        private delegate void ExecuteWorkerDelegate(MethodOverloads method, object[] rawParams);
        private void ExecuteWorker(MethodOverloads method, params object[] rawParams)
        {
            string threadId = Thread.CurrentThread.ManagedThreadId.ToString();

            try {
                Debug.WriteLine("Execution " + threadId + " started.", "Runtime");

                RunInternal(method, rawParams);
            }
            catch (ThreadAbortException) {
                UO.PrintInformation("Execution {0} terminated.", threadId);
            }
            catch (ScriptErrorException e) {
                Trace.WriteLine("Unhandled exception:\n" + e.ToString(), "Script");
                UO.PrintError(e.Message);
            }
            catch (ParameterException e) {
                Trace.WriteLine("Unhandled error during execution. Exception:\r\n" + e.ToString(), "Runtime");
                UO.PrintError("Runtime error: {0}", e.Message);

                string[] syntax = method.Syntax;

                if (syntax.Length > 0) {
                    UO.PrintInformation("usage:", method.Name);

                    for (int i = 0; i < syntax.Length; i++)
                        UO.PrintInformation("  " + syntax[i]);
                }
            }
            catch (Exception e) {
                Trace.WriteLine("Unhandled error during execution. Exception:\r\n" + e.ToString(), "Runtime");
                UO.PrintError("Runtime error: {0}", e.Message);
            }
            finally {
                Debug.WriteLine("Execution " + threadId + " finished.", "Runtime");
            }
        }

        #region IAssemblyObjectList Members

        void IAssemblyObjectList.Remove(IAssemblyObject obj)
        {
            ExecutionInfo info = obj as ExecutionInfo;

            if (info != null) {
                info.Thread.Abort();
            }
        }

        #endregion

        /// <summary>
        /// Executes method in current thread.
        /// </summary>
        /// <param name="method">Method or list of overloads to execute.</param>
        /// <param name="rawParams">Parameter list convertible to at least one method overload. Otherwise exception is thrown.</param>
        /// <returns>Return value of method or null if none.</returns>
        public object Run(MethodOverloads method, params object[] rawParams)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            return RunInternal(method, rawParams);
        }

        /// <summary>
        /// Executes method in thread. This method ends immediatly.
        /// </summary>
        /// <param name="method">Method or list of overloads to execute.</param>
        /// <param name="rawParams">Parameter list convertible to at least one method overload.</param>
        /// <returns>Thread where execution runs on.</returns>
        public Thread Execute(MethodOverloads method, params object[] rawParams)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            return ThreadStarter.StartBackround(new ExecuteWorkerDelegate(ExecuteWorker), method, rawParams);
        }

        /// <summary>
        /// Terminates running execution of specified id.
        /// </summary>
        /// <param name="id">Execution id.</param>
        public void Terminate(int id)
        {
            lock (syncRoot) {
                for (int i = 0; i < runningExecutions.Count; i++) {
                    if (runningExecutions[i].Id == id) {
                        runningExecutions[i].Thread.Abort();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Terminates all running executions.
        /// </summary>
        public void TerminateAll()
        {
            lock (syncRoot) {
                for (int i = 0; i < runningExecutions.Count; i++) {
                    runningExecutions[i].Thread.Abort();
                }
            }
        }
    }
}
