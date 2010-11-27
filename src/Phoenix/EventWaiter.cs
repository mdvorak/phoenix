using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Phoenix.Runtime;

namespace Phoenix
{
    /// <summary>
    /// This is helper class for waiting for events.
    /// Handler must be added to event handler list before Wait is called.
    /// Handler MUST be also REMOVED from event or it will hang in memory, and if object is disposed, exception will be thrown (IN HANDLER!!).
    /// </summary>
    /// <typeparam name="TEventArgs">Type inherited from EventArgs.</typeparam>
    public class EventWaiter<TEventArgs> : IDisposable
        where TEventArgs : EventArgs
    {
        public delegate bool TestEventArgsDelegate(object eventSender, TEventArgs eventArgs);

        private AutoResetEvent firedEvent;
        private EventHandler<TEventArgs> handler;
        private TestEventArgsDelegate eventTest;

        public EventWaiter()
        {
            firedEvent = new AutoResetEvent(false);
            handler = new EventHandler<TEventArgs>(EventHandler);
        }

        public EventWaiter(TestEventArgsDelegate testEventArgsProc)
        {
            firedEvent = new AutoResetEvent(false);
            handler = new EventHandler<TEventArgs>(EventHandler);
            eventTest = testEventArgsProc;
        }

        public EventHandler<TEventArgs> Handler
        {
            get { return handler; }
        }

        public bool Disposed
        {
            get { return firedEvent == null; }
        }

        private void EventHandler(object sender, TEventArgs e)
        {
            if (Disposed)
                throw new InvalidOperationException("Object has been disposed.");

            if (OnEventArgsTest(sender, e))
            {
                firedEvent.Set();
            }
        }

        /// <summary>
        /// Return true if event should be set; otherwise false.
        /// </summary>
        /// <param name="eventSender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        protected virtual bool OnEventArgsTest(object eventSender, TEventArgs eventArgs)
        {
            return eventTest == null || eventTest(eventSender, eventArgs);
        }

        /// <summary>
        /// Waits until event or timeout occurs. Returns true if event has occured.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds. Use -1 to infinite.</param>
        /// <returns>True if event occured; otherwise false.</returns>
        public bool Wait(int timeout)
        {
            if (Disposed)
                throw new InvalidOperationException("Object has been disposed.");

            return firedEvent.WaitOne(timeout, false);
        }

        public virtual void Dispose()
        {
            if (firedEvent != null)
                firedEvent.Close();

            firedEvent = null;
            handler = null;
        }
    }

    public class DefaultEventWaiter : EventWaiter<EventArgs>
    {
        public DefaultEventWaiter()
        {
        }

        public DefaultEventWaiter(TestEventArgsDelegate testEventArgsProc)
            : base(testEventArgsProc)
        {
        }
    }
}
