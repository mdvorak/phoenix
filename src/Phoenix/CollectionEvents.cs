using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    public class ItemAddedEventArgs : EventArgs
    {
        private object item;

        public ItemAddedEventArgs(object item)
        {
            this.item = item;
        }

        public object Item
        {
            get { return item; }
        }
    }

    public class ItemRemovedEventArgs : EventArgs
    {
        private object item;

        public ItemRemovedEventArgs(object item)
        {
            this.item = item;
        }

        public object Item
        {
            get { return item; }
        }
    }

    public delegate void ItemAddedEventHandler(object sender, ItemAddedEventArgs e);
    public delegate void ItemRemovedEventHandler(object sender, ItemRemovedEventArgs e);
}
