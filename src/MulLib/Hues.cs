using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MulLib
{
    /// <summary>
    /// Hue data object.
    /// </summary>
    public sealed class HueEntry
    {
        /// <summary>
        /// Object lenght in bytes.
        /// </summary>
        public const int Lenght = 88;

        private ushort[] colors = new ushort[32];
        private ushort tableStart;
        private ushort tableEnd;
        private string name = "";

        /// <summary>
        /// Gets or sets color table of this hue. Table contains 32 colors.
        /// </summary>
        public ushort[] Colors
        {
            get { return colors; }
            set
            {
                if (value.Length != 32)
                    throw new ArgumentException("Color table must have exactly 32 colors.");

                colors = value;
            }
        }

        /// <summary>
        /// Gets or sets TableStart value. Actual usage unknown. Could probably be ingored.
        /// </summary>
        public ushort TableStart
        {
            get { return tableStart; }
            set { tableStart = value; }
        }

        /// <summary>
        /// Gets or sets TableEnd value. Actual usage unknown. Could probably be ingored.
        /// </summary>
        public ushort TableEnd
        {
            get { return tableEnd; }
            set { tableEnd = value; }
        }

        /// <summary>
        /// Gets or sets name of this hue. Maximum lenght is 20 characters.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (value.Length > 20)
                {
                    Debug.WriteLine("Hues: Too long hue name. String trimmed to 20 chars.", "MulLib");
                    name = value.Remove(20).Trim();
                }
                else
                    name = value.Trim();
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return tableStart ^ tableEnd;
        }

        /// <summary>
        /// Returns a string that represents the current HueEntry object.
        /// </summary>
        /// <returns>Returns a string that represents the current HueEntry object.</returns>
        public override string ToString()
        {
            return String.Format("{0}->{1} {2}", tableStart, tableEnd, name);
        }
    }

    /// <summary>
    /// Collection of hues.
    /// </summary>
    public sealed class Hues : IDisposable
    {
        #region Block class

        private class Block
        {
            public const int Lenght = 4 + HueEntry.Lenght * 8;

            private uint header;
            private HueEntry[] entries;

            public Block()
            {
                entries = new HueEntry[8];

                for (int i = 0; i < 8; i++)
                {
                    entries[i] = new HueEntry();
                }
            }

            public uint Header
            {
                get { return header; }
                set { header = value; }
            }

            public HueEntry[] Entries
            {
                get { return entries; }
            }
        }

        #endregion

        private readonly object syncRoot;
        private List<Block> blockList;

        /// <summary>
        /// Constructs empty Hues object.
        /// </summary>
        public Hues()
        {
            syncRoot = new object();
            blockList = new List<Block>(256);
        }

        /// <summary>
        /// Gets synchronization object.
        /// </summary>
        public object SyncRoot
        {
            get { return syncRoot; }
        } 

        /// <summary>
        /// Gets minimum index that can be used
        /// </summary>
        public int MinIndex
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets maximum index that can be used.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public int MaxIndex
        {
            get
            {
                lock (syncRoot)
                {
                    if (Disposed)
                        throw new ObjectDisposedException("Hues");

                    return blockList.Count * 8;
                }
            }
        }

        /// <summary>
        /// Gets or sets hue of specified index.
        /// </summary>
        /// <param name="index">Hue index.</param>
        /// <returns>Hue data object.</returns>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public HueEntry this[int index]
        {
            get
            {
                lock (syncRoot)
                {
                    if (Disposed)
                        throw new ObjectDisposedException("Hues");

                    if (index < MinIndex || index > MaxIndex)
                        throw new ArgumentOutOfRangeException("index");

                    index--;
                    return blockList[index / 8].Entries[index % 8];
                }
            }

            set
            {
                lock (syncRoot)
                {
                    if (Disposed)
                        throw new ObjectDisposedException("Hues");

                    if (index < MinIndex || index > MaxIndex)
                        throw new ArgumentOutOfRangeException("index");

                    index--;
                    blockList[index / 8].Entries[index % 8] = value;
                }
            }
        }

        /// <summary>
        /// Returns hue of specified index.
        /// </summary>
        /// <param name="index">Hue index.</param>
        /// <returns>Hue data object.</returns>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public HueEntry Get(int index)
        {
            return this[index];
        }

        /// <summary>
        /// Adds 8 hue entries to the list.
        /// </summary>
        /// <param name="header">Block header, use 0. Usage unknown.</param>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void AddBlock(uint header)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("Hues");

                Block block = new Block();
                block.Header = header;
                blockList.Add(block);
            }
        }

        /// <summary>
        /// Returns True if object has been disposed; otherwise false.
        /// </summary>
        public bool Disposed
        {
            get { return blockList == null; }
        }

        /// <summary>
        /// Frees all resources holded by this object.
        /// </summary>
        public void Dispose()
        {
            lock (syncRoot)
            {
                if (blockList != null)
                {
                    blockList.Clear();
                    blockList = null;
                }
            }
        }

        /// <summary>
        /// Stores this object to specified file in Hues.mul format.
        /// </summary>
        /// <param name="file">File path.</param>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void Save(string file)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("Hues");

                Stream stream = null;
                BinaryWriter writer = null;

                try
                {
                    stream = File.OpenWrite(file);
                    writer = new BinaryWriter(stream);

                    for (int i = 0; i < blockList.Count; i++)
                    {
                        writer.Write(blockList[i].Header);

                        for (int b = 0; b < 8; b++)
                        {
                            HueEntry entry = blockList[i].Entries[b];

                            for (int c = 0; c < 32; c++)
                                writer.Write(entry.Colors[c]);

                            writer.Write(entry.TableStart);
                            writer.Write(entry.TableEnd);

                            byte[] name = Encoding.ASCII.GetBytes(entry.Name);
                            Array.Resize<byte>(ref name, 20);
                            writer.Write(name, 0, name.Length);
                        }
                    }

                    Trace.WriteLine(String.Format("Hues: File \"{0}\" succesfully saved.", file), "MulLib");
                }
                catch (Exception e)
                {
                    throw new Exception("Error saving Hues.", e);
                }
                finally
                {
                    if (writer != null)
                        writer.Close();

                    if (stream != null)
                        stream.Close();
                }
            }
        }

        /// <summary>
        /// Builds Hues object from file in Hues.mul format.
        /// </summary>
        /// <param name="file">File path.</param>
        public static Hues Load(string file)
        {
            Stream stream = null;
            BinaryReader reader = null;

            try
            {
                stream = File.OpenRead(file);
                reader = new BinaryReader(stream);

                Hues hues = new Hues();

                int blockCount = (int)(stream.Length / Block.Lenght);

                for (int i = 0; i < blockCount; i++)
                {
                    uint header = reader.ReadUInt32();

                    hues.AddBlock(header);

                    for (int b = 0; b < 8; b++)
                    {
                        HueEntry entry = new HueEntry();

                        for (int c = 0; c < 32; c++)
                            entry.Colors[c] = reader.ReadUInt16();

                        entry.TableStart = reader.ReadUInt16();
                        entry.TableEnd = reader.ReadUInt16();

                        byte[] nameBytes = reader.ReadBytes(20);
                        string name = Encoding.ASCII.GetString(nameBytes);
                        if (name.Contains("\0"))
                            name = name.Remove(name.IndexOf('\0'));

                        entry.Name = name;

                        hues.blockList[i].Entries[b] = entry;
                    }
                }

                Trace.WriteLine(String.Format("Hues: MaxIndex={0}.", hues.MaxIndex), "MulLib");

                Trace.WriteLine(String.Format("Hues: File \"{0}\" succesfully loaded.", file), "MulLib");
                return hues;
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Hues.", e);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (stream != null)
                    stream.Close();
            }
        }
    }
}
