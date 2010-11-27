using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MulLib
{
    [Serializable]
    public struct MultiTile
    {
        public ushort ItemID { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public int Flags { get; set; }

        public bool IsVisible
        {
            get { return (Flags & 1) > 0; }
        }
    }

    [Serializable]
    public struct MultiData
    {
        public ushort Id { get; set; }
        public MultiTile[] Tiles { get; set; }
    }

    public class Multi : IEnumerable<MultiData>
    {
        private Dictionary<ushort, MultiData> data = new Dictionary<ushort, MultiData>();

        public MultiData this[int index]
        {
            get { return data[(ushort)index]; }
        }

        public IEnumerator<MultiData> GetEnumerator()
        {
            return data.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Multi Load(string idxFile, string mulFile, MulFileAccessMode mode)
        {
            IndexFile indexFile = null;
            Stream stream = null;
            BinaryReader reader = null;

            try {
                indexFile = IndexFile.Load(idxFile);

                stream = File.OpenRead(mulFile);
                reader = new BinaryReader(stream);

                Multi multi = new Multi();

                IndexData indexData;
                for (int i = 0; i < indexFile.Count; i++) {
                    indexData = indexFile[i];

                    if (indexData.IsValid) {
                        stream.Seek(indexData.Lookup, SeekOrigin.Begin);

                        MultiData multiData = new MultiData();
                        multiData.Id = (ushort)i;
                        multiData.Tiles = new MultiTile[indexData.Lenght / 12];

                        for (int j = 0; j < indexData.Lenght / 12; j++) {
                            MultiTile tile = new MultiTile {
                                ItemID = reader.ReadUInt16(),
                                X = reader.ReadInt16(),
                                Y = reader.ReadInt16(),
                                Z = reader.ReadInt16(),
                                Flags = reader.ReadInt32()
                            };

                            multiData.Tiles[j] = tile;
                        }

                        multi.data.Add(multiData.Id, multiData);
                    }
                }

                Trace.WriteLine(String.Format("Multi: File \"{0}\" succesfully loaded.", mulFile), "MulLib");
                return multi;
            }
            catch (Exception e) {
                throw new Exception("Error loading Multi.", e);
            }
            finally {
                if (indexFile != null)
                    indexFile.Dispose();

                if (reader != null)
                    reader.Close();

                if (stream != null)
                    stream.Close();
            }
        }
    }
}
