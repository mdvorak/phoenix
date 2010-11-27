using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    public class CharacterCollection : IEnumerable<UOCharacter>
    {
        #region Enumerator class

        class Enumerator : IEnumerator<UOCharacter>
        {
            private List<uint> usedList;
            private UOCharacter current;

            public Enumerator()
            {
                usedList = new List<uint>();
                current = null;
            }

            #region IEnumerator<UOCharacter> Members

            public UOCharacter Current
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
                lock (World.SyncRoot)
                {
                    if (usedList == null) throw new InvalidOperationException("Enumerator has been disposed.");

                    foreach (KeyValuePair<uint, RealCharacter> pair in World.CharList)
                    {
                        if (!usedList.Contains(pair.Value.Serial))
                        {
                            current = new UOCharacter(pair.Value.Serial);
                            usedList.Add(pair.Value.Serial);
                            return true;
                        }
                    }

                    return false;
                }
            }

            public void Reset()
            {
                if (usedList == null) throw new InvalidOperationException("Enumerator has been disposed.");
                usedList.Clear();
                current = null;
            }

            #endregion
        }

        #endregion

        public CharacterCollection()
        {
        }

        public IEnumerator<UOCharacter> GetEnumerator()
        {
            return new Enumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator();
        }
    }
}
