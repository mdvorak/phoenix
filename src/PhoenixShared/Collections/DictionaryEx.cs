using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Security.Permissions;

namespace Phoenix.Collections
{
    #region DictionaryItemChangeEventArgs<TKey, TValue> class

    public class DictionaryItemChangeEventArgs<TKey, TValue> : EventArgs
    {
        private TKey key;
        private TValue item;

        public DictionaryItemChangeEventArgs(TKey key, TValue item)
        {
            this.key = key;
            this.item = item;
        }

        /// <summary>
        /// Changed item key.
        /// </summary>
        public TKey Key
        {
            get { return key; }
        }

        /// <summary>
        /// Changed item.
        /// </summary>
        public TValue Item
        {
            get { return item; }
        }
    }

    public delegate void DictionaryItemChangeEventHandler<TKey, TValue>(object sender, DictionaryItemChangeEventArgs<TKey, TValue> e);

    #endregion

    #region DictionaryItemUpdateEventArgs<TKey, TValue> class

    [Serializable]
    public class DictionaryItemUpdateEventArgs<TKey, TValue> : DictionaryItemChangeEventArgs<TKey, TValue>
    {
        private TValue oldItem;

        public DictionaryItemUpdateEventArgs(TKey key, TValue oldItem, TValue newItem)
            : base(key, newItem)
        {
            this.oldItem = oldItem;
        }

        /// <summary>
        /// Old item.
        /// </summary>
        public TValue OldItem
        {
            get { return oldItem; }
        }
    }

    public delegate void DictionaryItemUpdateEventHandler<TKey, TValue>(object sender, DictionaryItemUpdateEventArgs<TKey, TValue> e);

    #endregion

    [Serializable]
    public class DictionaryEx<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly object syncRoot = new object();
        private IDictionary<TKey, TValue> internalDictionary;

        public event DictionaryItemChangeEventHandler<TKey, TValue> ItemAdding;
        public event DictionaryItemChangeEventHandler<TKey, TValue> ItemAdded;
        public event DictionaryItemUpdateEventHandler<TKey, TValue> ItemUpdating;
        public event DictionaryItemUpdateEventHandler<TKey, TValue> ItemUpdated;
        public event DictionaryItemChangeEventHandler<TKey, TValue> ItemRemoving;
        public event DictionaryItemChangeEventHandler<TKey, TValue> ItemRemoved;
        public event EventHandler DictionaryClearing;
        public event EventHandler DictionaryCleared;

        public DictionaryEx()
        {
            internalDictionary = new Dictionary<TKey, TValue>();
        }

        protected DictionaryEx(IDictionary<TKey, TValue> internalDictionary)
        {
            if (internalDictionary == null)
                throw new ArgumentNullException("internalDictionary");

            this.internalDictionary = internalDictionary;
        }

