using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Phoenix.WorldData
{
    public class ObjectChangedEventWaiter : EventWaiter<ObjectChangedEventArgs>
    {
        private Serial serial;

        public ObjectChangedEventWaiter(Serial serial)
        {
            this.serial = serial;
            World.AddObjectChangedCallback(serial, new ObjectChangedEventHandler(Handler));
        }

        public ObjectChangedEventWaiter(Serial serial, TestEventArgsDelegate eventTest)
            : base(eventTest)
        {
            this.serial = serial;
            World.AddObjectChangedCallback(serial, new ObjectChangedEventHandler(Handler));
        }

        protected override bool OnEventArgsTest(object eventSender, ObjectChangedEventArgs eventArgs)
        {
            Debug.Assert(serial == eventArgs.Serial, "serial != eventArgs.Serial; Internal error?");
            return base.OnEventArgsTest(eventSender, eventArgs);
        }

        public override void Dispose()
        {
            World.RemoveObjectChangedCallback(serial, new ObjectChangedEventHandler(Handler));

            base.Dispose();
        }
    }
}
