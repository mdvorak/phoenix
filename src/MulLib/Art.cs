using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MulLib
{
    /// <summary>
    /// Provides access to one type of Art data.
    /// </summary>
    public interface IArtData
    {
        /// <summary>
        /// Gets count of items.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets texture of object at specified index.
        /// </summary>
        /// <param name="index">Object ID.</param>
        /// <returns>Bitmap object or null.</returns>
        Bitmap Get(int index);
        /// <summary>
        /// Sets texture of object at specified index.
        /// </summary>
        /// <param name="index">Object ID.</param>
        /// <param name="bitmap">New texture or null to delete.</param>
        void Set(int index, Bitmap bitmap);
        /// <summary>
        /// Gets or sets texture of object at specified index.
        /// </summary>
        /// <param name="index">Object ID.</param>
        /// <returns>Bitmap object or null.</returns>
        Bitmap this[int index] { get; set; }
    }

    /// <summary>
    /// Provides access to UO Arts.
    /// Contains static functions to reading and writing raw data from/to stream
    /// </summary>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public sealed class Art : IDisposable
    {
        #region Data classes

        private abstract class DataBase : IArtData
        {
            protected Art art;

            protected DataBase(Art art)
            {
                this.art = art;
            }

            public virtual int Offset
            {
                get { return 0; }
            }

            /// <summary>
            /// Gets 16384.
            /// </summary>
            public virtual int Count
            {
                get { return 16384; }
            }

            public Bitmap Get(int index)
            {
                lock (art.SyncRoot) {
                    if (art.Disposed)
                        throw new ObjectDisposedException("Art");

                    if (index < 0 || index >= Count)
                        throw new ArgumentOutOfRangeException("index");

                    if (art.changeList.ContainsKey(index + Offset))
                        return art.changeList[index + Offset];
                    else {
                        IndexData indexData = art.indexFile.Get(index + Offset, false);
                        if (indexData.IsValid) {
                            return ReadFromStream(indexData);
                        }
                        else {
                            return null;
                        }
                    }
                }
            }

            protected abstract Bitmap ReadFromStream(IndexData indexData);

            public void Set(int index, Bitmap bitmap)
            {
                lock (art.SyncRoot) {
                    if (art.Disposed)
                        throw new ObjectDisposedException("Art");

                    if (index < 0 || index >= Count)
                        throw new ArgumentOutOfRangeException("index");

                    art.changeList[index + Offset] = bitmap;
                }
            }

            public Bitmap this[int index]
            {
                get { return Get(index); }
                set { Set(index, value); }
            }
        }

        private class LandscapeData : DataBase
        {
            public LandscapeData(Art art)
                : base(art)
            {

            }

            protected override Bitmap ReadFromStream(IndexData indexData)
            {
                art.dataStream.Seek(indexData.Lookup, SeekOrigin.Begin);
                return Art.ReadTile(art.dataStream);
            }
        }

        private class ItemsData : DataBase
        {
            public ItemsData(Art art)
                : base(art)
            {

            }

            public override int Offset
            {
                get { return 16384; }
            }

            protected override Bitmap ReadFromStream(IndexData indexData)
            {
                art.dataStream.Seek(indexData.Lookup, SeekOrigin.Begin);
                return Art.ReadRun(art.dataStream);
            }
        }

        private class AnimationsData : ItemsData
        {
            public AnimationsData(Art art)
                : base(art)
            {

            }

            public override int Count
            {
                get
                {
                    lock (art.syncRoot) {
                        return art.indexFile.Count - Offset;
                    }
                }
            }

            public override int Offset
            {
                get { return 16384 * 2; }
            }
        }

        #endregion

        private readonly object syncRoot = new object();
        private string mulFile;
        private IndexFile indexFile;
        private Stream dataStream;

        private Dictionary<int, Bitmap> changeList = new Dictionary<int, Bitmap>();

        private LandscapeData tileObject;
        private ItemsData itemsObject;
        private AnimationsData animationsObject;

        /// <summary>
        /// Initializes the new empty Art object.
        /// </summary>
        public Art()
        {
            mulFile = null;
            dataStream = Stream.Null;

            indexFile = new IndexFile();
            indexFile.Resize(65536);

            tileObject = new LandscapeData(this);
            itemsObject = new ItemsData(this);
            animationsObject = new AnimationsData(this);
        }

        private Art(string mulFile, IndexFile indexFile, Stream dataStream)
        {
            this.mulFile = mulFile;
            this.indexFile = indexFile;
            this.dataStream = dataStream;

            tileObject = new LandscapeData(this);
            itemsObject = new ItemsData(this);
            animationsObject = new AnimationsData(this);
        }

        /// <summary>
        /// Gets synchronization object.
        /// </summary>
        public object SyncRoot
        {
            get { return syncRoot; }
        }

        /// <summary>
        /// Gets object encapsulating access to Landscape textures.
        /// </summary>
        public IArtData Landscape
        {
            get { return tileObject; }
        }

        /// <summary>
        /// Gets object encapsulating access to Items textures.
        /// </summary>
        public IArtData Items
        {
            get { return itemsObject; }
        }

        /// <summary>
        /// Gets object encapsulating access to Animation textures.
        /// Now it is probably unused, in earlier versions used probably only for hit tests.
        /// </summary>
        public IArtData Animations
        {
            get { return animationsObject; }
        }

        /// <summary>
        /// Gets whether object has been disposed.
        /// </summary>
        public bool Disposed
        {
            get { return indexFile.Disposed; }
        }

        /// <summary>
        /// Disposes object.
        /// </summary>
        public void Dispose()
        {
            lock (syncRoot) {
                mulFile = null;
                indexFile.Dispose();
                dataStream.Close();
                changeList.Clear();
            }
        }

        /// <summary>
        /// Gets whether data file could be saved to currently opened file.
        /// </summary>
        public bool CanUpdateDataFile
        {
            get
            {
                lock (syncRoot) {
                    if (Disposed)
                        throw new ObjectDisposedException("Art");

                    return dataStream.CanWrite;
                }
            }
        }

        /// <summary>
        /// Gets currently opened data file.
        /// </summary>
        public string LoadedDataFile
        {
            get { return mulFile; }
        }

        /// <summary>
        /// Saves object to specified files.
        /// </summary>
        /// <param name="idxFile">Index file path.</param>
        /// <param name="mulFile">Data file path.</param>
        /// <remarks>
        /// If mulFile differs from currently loaded file, new file is optimized (data file is sorted and without empty entries).
        /// Otherwise file must be loaded with write access.
        /// </remarks>
        public void Save(string idxFile, string mulFile)
        {
            lock (syncRoot) {
                if (Disposed)
                    throw new ObjectDisposedException("Art");

                Stream indexStream = null;

                try {
                    indexStream = File.Open(idxFile, FileMode.Create, FileAccess.Write, FileShare.None);

                    if (String.Compare(mulFile, this.mulFile, StringComparison.InvariantCultureIgnoreCase) == 0) {
                        // Target data file is same as source file. File will not be optimized.
                        Stream dataStream = this.dataStream;

                        if (!dataStream.CanWrite)
                            throw new InvalidOperationException("Trying to save data to source file, that is not opened for Write access.");

                        foreach (KeyValuePair<int, Bitmap> pair in changeList) {
                            if (pair.Key < 16384) {
                                // Tile, can be saved to same location in data stream
                                IndexData indexData = indexFile.Get(pair.Key, true);
                                if (indexData.IsValid) {
                                    dataStream.Seek(indexData.Lookup, SeekOrigin.Begin);
                                    IndexData writtenData = Art.WriteTile(dataStream, pair.Value);

                                    Debug.Assert(writtenData.Lookup == indexData.Lookup, "writtenData.Lookup == indexData.Lookup");
                                    Debug.Assert(writtenData.Lenght == indexData.Lenght, "writtenData.Lenght == indexData.Lenght");
                                }
                                else {
                                    dataStream.Seek(0, SeekOrigin.End);
                                    IndexData writtenData = Art.WriteTile(dataStream, pair.Value);
                                    indexFile.Set(pair.Key, writtenData, true);
                                }
                            }
                            else {
                                // Run, will be saved to end of data stream
                                dataStream.Seek(0, SeekOrigin.End);
                                IndexData writtenData = Art.WriteRun(dataStream, pair.Value);
                                indexFile.Set(pair.Key, writtenData, true);
                            }
                        }

                        dataStream.Flush();
                        Trace.WriteLine(String.Format("Art: File \"{0}\" succesfully updated.", mulFile), "MulLib");

                        indexFile.Save(indexStream);
                        Trace.WriteLine(String.Format("IndexFile: File \"{0}\" succesfully saved.", idxFile), "MulLib");

                        changeList.Clear();
                    }
                    else {
                        // Target data file differs from source file. Optimization will be performed.
                        try {
                            Stream dataStream = File.Open(mulFile, FileMode.Create, FileAccess.Write, FileShare.None);
                            IndexFile newIndexFile = new IndexFile();

                            for (int i = 0; i < indexFile.Count; i++) {
                                if (changeList.ContainsKey(i)) {
                                    IndexData writtenData;

                                    if (i < 16384)
                                        writtenData = Art.WriteTile(dataStream, changeList[i]);
                                    else
                                        writtenData = Art.WriteRun(dataStream, changeList[i]);

                                    newIndexFile.Set(i, writtenData, true);
                                }
                                else {
                                    IndexData indexData = indexFile.Get(i, false);
                                    if (indexData.IsValid) {
                                        byte[] data = new byte[indexData.Lenght];

                                        this.dataStream.Seek(indexData.Lookup, SeekOrigin.Begin);
                                        this.dataStream.Read(data, 0, (int)indexData.Lenght);

                                        IndexData writtenData = new IndexData();
                                        writtenData.Lookup = (uint)dataStream.Position;
                                        writtenData.Lenght = indexData.Lenght;
                                        writtenData.Extra = indexData.Extra;

                                        dataStream.Write(data, 0, (int)writtenData.Lenght);
                                    }
                                }
                            }

                            dataStream.Flush();
                            Trace.WriteLine(String.Format("Art: File \"{0}\" succesfully saved.", mulFile), "MulLib");

                            newIndexFile.Save(indexStream);
                            Trace.WriteLine(String.Format("IndexFile: File \"{0}\" succesfully saved.", idxFile), "MulLib");
                        }
                        finally {
                            if (dataStream != null)
                                dataStream.Close();
                        }
                    }
                }
                catch (Exception e) {
                    throw new Exception("Error saving Art.", e);
                }
                finally {
                    if (indexStream != null)
                        indexStream.Close();
                }
            }
        }

        /// <summary>
        /// Loads Art data from files.
        /// </summary>
        /// <param name="idxFile">Path to index file.</param>
        /// <param name="mulFile">Path to data file.</param>
        /// <param name="access">Access mode to data file.</param>
        /// <returns>New object encapsulating loaded data.</returns>
        public static Art Load(string idxFile, string mulFile, MulFileAccessMode access)
        {
            Stream dataStream = null;

            try {
                switch (access) {
                    case MulFileAccessMode.RequestWrite:
                        dataStream = File.Open(mulFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                        break;

                    case MulFileAccessMode.TryWrite:
                        try {
                            dataStream = File.Open(mulFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                        }
                        catch {
                            goto case MulFileAccessMode.ReadOnly;
                        }
                        break;

                    case MulFileAccessMode.ReadOnly:
                        dataStream = File.Open(mulFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
                        break;
                }

                IndexFile indexFile = IndexFile.Load(idxFile);

                Art art = new Art(Path.GetFullPath(mulFile), indexFile, dataStream);

                Trace.WriteLine(String.Format("Art: File \"{0}\" succesfully loaded.", mulFile), "MulLib");
                return art;
            }
            catch (Exception e) {
                if (dataStream != null)
                    dataStream.Close();

                throw new Exception("Error loading Art.", e);
            }
        }

        #region Static read and write functions

        /// <summary>
        /// Reads a TileID texture (44x44) from current position in specified file.
        /// </summary>
        /// <param name="stream">Stream where to read from.</param>
        /// <returns>New instance of Bitmap containing read texture.</returns>
        public static Bitmap ReadTile(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            Bitmap bitmap = new Bitmap(44, 44, PixelFormat.Format16bppArgb1555);

            BitmapData data = bitmap.LockBits(Ultima.GetBitmapBounds(bitmap), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);
            byte[] buffer = new byte[44 * 2];

            for (int y = 0; y < 22; y++) {
                int x = 22 - (y + 1);
                int lenght = (y + 1) * 2;

                stream.Read(buffer, 0, lenght * 2);

                for (int i = 0; i < lenght; i++) {
                    buffer[i * 2 + 1] |= 0x80;
                }

                Marshal.Copy(buffer, 0, (IntPtr)((int)data.Scan0 + y * data.Stride + x * 2), lenght * 2);
            }

            for (int y = 22; y < 44; y++) {
                int x = y - 22;
                int lenght = (44 - y) * 2;

                stream.Read(buffer, 0, lenght * 2);

                for (int i = 0; i < lenght; i++) {
                    buffer[i * 2 + 1] |= 0x80;
                }

                Marshal.Copy(buffer, 0, (IntPtr)((int)data.Scan0 + y * data.Stride + x * 2), lenght * 2);
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }

        /// <summary>
        /// Writes a Tile bitmap (44x44) into specified file and returns structure with indexing information.
        /// </summary>
        /// <param name="stream">Stream where to write to.</param>
        /// <param name="bitmap">Bitmap to write. If null, returns invalid IndexData.</param>
        /// <returns>IndexData structure with indexing information.</returns>
        /// <remarks>It writes only diamond from source image, other is ignored.</remarks>
        public static IndexData WriteTile(Stream stream, Bitmap bitmap)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (bitmap == null)
                return IndexData.Empty;

            // Check bitmap
            if (bitmap.Height != 44 || bitmap.Width != 44)
                throw new ArgumentException("Bitmap has incorrect size. Only allowed is 44x44.", "bitmap");

            // Create index list
            IndexData indexdata = IndexData.Empty;
            indexdata.Lookup = (uint)stream.Position;
            indexdata.Extra = 0;

            BinaryWriter writer = new BinaryWriter(stream);

            for (int y = 0; y < 22; y++) {
                int x = 22 - (y + 1);
                int lenght = (y + 1) * 2;
                for (int i = 0; i < lenght; i++) {
                    writer.Write(UOColorConverter.FromColor(bitmap.GetPixel(x + i, y)));
                }
            }
            for (int y = 22; y < 44; y++) {
                int x = y - 22;
                int lenght = (44 - y) * 2;
                for (int i = 0; i < lenght; i++) {
                    writer.Write(UOColorConverter.FromColor(bitmap.GetPixel(x + i, y)));
                }
            }

            indexdata.Lenght = (uint)stream.Position - indexdata.Lookup;

            return indexdata;
        }

        /// <summary>
        /// Reads a run-type texture from current position in specified file.
        /// </summary>
        /// <param name="stream">Stream where to read from.</param>
        /// <returns>New instance of Bitmap containing read texture.</returns>
        public static Bitmap ReadRun(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            BinaryReader reader = new BinaryReader(stream);

            int header = reader.ReadInt32();
            int width = reader.ReadInt16();
            int height = reader.ReadInt16();

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
            BitmapData data = bitmap.LockBits(Ultima.GetBitmapBounds(bitmap), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

            byte[] buffer = new byte[4096];

            // Read lookup table
            ushort[] lookupTable = new ushort[height];
            for (int i = 0; i < height; i++) {
                lookupTable[i] = reader.ReadUInt16();
            }

            // list begining in file
            long dataPos = stream.Position;

            int y = 0;
            while (y < height) {
                // New line
                stream.Seek(dataPos + lookupTable[y] * 2, SeekOrigin.Begin);

                // ReadLine
                short xOffset;
                short xRun;
                int x = 0;
                do {
                    xOffset = reader.ReadInt16();
                    xRun = reader.ReadInt16();

                    if (xOffset + xRun >= 2048)
                        throw new IOException("Corrupted list.");

                    // Read current chunk
                    x += xOffset;

                    stream.Read(buffer, 0, xRun * 2);

                    for (int i = 0; i < xRun; i++) {
                        buffer[i * 2 + 1] |= 0x80;
                    }

                    Marshal.Copy(buffer, 0, (IntPtr)((int)data.Scan0 + y * data.Stride + x * 2), xRun * 2);
                    x += xRun;
                } while (xRun != 0 || xOffset != 0);

                y++;
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }

        /// <summary>
        /// Writes a run-compressed bitmap into specified file and returns structure with indexing information.
        /// </summary>
        /// <param name="stream">Stream where to write to.</param>
        /// <param name="bitmap">Bitmap to write. If null, returns invalid IndexData.</param>
        /// <returns>IndexData structure with indexing information.</returns>
        public static IndexData WriteRun(Stream stream, Bitmap bitmap)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (bitmap == null)
                return IndexData.Empty;

            BinaryWriter writer = new BinaryWriter(stream);

            IndexData indexdata = IndexData.Empty;
            indexdata.Lookup = (uint)writer.BaseStream.Position;
            indexdata.Extra = 0;

            short width = (short)bitmap.Width;
            short height = (short)bitmap.Height;

            writer.Write((int)0); // header, unknown
            writer.Write(width);
            writer.Write(height);

            long lookupTable = writer.BaseStream.Position;
            int dataStart = (int)writer.BaseStream.Position + height * 2;

            // Skip lookup table
            if (writer.BaseStream.Length < dataStart) {
                writer.BaseStream.SetLength(dataStart);
            }
            writer.Seek(dataStart, SeekOrigin.Begin);

            short y = 0;
            while (y < height) {
                // Write record to lookup table
                int pos = (int)writer.BaseStream.Position;

                writer.Seek((int)lookupTable, SeekOrigin.Begin);
                writer.Write((short)((pos - dataStart) / 2));
                lookupTable = writer.BaseStream.Position;

                writer.Seek(pos, SeekOrigin.Begin);

                // Write line
                int x = 0;
                while (x < width) {
                    short xOffset = 0;
                    short xRun = 0;

                    // Skip invisible pixels
                    while (x < width && bitmap.GetPixel(x, y).A == 0) {
                        xOffset++;
                        x++;
                    }

                    if (x == width) break;

                    // Read list
                    int xPos = x;
                    while (xPos < width && bitmap.GetPixel(xPos, y).A != 0) {
                        xPos++;
                        xRun++;
                    }

                    // Write chunk header
                    writer.Write(xOffset);
                    writer.Write(xRun);

                    // Write pixel list
                    while (x < width && bitmap.GetPixel(x, y).A != 0) {
                        writer.Write(UOColorConverter.FromColor(bitmap.GetPixel(x, y)));
                        x++;
                    }
                }

                // End line
                writer.Write((short)0);
                writer.Write((short)0);

                y++;
            }

            indexdata.Lenght = (uint)writer.BaseStream.Position - indexdata.Lookup;

            return indexdata;
        }

        #endregion
    }
}
