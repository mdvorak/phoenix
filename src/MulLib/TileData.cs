using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MulLib
{
    /// <summary>
    /// Loads tiledata file and provide access to data by index.
    /// Contains static functions to reading and writing raw data from/to stream.
    /// </summary>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public sealed class TileData : IDisposable
    {
        #region Data objects

        /// <summary>
        /// Properties for each item.
        /// Usage is not exactly known for most flags
        /// </summary>
        [Flags]
        public enum Flags : uint
        {
            None = 0,
            /// <summary>
            /// You will step on this and you will be moved at items Z
            /// </summary>
            Floor = 0x00000001,
            /// <summary>
            /// Usage unknown
            /// </summary>
            Weapon = 0x00000002,
            /// <summary>
            /// Usage unknown
            /// </summary>
            Transparent = 0x00000004,
            /// <summary>
            /// Usage unknown
            /// </summary>
            Translucent = 0x00000008,
            /// <summary>
            /// Specific usage unknown, it combines flags Impassible and NoShoot I thing
            /// </summary>
            Wall = 0x00000010,
            /// <summary>
            /// This item should cause you some damage, specific use unknown
            /// </summary>
            Damaging = 0x00000020,
            /// <summary>
            /// Player cant walk on/thru this item
            /// </summary>
            Impassible = 0x00000040,
            /// <summary>
            /// Something with water
            /// </summary>
            Wet = 0x00000080,
            /// <summary>
            /// Unknown
            /// </summary>
            Unknown1 = 0x00000100,
            /// <summary>
            /// You can drop items here
            /// </summary>
            Surface = 0x00000200,
            /// <summary>
            /// You could problably walk on it even if it is at higher than you.
            /// </summary>
            Climb = 0x00000400,
            /// <summary>
            /// Many items could be placed in one stock
            /// </summary>
            Stockable = 0x00000800,
            /// <summary>
            /// Item is window and have window-like shadow
            /// </summary>
            Window = 0x00001000,
            /// <summary>
            /// You cant shoot over this item
            /// </summary>
            NoShoot = 0x00002000,
            /// <summary>
            /// Item has 'a' article as default
            /// </summary>
            ArticleA = 0x00004000,
            /// <summary>
            /// Item has 'an' article as default
            /// </summary>
            ArticleAn = 0x00008000,
            /// <summary>
            /// Usage unknown
            /// </summary>
            Moongen = 0x00010000,
            /// <summary>
            /// Unknown
            /// </summary>
            Foliage = 0x00020000,
            /// <summary>
            /// Item is partially colored, currently unused as i know
            /// </summary>
            PartialHue = 0x00040000,
            /// <summary>
            /// Unknown
            /// </summary>
            Unknown2 = 0x00080000,
            /// <summary>
            /// Usage unknown
            /// </summary>
            Map = 0x00100000,
            /// <summary>
            /// Item could contain other items
            /// </summary>
            Container = 0x00200000,
            /// <summary>
            /// You can wear this
            /// </summary>
            Wearable = 0x00400000,
            /// <summary>
            /// Produces light
            /// </summary>
            LightSource = 0x00800000,
            /// <summary>
            /// Item is animated, detailed info probably in Animation property
            /// </summary>
            Animation = 0x01000000,
            /// <summary>
            /// Unknown
            /// </summary>
            NoDiagonal = 0x02000000,
            /// <summary>
            /// Unknown
            /// </summary>
            Unknown3 = 0x04000000,
            /// <summary>
            /// Usage unknown
            /// </summary>
            Armor = 0x08000000,
            /// <summary>
            /// You are in building if you are under this (probably)
            /// </summary>
            Roof = 0x10000000,
            /// <summary>
            /// Usage unknown
            /// </summary>
            Door = 0x20000000,
            /// <summary>
            /// You can walk on it from front side
            /// </summary>
            StairBack = 0x40000000,
            /// <summary>
            /// You can walk on it from left side
            /// </summary>
            StairRight = 0x80000000
        }

        /// <summary>
        /// Lands list
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Land
        {
            private Flags flags;
            private ushort textureID;
            private string name;

            /// <summary>
            /// Combination of Flags
            /// </summary>
            public Flags Flags
            {
                get { return flags; }
                set { flags = value; }
            }

            /// <summary>
            /// Id of texture to use probably from "texmaps.mul"
            /// </summary>
            public ushort TextureID
            {
                get { return textureID; }
                set { textureID = value; }
            }

            /// <summary>
            /// Name of item of maximum lenght of 20 characters.
            /// </summary>
            public string Name
            {
                get { return name; }
                set
                {
                    if (value.Length > 20)
                    {
                        Debug.WriteLine("TileData: Too long land name. String trimmed to 20 chars.", "MulLib");
                        name = value.Remove(20).Trim();
                    }
                    else
                        name = value.Trim();
                }
            }

            /// <summary>
            /// Gets or sets name from/to byte array with ascii encoding.
            /// </summary>
            /// <value>Byte array containing encoded string.</value>
            public byte[] NameBytes
            {
                get
                {
                    byte[] bytes = new byte[20];
                    ASCIIEncoding.ASCII.GetBytes(name, 0, name.Length, bytes, 0);
                    return bytes;
                }
                set
                {
                    this.Name = ASCIIEncoding.ASCII.GetString(value);
                }
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance.</returns>
            public override int GetHashCode()
            {
                return textureID;
            }

            /// <summary>
            /// Compares actual object with specified one.
            /// </summary>
            /// <param name="obj">Object that will be this instance compared to.</param>
            /// <returns>True if object are identical; otherwise false.</returns>
            public override bool Equals(object obj)
            {
                if (obj is Land)
                {
                    return ((Land)obj) == this;
                }
                return false;
            }

            /// <summary>
            /// Returns a string that represents the current Land object.
            /// </summary>
            /// <returns>Returns a string that represents the current Land object.</returns>
            public override string ToString()
            {
                return String.Format("Id: {0} Name: {1} Flags: 0x{2}", textureID, name, ((uint)flags).ToString("X8"));
            }

            /// <summary>
            /// Compares two Land objects.
            /// </summary>
            /// <param name="l1">One of the comapred objects.</param>
            /// <param name="l2">One of the comapred objects.</param>
            /// <returns>True if object are identical; otherwise false.</returns>
            public static bool operator ==(Land l1, Land l2)
            {
                return l1.textureID == l2.textureID && l1.name == l2.name && l1.flags == l2.flags;
            }

            /// <summary>
            /// Compares two Land objects.
            /// </summary>
            /// <param name="l1">One of the comapred objects.</param>
            /// <param name="l2">One of the comapred objects.</param>
            /// <returns>True if object are not identical; otherwise false.</returns>
            public static bool operator !=(Land l1, Land l2)
            {
                return l1.textureID != l2.textureID || l1.name != l2.name || l1.flags != l2.flags;
            }
        }

        /// <summary>
        /// Art  list
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Art
        {
            private Flags flags;
            private byte weight;
            private byte layer;
            private ushort unknown1;
            private byte unknown2;
            private byte quantity;
            private ushort animation;
            private byte unknown3;
            private byte hue;
            private byte unknown4;
            private byte value;
            private byte height;
            private string name;

            /// <summary>
            /// Combination of Flags
            /// </summary>
            public Flags Flags
            {
                get { return flags; }
                set { flags = value; }
            }

            /// <summary>
            /// Weight of the item
            /// </summary>
            public byte Weight
            {
                get { return weight; }
                set { weight = value; }
            }

            /// <summary>
            /// Sometimes it is Layer, sometimes something else.. (some source called it Quality)
            /// </summary>
            public byte Layer
            {
                get { return layer; }
                set { layer = value; }
            }

            /// <summary>
            /// Unknown
            /// </summary>
            public ushort Unknown1
            {
                get { return unknown1; }
                set { unknown1 = value; }
            }

            /// <summary>
            /// Unknown
            /// </summary>
            public byte Unknown2
            {
                get { return unknown2; }
                set { unknown2 = value; }
            }

            /// <summary>
            /// Use unknown
            /// </summary>
            public byte Quantity
            {
                get { return quantity; }
                set { quantity = value; }
            }

            /// <summary>
            /// Probably how many graphics is used to this animated item
            /// </summary>
            public ushort Animation
            {
                get { return animation; }
                set { animation = value; }
            }

            /// <summary>
            /// Unknown
            /// </summary>
            public byte Unknown3
            {
                get { return unknown3; }
                set { unknown3 = value; }
            }

            /// <summary>
            /// Use unknown, probably currently unused
            /// </summary>
            public byte Hue
            {
                get { return hue; }
                set { hue = value; }
            }

            /// <summary>
            /// Unknown
            /// </summary>
            public byte Unknown4
            {
                get { return unknown4; }
                set { unknown4 = value; }
            }

            /// <summary>
            /// Use unknown
            /// </summary>
            public byte Value
            {
                get { return value; }
                set { this.value = value; }
            }

            /// <summary>
            /// Height of item
            /// </summary>
            public byte Height
            {
                get { return height; }
                set { height = value; }
            }

            /// <summary>
            /// Name of item of maximum lenght of 20 characters
            /// </summary>
            public string Name
            {
                get { return name; }
                set
                {
                    if (value.Length > 20)
                    {
                        Debug.WriteLine("TileData: Too long art name. String trimmed to 20 chars.", "MulLib");
                        name = value.Remove(20).Trim();
                    }
                    else
                        name = value.Trim();
                }
            }

            /// <summary>
            /// Gets or sets name from/to byte array with ascii encoding
            /// </summary>
            /// <value>Byte array containing encoded string.</value>
            public byte[] NameBytes
            {
                get
                {
                    byte[] bytes = new byte[20];
                    ASCIIEncoding.ASCII.GetBytes(name, 0, name.Length, bytes, 0);
                    return bytes;
                }
                set
                {
                    this.Name = ASCIIEncoding.ASCII.GetString(value);
                }
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance.</returns>
            public override int GetHashCode()
            {
                return height ^ value ^ (int)flags;
            }

            /// <summary>
            /// Comapares this obejct with another one.
            /// </summary>
            /// <param name="obj">Second comapred object.</param>
            /// <returns>True if object is identical; otherwise false.</returns>
            public override bool Equals(object obj)
            {
                if (obj is Art)
                {
                    return ((Art)obj) == this;
                }
                return false;
            }

            /// <summary>
            /// Returns a string that represents the current Art object.
            /// </summary>
            /// <returns>Returns a string that represents the current Art object.</returns>
            public override string ToString()
            {
                return String.Format("Name: {0} Flags: 0x{1} Animation: {2} Height: {3} Hue: {4} Quality: {5} Quantity: {6} Value: {7} Weight: {8}",
                                     name, flags.ToString("X8"), animation, height, hue, layer, quantity, value, weight);
            }

            /// <summary>
            /// Compares two Art objects.
            /// </summary>
            /// <param name="a1">One of the comapred objects.</param>
            /// <param name="a2">One of the comapred objects.</param>
            /// <returns>True if object are identical; otherwise false.</returns>
            public static bool operator ==(Art a1, Art a2)
            {
                return a1.flags == a2.flags && a1.weight == a2.weight && a1.value == a2.value &&
                       a1.animation == a2.animation && a1.height == a2.height && a1.hue == a2.hue &&
                       a1.name == a2.name && a1.layer == a2.layer && a1.quantity == a2.quantity &&
                       a1.unknown1 == a2.unknown1 && a1.unknown2 == a2.unknown2 && a1.unknown3 == a2.unknown3 && a1.unknown4 == a2.unknown4;
            }

            /// <summary>
            /// Compares two Art objects.
            /// </summary>
            /// <param name="a1">One of the comapred objects.</param>
            /// <param name="a2">One of the comapred objects.</param>
            /// <returns>True if object are not identical; otherwise false.</returns>
            public static bool operator !=(Art a1, Art a2)
            {
                return !(a1 == a2);
            }
        }

        #endregion

        /// <summary>
        /// Maximum index = 16384.
        /// </summary>
        public const ushort MaxIndex = 0x4000;

        /// <summary>
        /// Expected file size. Value = 1036288 bytes.
        /// </summary>
        /// <value>Exact file size (1036288 bytes).</value>
        public const int DefaultFileLenght = 1036288;

        private object syncRoot = new object();

        private Land[] lands;
        private Art[] arts;

        /// <summary>
        /// Initializes the new empty TileData object.
        /// </summary>
        public TileData()
        {
            lands = new Land[MaxIndex];
            arts = new Art[MaxIndex];
        }

        /// <summary>
        /// Gets synchronization object.
        /// </summary>
        public object SyncRoot
        {
            get { return syncRoot; }
        } 

        /// <summary>
        /// Gets arts and lands count.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public int Count
        {
            get
            {
                lock (syncRoot)
                {
                    if (Disposed)
                        throw new ObjectDisposedException("TileData");

                    return lands.Length;
                }
            }
        }

        /// <summary>
        /// Reads a Land with specified index.
        /// </summary>
        /// <param name="index">Index between 0 and MaxIndex.</param>
        /// <returns>New instance of Land filled with read list</returns>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public Land GetLand(int index)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("TileData");

                return lands[index];
            }
        }

        /// <summary>
        /// Writes a Land data with specified index.
        /// </summary>
        /// <param name="index">Index between 0 and MaxIndex.</param>
        /// <param name="land">Data to write.</param>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void SetLand(int index, Land land)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("TileData");

                lands[index] = land;
            }
        }

        /// <summary>
        /// Reads an Art data with specified index.
        /// </summary>
        /// <param name="index">Index between 0 and MaxIndex.</param>
        /// <returns>New instance of Art filled with read list</returns>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public Art GetArt(int index)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("TileData");

                return arts[index];
            }
        }

        /// <summary>
        /// Writes an Art with specified index.
        /// </summary>
        /// <param name="index">Index between 0 and MaxIndex.</param>
        /// <param name="art">Data to write</param>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void SetArt(int index, Art art)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("TileData");

                arts[index] = art;
            }
        }

        /// <summary>
        /// Gets whether object has been disposed.
        /// </summary>
        public bool Disposed
        {
            get { return lands == null || arts == null; }
        }

        /// <summary>
        /// Disposes object.
        /// </summary>
        public void Dispose()
        {
            lock (syncRoot)
            {
                lands = null;
                arts = null;
            }
        }

        /// <summary>
        /// Saves data to the file.
        /// </summary>
        /// <param name="file">File path.</param>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void Save(string file)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("TileData");

                Stream stream = null;
                BinaryWriter writer = null;

                try
                {
                    stream = File.OpenWrite(file);
                    writer = new BinaryWriter(stream);

                    for (int i = 0; i < MaxIndex; i++)
                    {
                        if (i % 32 == 0) writer.Write((int)0);
                        TileData.WriteLand(writer, lands[i]);
                    }

                    for (int i = 0; i < MaxIndex; i++)
                    {
                        if (i % 32 == 0) writer.Write((int)0);
                        TileData.WriteArt(writer, arts[i]);
                    }

                    Trace.WriteLine(String.Format("TileData: File \"{0}\" succesfully saved.", file), "MulLib");
                }
                catch (Exception e)
                {
                    throw new Exception("Error saving TileData.", e);
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
        /// Loads TileData object from file.
        /// </summary>
        /// <param name="file">Path to "tiledata.mul" file.</param>
        public static TileData Load(string file)
        {
            Stream stream = null;
            BinaryReader reader = null;

            try
            {
                stream = File.OpenRead(file);
                reader = new BinaryReader(stream);

                if (stream.Length < TileData.DefaultFileLenght)
                {
                    throw new Exception("Invalid TileData file size. Expected: " + TileData.DefaultFileLenght.ToString());
                }

                TileData tileData = new TileData();

                for (int i = 0; i < MaxIndex; i++)
                {
                    if (i % 32 == 0) stream.Seek(4, SeekOrigin.Current);
                    tileData.lands[i] = TileData.ReadLand(reader);
                }

                for (int i = 0; i < MaxIndex; i++)
                {
                    if (i % 32 == 0) stream.Seek(4, SeekOrigin.Current);
                    tileData.arts[i] = TileData.ReadArt(reader);
                }

                Trace.WriteLine(String.Format("TileData: File \"{0}\" succesfully loaded.", file), "MulLib");
                return tileData;
            }
            catch (Exception e)
            {
                throw new Exception("Error loading TileData.", e);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (stream != null)
                    stream.Close();
            }
        }

        #region Static read and write functions

        /// <summary>
        /// Static function that reads Land data from specified file at current location.
        /// </summary>
        /// <param name="reader">Stream to read from.</param>
        /// <returns>Read data.</returns>
        public static Land ReadLand(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            Land land = new Land();

            land.Flags = (Flags)reader.ReadUInt32();
            land.TextureID = reader.ReadUInt16();
            land.NameBytes = reader.ReadBytes(20);

            return land;
        }

        /// <summary>
        /// Static function that writes Land data to specified file at current location.
        /// </summary>
        /// <param name="writer">Stream to write to.</param>
        /// <param name="land">Object to write.</param>
        public static void WriteLand(BinaryWriter writer, Land land)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            writer.Write((uint)land.Flags);
            writer.Write(land.TextureID);
            writer.Write(land.NameBytes, 0, 20);
        }

        /// <summary>
        /// Static function that reads Art data from specified file at current location.
        /// </summary>
        /// <param name="reader">Stream to read from.</param>
        /// <returns>Read data.</returns>
        public static Art ReadArt(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            Art art = new Art();

            art.Flags = (Flags)reader.ReadUInt32();
            art.Weight = reader.ReadByte();
            art.Layer = reader.ReadByte();
            art.Unknown1 = reader.ReadUInt16();
            art.Unknown2 = reader.ReadByte();
            art.Quantity = reader.ReadByte();
            art.Animation = reader.ReadUInt16();
            art.Unknown3 = reader.ReadByte();
            art.Hue = reader.ReadByte();
            art.Unknown4 = reader.ReadByte();
            art.Value = reader.ReadByte();
            art.Height = reader.ReadByte();
            art.NameBytes = reader.ReadBytes(20);

            return art;
        }

        /// <summary>
        /// Static function that writes Art data to specified file at current location.
        /// </summary>
        /// <param name="writer">Stream to write to.</param>
        /// <param name="art">Object to write.</param>
        public static void WriteArt(BinaryWriter writer, Art art)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            writer.Write((uint)art.Flags);
            writer.Write(art.Weight);
            writer.Write(art.Layer);
            writer.Write(art.Unknown1);
            writer.Write(art.Unknown2);
            writer.Write(art.Quantity);
            writer.Write(art.Animation);
            writer.Write(art.Unknown3);
            writer.Write(art.Hue);
            writer.Write(art.Unknown4);
            writer.Write(art.Value);
            writer.Write(art.Height);
            writer.Write(art.NameBytes, 0, 20);
        }

        #endregion
    }
}

