using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Collections
{
    [Serializable]
    public class QueueEx<T> : ListEx<T>
    {
        public QueueEx()
        {
        }

        protected QueueEx(IList<T> internalList)
            : base(internalList)
        {
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the queue.
        /// </summary>
        /// <returns>The object that is removed from the beginning of the queue.</returns>
        /// <exception cref="System.InvalidOperationException">The queue is empty.</exception>
        public virtual T Dequeue()
        {
            if (Items.Count < 1)
                throw new InvalidOperationException("The queue is empty.");

            T item = Items[0];
            Items.RemoveAt(0);
            return item;
        }

        /// <summary>
        /// Adds an object to the end of the queue.
        /// </summary>
        /// <param name="item">The object to add to the queue. The value can be null for reference types.</param>
        public virtual void Enqueue(T item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Returns the object at the beginning of the queue without removing it.
        /// </summary>
        /// <returns>The object at the beginning of the queue.</returns>
        /// <exception cref="System.InvalidOperationException">The queue is empty.</exception>
        public virtual T Peek()
        {
            if (Items.Count < 1)
                throw new InvalidOperationException("The queue is empty.");

            return Items[0];
        }

        /// <summary>
        /// Creates new empty queue of same type.
        /// </summary>
        /// <returns></returns>
        public new QueueEx<T> CreateSynchronized()
        {
            return (QueueEx<T>)base.CreateSynchronized();
        }
    }
}
