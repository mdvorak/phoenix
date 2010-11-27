using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    public class ItemUpdateEventWaiter : SpecializedObjectChangedEventWaiter
    {
        public ItemUpdateEventWaiter(Serial serial)
            : base(serial, ObjectChangeType.ItemUpdated)
        {
        }
    }

    public class ItemOpenedEventWaiter : SpecializedObjectChangedEventWaiter
    {
        public ItemOpenedEventWaiter(Serial serial)
            : base(serial, ObjectChangeType.ItemOpened)
        {
        }
    }

    public class ObjectRemovedEventWaiter : SpecializedObjectChangedEventWaiter
    {
        public ObjectRemovedEventWaiter(Serial serial)
            : base(serial, ObjectChangeType.Removed)
        {
        }
    }
}
