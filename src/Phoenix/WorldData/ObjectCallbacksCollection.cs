using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Phoenix.WorldData
{
    internal sealed class ObjectCallbacksCollection
    {
        class Event
        {
            public ObjectChangedEventArgs EventArgs;
            public ObjectChangedPublicEvent Handler;
        }

        private readonly object syncRoot = new object();
        private Dictionary<uint, ObjectChangedPublicEvent> list = new Dictionary<uint, ObjectChangedPublicEvent>();

        private Thread workerThread;
        private Queue<Event> eventQueue = new Queue<Event>();
        private AutoResetEvent itemQueuedEvent = new AutoResetEvent(false);

        public ObjectCallbacksCollection()
        {
            workerThread = new Thread(new ThreadStart(AsyncInvokeWorker));
            workerThread.IsBackground = true;
            workerThread.Start();
        }

        ~ObjectCallbacksCollection()
        {
            workerThread.Abort();
        }

        public void Add(uint serial, ObjectChangedEventHandler handler)
        {
            lock (syncRoot)
            {
                ObjectChangedPublicEvent callbacks;
                if (!list.TryGetValue(serial, out callbacks))
                {
                    callbacks = new ObjectChangedPublicEvent();
                    list.Add(serial, callbacks);
                }

                callbacks.AddHandler(handler);
            }
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                list.Clear();
            }
        }

        public void Remove(uint serial, ObjectChangedEventHandler handler)
        {
            lock (syncRoot)
            {
                ObjectChangedPublicEvent callbacks;
                if (list.TryGetValue(serial, out callbacks))
                {
                    callbacks.RemoveHandler(handler);

                    if (callbacks.IsEmpty)
                    {
                        list.Remove(serial);
                    }
                }
            }
        }

        public void Invoke(ObjectChangedEventArgs e)
        {
            lock (syncRoot)
            {
                ObjectChangedPublicEvent callbacks;
                if (list.TryGetValue(e.Serial, out callbacks))
                {
                    callbacks.Invoke(null, e);
                }
            }
        }

        public void InvokeAsync(ObjectChangedEventArgs e)
        {
            lock (syncRoot)
            {
                ObjectChangedPublicEvent callbacks;
                if (list.TryGetValue(e.Serial, out callbacks))
                {
                    Event args = new Event();
                    args.Handler = callbacks;
                    args.EventArgs = e;

                    eventQueue.Enqueue(args);
                    itemQueuedEvent.Set();
                }
            }
        }

        private void AsyncInvokeWorker()
        {
            while (true)
            {
                itemQueuedEvent.WaitOne();

                try
                {
                    while (eventQueue.Count > 0)
                    {
                        Event e = null;
                        lock (syncRoot)
                        {
                            Debug.Assert(eventQueue.Peek() != null, "null in eventQueue.");
                            e = eventQueue.Dequeue();
                        }

                        if (e != null)
                        {
                            e.Handler.Invoke(null, e.EventArgs);
                        }
                    }
                }
                catch { }
            }
        }
    }
}
