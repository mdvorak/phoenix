using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Phoenix.Runtime
{
    public static class ThreadStarter
    {
        class StartArguments
        {
            public StartArguments(Delegate methodDelegate, object[] parameters)
            {
                Delegate = methodDelegate;
                Parameters = parameters;
            }

            public Delegate Delegate;
            public object[] Parameters;
        }

        public static Thread Start(Delegate methodDelegate, params object[] parameters)
        {
            if (methodDelegate == null)
                throw new ArgumentNullException("methodDelegate");

            Thread t = new Thread(new ParameterizedThreadStart(Worker));
            t.Start(new StartArguments(methodDelegate, parameters));
            return t;
        }

        public static Thread StartBackround(Delegate methodDelegate, params object[] parameters)
        {
            if (methodDelegate == null)
                throw new ArgumentNullException("methodDelegate");

            Thread t = new Thread(new ParameterizedThreadStart(Worker));
            t.IsBackground = true;
            t.Start(new StartArguments(methodDelegate, parameters));
            return t;
        }

        public static void EnqueueInPool(Delegate methodDelegate, params object[] parameters)
        {
            if (methodDelegate == null)
                throw new ArgumentNullException("methodDelegate");

            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(Worker), new StartArguments(methodDelegate, parameters))) {
                StartBackround(methodDelegate, parameters);
            }
        }

        private static void Worker(object arg)
        {
            StartArguments startArgs = (StartArguments)arg;
            startArgs.Delegate.Method.Invoke(startArgs.Delegate.Target, startArgs.Parameters);
        }
    }
}