        protected IDictionary<TKey, TValue> InternalDictionary
        {
            get { return internalDictionary; }
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

        #region Event callers

        [Synchronize(false)]
        protected virtual void OnItemAdding(DictionaryItemChangeEventArgs<TKey, TValue> e)
        {
            SyncEvent.Invoke(ItemAdding, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnItemAdded(DictionaryItemChangeEventArgs<TKey, TValue> e)
        {
            SyncEvent.Invoke(ItemAdded, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnItemUpdating(DictionaryItemUpdateEventArgs<TKey, TValue> e)
        {
            SyncEvent.Invoke(ItemUpdating, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnItemUpdated(DictionaryItemUpdateEventArgs<TKey, TValue> e)
        {
            SyncEvent.Invoke(ItemUpdated, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnItemRemoving(DictionaryItemChangeEventArgs<TKey, TValue> e)
        {
            SyncEvent.Invoke(ItemRemoving, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnItemRemoved(DictionaryItemChangeEventArgs<TKey, TValue> e)
        {
            SyncEvent.Invoke(ItemRemoved, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnDictionaryClearing(EventArgs e)
        {
            SyncEvent.Invoke(DictionaryClearing, this, e);
        }

        [Synchronize(false)]
        protected virtual void OnDictionaryCleared(EventArgs e)
        {
            SyncEvent.Invoke(DictionaryCleared, this, e);
        }

        #endregion

        public virtual void Add(TKey key, TValue value)
        {
            DictionaryItemChangeEventArgs<TKey, TValue> e = new DictionaryItemChangeEventArgs<TKey, TValue>(key, value);
            OnItemAdding(e);
            internalDictionary.Add(key, value);
            OnItemAdded(e);
        }

        public virtual bool ContainsKey(TKey key)
        {
            return internalDictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return internalDictionary.Keys; }
        }

        public virtual bool Remove(TKey key)
        {
            if (internalDictionary.ContainsKey(key)) {
                DictionaryItemChangeEventArgs<TKey, TValue> e = new DictionaryItemChangeEventArgs<TKey, TValue>(key, internalDictionary[key]);
                OnItemRemoving(e);
#if DEBUG
                Debug.Assert(internalDictionary.Remove(key));
#else
                internalDictionary.Remove(key);
#endif
                OnItemRemoved(e);
                return true;
            }

            return false;
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            return internalDictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return internalDictionary.Values; }
        }

        public virtual TValue this[TKey key]
        {
            get { return internalDictionary[key]; }
            set
            {
                TValue oldVal;
                if (internalDictionary.TryGetValue(key, out oldVal)) {
                    DictionaryItemUpdateEventArgs<TKey, TValue> e = new DictionaryItemUpdateEventArgs<TKey, TValue>(key, oldVal, value);
                    OnItemUpdating(e);
                    internalDictionary[key] = value;
                    OnItemUpdated(e);
                }
                else {
                    Add(key, value);
                }
            }
        }

        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            DictionaryItemChangeEventArgs<TKey, TValue> e = new DictionaryItemChangeEventArgs<TKey, TValue>(item.Key, item.Value);
            OnItemAdding(e);
            internalDictionary.Add(item);
            OnItemAdded(e);
        }

        public virtual void Clear()
        {
            OnDictionaryClearing(EventArgs.Empty);
            internalDictionary.Clear();
            OnDictionaryCleared(EventArgs.Empty);
        }

        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return internalDictionary.Contains(item);
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            internalDictionary.CopyTo(array, arrayIndex);
        }

        public virtual int Count
        {
            get { return internalDictionary.Count; }
        }

        [Synchronize(false)]
        public bool IsReadOnly
        {
            get { return internalDictionary.IsReadOnly; }
        }

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (internalDictionary.Contains(item)) {
                DictionaryItemChangeEventArgs<TKey, TValue> e = new DictionaryItemChangeEventArgs<TKey, TValue>(item.Key, item.Value);
                OnItemRemoving(e);
#if DEBUG
                Debug.Assert(internalDictionary.Remove(item));
#else
                internalDictionary.Remove(item);
#endif
                OnItemRemoved(e);
                return true;
            }

            return false;
        }

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates new empty synchronized dictionary of same type.
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// This sample shows how to create synchronized instances.
        /// <code>
        /// public class StringDictionaryEx : DictionaryEx&lt;string, string&gt;
        /// {
        ///     public StringDictionaryEx()
        ///     {
        ///     }
        /// }
        /// 
        /// public class TestClass
        /// {
        ///     private DictionaryEx&lt;int, int&gt; synchronizedDictionary = new DictionaryEx&lt;int, int&gt;.CreateSynchronized();
        /// 
        ///     private StringDictionaryEx synchronizedStringDictionary = (StringDictionaryEx)new StringDictionaryEx().CreateSynchronized();
        /// }
        /// </code>
        /// </example>
        public DictionaryEx<TKey, TValue> CreateSynchronized()
        {
            Type synchronizedType = SynchronizedTypeBuilder.Get(GetType());
            return (DictionaryEx<TKey, TValue>)Activator.CreateInstance(synchronizedType);
        }
    }
}
