using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Phoenix.Collections
{
    #region ListItemChangeEventArgs<T> class

    public class ListItemChangeEventArgs<T> : EventArgs
    {
        private int index;
        private T item;

        public ListItemChangeEventArgs(int index, T item)
        {
            this.index = index;
            this.item = item;
        }

        /// <summary>
        /// Index of changed item;
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// Changed item.
        /// </summary>
        public T Item
        {
            get { return item; }
        }
    }

    public delegate void ListItemChangeEventHandler<T>(object sender, ListItemChangeEventArgs<T> e);

    #endregion

    #region ListItemUpdateEventArgs<T> class

    public class ListItemUpdateEventArgs<T> : ListItemChangeEventArgs<T>
    {
        private T oldItem;

        public ListItemUpdateEventArgs(int index, T oldItem, T newItem)
            : base(index, newItem)
        {
            this.oldItem = oldItem;
        }

        /// <summary>
        /// Old item.
        /// </summary>
        public T OldItem
        {
            get { return oldItem; }
        }
    }

    public delegate void ListItemUpdateEventHandler<T>(object sender, ListItemUpdateEventArgs<T> e);

    #endregion

    [Serializable]
    public class ListEx<T> : Collection<T>
    {
        private readonly object syncRoot = new object();
        public event ListItemChangeEventHandler<T> ItemInserting;
        public event ListItemChangeEventHandler<T> ItemInserted;
        public event ListItemUpdateEventHandler<T> ItemUpdating;
        public event ListItemUpdateEventHandler<T> ItemUpdated;
        public event ListItemChangeEventHandler<T> ItemRemoving;
        public event ListItemChangeEventHandler<T> ItemRemoved;
        public event EventHandler ListClearing;
        public event EventHandler ListCleared;

        public ListEx()
        {
        }

        protected ListEx(IList<T> internalList)
            : base(internalList)
        {
        }

        /// <summary>
        /// Gets synchronization object.
        /// </summary>
        public object SyncRoot
        {
            get { return syncRoot; }
        }

        [Synchronize(false)]
        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        [Synchronize(false)]
        protected virtual void OnItemInserting(ListItemChangeEventArgs<T> e)
        {
            SyncEvent.Invoke(ItemInserting, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnItemInserted(ListItemChangeEventArgs<T> e)
        {
            SyncEvent.Invoke(ItemInserted, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnItemUpdating(ListItemUpdateEventArgs<T> e)
        {
            SyncEvent.Invoke(ItemUpdating, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnItemUpdated(ListItemUpdateEventArgs<T> e)
        {
            SyncEvent.Invoke(ItemUpdated, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnItemRemoving(ListItemChangeEventArgs<T> e)
        {
            SyncEvent.Invoke(ItemRemoving, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnItemRemoved(ListItemChangeEventArgs<T> e)
        {
            SyncEvent.Invoke(ItemRemoved, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnListClearing(EventArgs e)
        {
            SyncEvent.Invoke(ListClearing, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnListCleared(EventArgs e)
        {
            SyncEvent.Invoke(ListCleared, this, e);
        }

        protected override void InsertItem(int index, T item)
        {
            ListItemChangeEventArgs<T> e = new ListItemChangeEventArgs<T>(index, item);
            OnItemInserting(e);
            base.InsertItem(index, item);
            OnItemInserted(e);
        }

        protected override void SetItem(int index, T item)
        {
            ListItemUpdateEventArgs<T> e = new ListItemUpdateEventArgs<T>(index, Items[index], item);
            OnItemUpdating(e);
            base.SetItem(index, item);
            OnItemUpdated(e);
        }

        protected override void RemoveItem(int index)
        {
            if (index >= 0 && index < Items.Count) {
                ListItemChangeEventArgs<T> e = new ListItemChangeEventArgs<T>(index, Items[index]);
                OnItemRemoving(e);
                base.RemoveItem(index);
                OnItemRemoved(e);
            }
        }

        protected override void ClearItems()
        {
            OnListClearing(EventArgs.Empty);
            base.ClearItems();
            OnListCleared(EventArgs.Empty);
        }

        public virtual void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (T item in collection) {
                Add(item);
            }
        }

        public virtual T[] ToArray()
        {
            T[] array = new T[Items.Count];
            for (int i = 0; i < Items.Count; i++) {
                array[i] = Items[i];
            }
            return array;
        }

        /// <summary>
        /// Creates new empty synchronized collection of same type.
        /// </summary>
        /// <returns></returns>
        public ListEx<T> CreateSynchronized()
        {
            Type synchronizedType = SynchronizedTypeBuilder.Get(GetType());
            return (ListEx<T>)Activator.CreateInstance(synchronizedType);
        }
    }
}
