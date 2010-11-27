using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Runtime;

namespace Phoenix
{
    public class PublicEvent<THandler, TEventArgs> : IAssemblyObjectList
        where TEventArgs : EventArgs
    {
        private class EventObject : IAssemblyObject
        {
            private Delegate handler;

            public EventObject(object eventHandler)
            {
                handler = (Delegate)eventHandler;
            }

            public Delegate Handler
            {
                get { return handler; }
            }

            System.Reflection.Assembly IAssemblyObject.Assembly
            {
                get { return handler.Method.DeclaringType.Assembly; }
            }

            public override int GetHashCode()
            {
                return handler.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj == null || obj.GetType() != GetType())
                    return false;
                return handler.Equals(((EventObject)obj).handler);
            }
        }

        private Delegate handlers = null;

        public PublicEvent()
        {
            if (!typeof(THandler).IsSubclassOf(typeof(Delegate)))
                throw new InvalidOperationException("THandler must be Delegate.");
        }

        public bool IsEmpty
        {
            get { return handlers == null; }
        }

        public Delegate Handlers
        {
            get { return handlers; }
        }

        public void Invoke(object sender, TEventArgs e)
        {
            SyncEvent.Invoke(handlers, sender, e);
        }

        public void BeginInvoke(object sender, TEventArgs e)
        {
            SyncEvent.BeginInvoke(handlers, sender, e);
        }

        public void InvokeAsync(object sender, TEventArgs e)
        {
            SyncEvent.InvokeAsync(handlers, sender, e);
        }

        public void AddHandler(THandler handler)
        {
            RuntimeCore.AddAssemblyObject(new EventObject(handler), this);
            handlers = Delegate.Combine(handlers, (Delegate)(object)handler);
        }

        public void RemoveHandler(THandler handler)
        {
            handlers = Delegate.Remove(handlers, (Delegate)(object)handler);
            RuntimeCore.RemoveAssemblyObject(new EventObject(handler));
        }

        void IAssemblyObjectList.Remove(IAssemblyObject obj)
        {
            EventObject eventObject = obj as EventObject;
            if (eventObject != null) {
                handlers = Delegate.Remove(handlers, eventObject.Handler);
            }
        }
    }

    /// <summary>
    /// Equal to PublicEvent with EventHandler and EventArgs.
    /// </summary>
    public class DefaultPublicEvent : PublicEvent<EventHandler, EventArgs>
    {
    }
}
