using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Phoenix.Runtime;

namespace Phoenix
{
    public enum CallbackPriority
    {
        Highest,
        High,
        Normal,
        Low,
        Lowest
    }

    public enum CallbackResult
    {
        /// <summary>
        /// This message could be normally passed to client/server.
        /// </summary>
        Normal,
        /// <summary>
        /// This message will be dropped, after all handlers will process it.
        /// </summary>
        Eat,
        /// <summary>
        /// Custom data has been sent. Don't send anything else to avoid message duplicating.
        /// </summary>
        Sent
    }

    /// <summary>
    /// Delegate for server and client messages callbacks. Message processing should take as least time as possible.
    /// </summary>
    /// <param name="data">Raw message data.</param>
    /// <param name="prevState">Highest state returned by previous handlers.
    /// <para>This value should be always checked before sending any data to avoid packet duplicating.</para>
    /// </param>
    /// <returns>This value will be passed to following handlers. <see cref="Phoenix.CallbackResult"/> for more informations.</returns>
    public delegate CallbackResult MessageCallback(byte[] data, CallbackResult prevState);

    internal class MessageCallbacksCollection : IAssemblyObjectList
    {
        private class CallbackObject : IAssemblyObject
        {
            public readonly byte MessageId;
            public readonly MessageCallback Callback;
            public readonly CallbackPriority Priority;
            private readonly Assembly assembly;

            public CallbackObject(byte messageId, MessageCallback callback, CallbackPriority priority)
            {
                MessageId = messageId;
                Callback = callback;
                Priority = priority;
                assembly = callback.Method.Module.Assembly;
            }

            Assembly IAssemblyObject.Assembly
            {
                get { return assembly; }
            }

            public override int GetHashCode()
            {
                return MessageId;
            }

            public override bool Equals(object obj)
            {
                CallbackObject callback = obj as CallbackObject;
                if (callback != null) {
                    return callback.MessageId == MessageId && callback.Callback == Callback && callback.assembly == assembly;
                }
                return false;
            }
        }

        private class CallbacksCollection : List<CallbackObject>
        {
            public void SortByPriority()
            {
                Sort(new Comparison<CallbackObject>(Compare));
            }

            private static int Compare(CallbackObject o1, CallbackObject o2)
            {
                return o1.Priority - o2.Priority;
            }
        }

        private readonly object syncRoot;
        private CallbacksCollection[] list;
        private string callbacksName;

        public MessageCallbacksCollection(string callbacksName)
        {
            syncRoot = new object();
            list = new CallbacksCollection[256];
            this.callbacksName = callbacksName;
        }

        public string CallbacksName
        {
            get { return callbacksName; }
        }

        public void Add(byte id, MessageCallback callback, CallbackPriority priority)
        {
            lock (syncRoot) {
                if (list[id] == null) list[id] = new CallbacksCollection();

                CallbackObject obj = new CallbackObject(id, callback, priority);

                if (list[id].Contains(obj))
                    throw new ArgumentException("Callback is already registered for this message.");

                RuntimeCore.AddAssemblyObject(obj, this);
                list[id].Add(obj);

                list[id].SortByPriority();

                Debug.WriteLine(callbacksName + " callback added (0x" + id.ToString("X2") + ", " + priority.ToString() + ")", "Phoenix");
            }
        }

        public void Remove(byte id, MessageCallback callback)
        {
            lock (syncRoot) {
                if (list[id] != null) {
                    foreach (CallbackObject obj in list[id]) {
                        if (obj.Callback == callback) {
                            list[id].Remove(obj);
                            RuntimeCore.RemoveAssemblyObject(obj);

                            Debug.WriteLine(callbacksName + " callback removed (0x" + id.ToString("X2") + ")", "Phoenix");
                            return;
                        }
                    }
                }
            }
        }

        public CallbackResult ProcessMessage(byte[] data, CallbackResult prevResult)
        {
            CallbackObject[] cl;

            lock (syncRoot) {
                if (data == null || data.Length == 0)
                    return CallbackResult.Eat;

                byte id = data[0];

                cl = list[id] != null ? list[id].ToArray() : null;
            }

            // Mimo synchronizacni kontext
            if (cl != null) {
                byte[] local = new byte[data.Length];
                Array.Copy(data, local, data.Length);

                for (int i = 0; i < cl.Length; i++) {
                    try {
                        CallbackResult r = cl[i].Callback(data, prevResult);

                        if (r > prevResult) prevResult = r;
                    }
                    catch (Exception e) {
                        Trace.WriteLine(String.Format("Unhandled error in {0}. Exception:\r\n{1}", callbacksName, e), "Phoenix");
                    }
                }
            }

            return prevResult;
        }

        void IAssemblyObjectList.Remove(IAssemblyObject obj)
        {
            lock (syncRoot) {
                CallbackObject callbackObj = obj as CallbackObject;

                if (callbackObj != null && list[callbackObj.MessageId] != null) {
                    list[callbackObj.MessageId].Remove(callbackObj);

                    Debug.WriteLine(callbacksName + " callback removed (0x" + callbackObj.MessageId.ToString("X2") + ")", "Phoenix");
                }
            }
        }
    }
}
