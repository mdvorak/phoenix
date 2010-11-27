using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    public class SpecializedObjectChangedEventWaiter : ObjectChangedEventWaiter
    {
        private ObjectChangeType changes;

        public SpecializedObjectChangedEventWaiter(Serial serial, ObjectChangeType changes)
            : base(serial)
        {
            this.changes = changes;
        }

        public SpecializedObjectChangedEventWaiter(Serial serial, ObjectChangeType changes, TestEventArgsDelegate eventTest)
            : base(serial, eventTest)
        {
            this.changes = changes;
        }

        public ObjectChangeType Changes
        {
            get { return changes; }
        }

        protected override bool OnEventArgsTest(object eventSender, ObjectChangedEventArgs eventArgs)
        {
            if ((eventArgs.Type & changes) != 0)
            {
                return base.OnEventArgsTest(eventArgs, eventArgs);
            }
            else return false;
        }
    }
}
