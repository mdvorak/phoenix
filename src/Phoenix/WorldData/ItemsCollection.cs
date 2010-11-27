using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    public class ItemsCollection : IEnumerable<UOItem>
    {
        private static readonly Layer[] allowedLayers = new Layer[] {
            Layer.None,
            Layer.Bracelet, 
            Layer.Cloak, 
            Layer.Earrings, 
            Layer.Gloves, 
            Layer.Hat, 
            Layer.InnerLegs, 
            Layer.InnerTorso, 
            Layer.LeftHand, 
            Layer.MiddleTorso, 
            Layer.Neck, 
            Layer.OuterLegs, 
            Layer.OuterTorso, 
            Layer.Pants, 
            Layer.RightHand, 
            Layer.Ring, 
            Layer.Shirt, 
            Layer.Shoes, 
            Layer.Waist, 
            Layer.Backpack
        };

        #region Enumerator class

        protected class Enumerator : IEnumerator<UOItem>
        {
            private uint serial;
            private List<uint> usedList;
            private bool secondPhase;
            private UOItem current;
            private bool searchSub;
            private bool ignoreBank = true;

            /// <summary>
            /// Default c..tor
            /// </summary>
            /// <param name="serial">Container serial.</param>
            public Enumerator(uint serial, bool searchSubContainers)
            {
                this.serial = serial;
                usedList = new List<uint>();
                secondPhase = false;
                current = null;
                searchSub = searchSubContainers;
            }

            #region IEnumerator<UOItem> Members

            public UOItem Current
            {
                get
                {
                    if (usedList == null) throw new InvalidOperationException("Enumerator has been disposed.");
                    return current;
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                usedList = null;
                secondPhase = false;
                current = null;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (usedList == null) throw new InvalidOperationException("Enumerator has been disposed.");
                    return current;
                }
            }

            public bool MoveNext()
            {
                lock (World.SyncRoot) {
                    if (usedList == null) throw new InvalidOperationException("Enumerator has been disposed.");

                    if (!secondPhase) {
                        foreach (KeyValuePair<uint, RealItem> pair in World.ItemList) {
                            if (pair.Value.Container == serial && !usedList.Contains(pair.Value.Serial)) {
                                if (pair.Value.Container == 0 && pair.Value.GetDistance(World.RealPlayer) > World.FindDistance)
                                    continue;
                                if (ignoreBank && pair.Value.Layer == (byte)Layer.Bank)
                                    continue;

                                current = new UOItem(pair.Value.Serial);
                                usedList.Add(pair.Value.Serial);
                                return true;
                            }
                        }

                        secondPhase = true;
                    }

                    if (searchSub) {
                        if (usedList.Count < World.ItemList.Count) {
                            foreach (KeyValuePair<uint, RealItem> pair in World.ItemList) {
                                if (usedList.Contains(pair.Value.Container) && !usedList.Contains(pair.Value.Serial)) {
                                    current = new UOItem(pair.Value.Serial);
                                    usedList.Add(pair.Value.Serial);
                                    return true;
                                }
                            }
                        }
                        else {
                            while (usedList.Count > 0) {
                                foreach (KeyValuePair<uint, RealItem> pair in World.ItemList) {
                                    if (pair.Value.Container == usedList[0] && !usedList.Contains(pair.Value.Serial)) {
                                        current = new UOItem(pair.Value.Serial);
                                        usedList.Add(pair.Value.Serial);
                                        return true;
                                    }
                                }

                                usedList.RemoveAt(0);
                            }
                        }
                    }

                    return false;
                }
            }

            public void Reset()
            {
                if (usedList == null) throw new InvalidOperationException("Enumerator has been disposed.");
                usedList.Clear();
                secondPhase = false;
                current = null;
            }

            #endregion
        }

        #endregion

        private uint serial;
        private bool searchSubContainers;

        public ItemsCollection(uint serial, bool searchSubContainers)
        {
            this.serial = serial;
            this.searchSubContainers = searchSubContainers;
        }

        public Serial Container
        {
            get { return serial; }
        }

        public bool SearchSubContainers
        {
            get { return searchSubContainers; }
        }

        public virtual IEnumerator<UOItem> GetEnumerator()
        {
            return new Enumerator(serial, searchSubContainers);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(serial, searchSubContainers);
        }

        /// <summary>
        /// Finds out whether item is in this container or not.
        /// </summary>
        /// <param name="serial">Item serial.</param>
        /// <returns></returns>
        public bool Contains(Serial serial)
        {
            lock (World.SyncRoot) {
                RealItem item = World.FindRealItem(serial);

                while (item != null && (this.serial == 0 || item.Container != 0)) {
                    if (item.Container == this.serial)
                        return true;
                    else
                        item = searchSubContainers ? World.FindRealItem(item.Container) : null;
                }

                return false;
            }
        }

        public int Count(Graphic graphic)
        {
            int count = 0;

            foreach (UOItem item in this) {
                if (item.Graphic == graphic) {
                    if (item.Amount > 0) count += item.Amount;
                    else count++;
                }
            }

            return count;
        }

        public int Count(Graphic graphic, UOColor color)
        {
            int count = 0;

            foreach (UOItem item in this) {
                if (item.Graphic == graphic && item.Color == color) {
                    if (item.Amount > 0) count += item.Amount;
                    else count++;
                }
            }

            return count;
        }

        public UOItem FindType(Graphic graphic)
        {
            foreach (UOItem item in this) {
                if (item.Graphic == graphic && Array.IndexOf<Layer>(allowedLayers, item.Layer) >= 0)
                    return item;
            }

            return new UOItem(Serial.Invalid);
        }

        public UOItem FindType(Graphic graphic, UOColor color)
        {
            foreach (UOItem item in this) {
                if (item.Graphic == graphic && item.Color == color && Array.IndexOf<Layer>(allowedLayers, item.Layer) >= 0)
                    return item;
            }

            return new UOItem(Serial.Invalid);
        }

        public int CountItems()
        {
            lock (World.SyncRoot) {
                int count = 0;
                List<uint> usedList = new List<uint>();

                foreach (KeyValuePair<uint, RealItem> pair in World.ItemList) {
                    if (pair.Value.Container == serial) {
                        if (pair.Value.Container == 0 && pair.Value.GetDistance(World.RealPlayer) > World.FindDistance)
                            continue;

                        usedList.Add(pair.Value.Serial);
                        count++;
                    }
                }

                if (searchSubContainers) {
                    foreach (KeyValuePair<uint, RealItem> pair in World.ItemList) {
                        if (usedList.Contains(pair.Value.Container) && !usedList.Contains(pair.Value.Serial)) {
                            usedList.Add(pair.Value.Serial);
                            count++;
                        }
                    }
                }

                return count;
            }
        }
    }
}
