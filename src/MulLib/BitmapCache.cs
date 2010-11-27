using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;

namespace MulLib
{
    // PREPSAT!! Komplet
    sealed class BitmapCache
    {
        private class DataObject
        {
            public readonly Bitmap Bitmap;
            public readonly int Size;
            public DateTime LastAccess;

            public DataObject(Bitmap bitmap)
            {
                this.Bitmap = bitmap;
                Size = bitmap.Width * bitmap.Height;
                LastAccess = DateTime.Now;
            }
        }

        private int cacheSize = 4096 * 1024;
        private Hashtable list = Hashtable.Synchronized(new Hashtable());
        private int dataSize = 0;

        private Timer cleanTimer;
        private bool timerEnabled = false;

        /// <summary>
        /// Initializes new empty BitmapCache.
        /// </summary>
        public BitmapCache()
        {
            cleanTimer = new Timer(new TimerCallback(TimerCallback), null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Initializes new empty BitmapCache.
        /// </summary>
        /// <param name="size">Maximum size of cache in pixel.</param>
        public BitmapCache(int size)
        {
            cleanTimer = new Timer(new TimerCallback(TimerCallback), null, Timeout.Infinite, Timeout.Infinite);
            cacheSize = size;
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to MulLib.BitmapCache.
        /// </summary>
        public object SyncRoot
        {
            get { return list.SyncRoot; }
        }

        /// <summary>
        /// Gets or sets maximum allowed cache size in pixels.
        /// </summary>
        public int CacheSize
        {
            get { return cacheSize; }
            set { cacheSize = value; }
        }

        /// <summary>
        /// Gets size of data in cache. In pixels.
        /// </summary>
        public int DataSize
        {
            get { return dataSize; }
        }

        /// <summary>
        /// Return whether cache contains bitmap of specied id or not.
        /// </summary>
        /// <param name="key">Bitmap id.</param>
        /// <returns>True if bitmap exists; otherwise false.</returns>
        public bool Contains(object key)
        {
            return list.Contains(key);
        }

        public Bitmap Get(object key)
        {
            lock (list.SyncRoot)
            {
                DataObject obj = (DataObject)list[key];
                if (obj != null)
                {
                    obj.LastAccess = DateTime.Now;
                    return obj.Bitmap;
                }
                else
                {
                    return null;
                }
            }
        }

        public void Set(object key, Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            DataObject obj = new DataObject(bitmap);

            lock (list.SyncRoot)
            {
                list.Add(key, obj);
                dataSize += obj.Size;

                if (dataSize > cacheSize && !timerEnabled)
                {
                    cleanTimer.Change(500, Timeout.Infinite);
                    timerEnabled = true;
                }
            }
        }

        public Bitmap this[object key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        private void TimerCallback(object state)
        {
            lock (list.SyncRoot)
            {
                while (dataSize > cacheSize && list.Count > 1)
                {
                    Object key = null;
                    DataObject obj = null;

                    foreach (DictionaryEntry entry in list)
                    {
                        if (obj == null)
                        {
                            key = entry.Key;
                            obj = (DataObject)entry.Value;
                        }
                        else
                        {
                            DataObject current = (DataObject)entry.Value;

                            if (obj.LastAccess < current.LastAccess)
                            {
                                key = entry.Key;
                                obj = current;
                            }
                        }
                    }

                    if (obj != null)
                    {
                        list.Remove(key);
                        dataSize -= obj.Size;
                    }
                }

                timerEnabled = false;
            }
        }
    }
}
