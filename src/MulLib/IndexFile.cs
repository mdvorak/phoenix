using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MulLib
{
    /// <summary>
    /// Structure that holds index informations.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct IndexData
    {
        /// <summary>
        /// Unmanaged size of IndexData.
        /// </summary>
        public const int Size = 12;

        /// <summary>
        /// Position where to look in mul file for actual data.
        /// </summary>
        public uint Lookup;
        /// <summary>
        /// Size of data.
        /// </summary>
        public uint Lenght;
        /// <summary>
        /// Some extra information dependend on mul format.
        /// </summary>
        public uint Extra;

        /// <summary>
        /// Gets wheter this index is valid.
        /// </summary>
        public bool IsValid
        {
            get { return Lookup != UInt32.MaxValue; }
        }

        /// <summary>
        /// Returns IndexData object with invalid lookup.
        /// </summary>
        public static IndexData Empty
        {
            get
            {
                IndexData data = new IndexData();
                data.Lookup = 0xFFFFFFFF;
                return data;
            }
        }
    }

    /// <summary>
    /// List of indicies for some mul files.
    /// </summary>
    /// <remarks>This class is not thread-safe.</remarks>
    public class IndexFile : IDisposable
    {
        private List<IndexData> list;

        /// <summary>
        /// Initializes new empty object.
        /// </summary>
        public IndexFile()
        {
            list = new List<IndexData>();
        }

        /// <summary>
        /// Gets current count of indexes.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public int Count
        {
            get
            {
                if (Disposed)
                    throw new ObjectDisposedException("IndexFile");

                return list.Count;
            }
        }

        /// <summary>
        /// Gets or sets data at specified index.
        /// </summary>
        /// <param name="index">Index of data.</param>
        /// <returns>Data at specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when index is less than zero or equal to or greater than Count.</exception>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public IndexData this[int index]
        {
            get
            {
                if (Disposed)
                    throw new ObjectDisposedException("IndexFile");

                return list[index];
            }
            set
            {
                if (Disposed)
                    throw new ObjectDisposedException("IndexFile");

                list[index] = value;
            }
        }

        /// <summary>
        /// Returns data at specified index.
        /// </summary>
        /// <param name="index">Index of data.</param>
        /// <param name="create">If true list is automaticly resized; otherwise exception is thrown.</param>
        /// <returns>Data at specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when create is false and index is less than zero or equal to or greater than Count.</exception>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public IndexData Get(int index, bool create)
        {
            if (Disposed)
                throw new ObjectDisposedException("IndexFile");

            if (index > list.Count && create)
            {
                Resize(index);
            }

            return list[index];
        }

        /// <summary>
        /// Sets data at specified index.
        /// </summary>
        /// <param name="index">Index of data.</param>
        /// <param name="data">New data.</param>
        /// <param name="create">If true list is automaticly resized; otherwise exception is thrown.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when create is false and index is less than zero or equal to or greater than Count.</exception>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void Set(int index, IndexData data, bool create)
        {
            if (Disposed)
                throw new ObjectDisposedException("IndexFile");

            if (index > list.Count && create)
            {
                Resize(index);
            }

            list[index] = data;
        }

        /// <summary>
        /// Adds data to the end of the list.
        /// </summary>
        /// <param name="data">New data.</param>
        /// <returns>Index of written data.</returns>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public int Add(IndexData data)
        {
            if (Disposed)
                throw new ObjectDisposedException("IndexFile");

            list.Add(data);

            return list.Count - 1;
        }

        /// <summary>
        /// Sets data at specified index to IndexData.Empty.
        /// </summary>
        /// <param name="index">Index of data.</param>
        /// <param name="create">If true list is automaticly resized; otherwise exception is thrown.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when create is false and index is less than zero or equal to or greater than Count.</exception>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void Reset(int index, bool create)
        {
            if (Disposed)
                throw new ObjectDisposedException("IndexFile");

            if (index > list.Count && create)
            {
                Resize(index);
            }

            list[index] = IndexData.Empty;
        }

        /// <summary>
        /// Resizes list. All new values are set to IndexData.Empty.
        /// </summary>
        /// <param name="indexCount">New list size.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when indexCount is less than zero.</exception>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void Resize(int indexCount)
        {
            if (Disposed)
                throw new ObjectDisposedException("IndexFile");

            if (indexCount < 0)
                throw new ArgumentOutOfRangeException("indexCount", "List size cannot be less than zero.");

            while (list.Count < indexCount)
            {
                list.Add(IndexData.Empty);
            }

            if (list.Count > indexCount)
            {
                list.RemoveRange(indexCount, list.Count - indexCount);
            }
        }

        /// <summary>
        /// Returns True if object has been disposed; otherwise false.
        /// </summary>
        public bool Disposed
        {
            get { return list == null; }
        }

        /// <summary>
        /// Frees all resources holded by this object.
        /// </summary>
        public void Dispose()
        {
            if (list != null)
            {
                list.Clear();
                list = null;
            }
        }

        /// <summary>
        /// Stores this object to specified file.
        /// </summary>
        /// <param name="file">File path.</param>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void Save(string file)
        {
            if (Disposed)
                throw new ObjectDisposedException("IndexFile");

            Stream stream = null;

            try
            {
                stream = File.OpenWrite(file);

                Save(stream);

                Trace.WriteLine(String.Format("IndexFile: File \"{0}\" succesfully saved.", file), "MulLib");
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Stores this object to specified stream. Stream is truncated.
        /// </summary>
        /// <param name="stream">Target stream.</param>
        /// <remarks>Intended only for internal use, so use with care.</remarks>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void Save(Stream stream)
        {
            if (Disposed)
                throw new ObjectDisposedException("IndexFile");

            BinaryWriter writer = null;

            try
            {
                writer = new BinaryWriter(stream);

                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(list.Count * IndexData.Size);

                foreach (IndexData data in list)
                {
                    writer.Write(data.Lookup);
                    writer.Write(data.Lenght);
                    writer.Write(data.Extra);
                }

                stream.Flush();
            }
            catch (Exception e)
            {
                throw new Exception("Error saving IndexFile.", e);
            }
        }

        /// <summary>
        /// Loads IndexFile object from file.
        /// </summary>
        /// <param name="file">File path.</param>
        public static IndexFile Load(string file)
        {
            Stream stream = null;
            BinaryReader reader = null;

            try
            {
                stream = File.OpenRead(file);
                reader = new BinaryReader(stream);

                Trace.WriteLineIf((stream.Length % IndexData.Size) != 0, "IndexFile: File size is not multiple of 12.", "MulLib");

                IndexFile indexFile = new IndexFile();

                IndexData data = new IndexData();
                for (int i = 0; i < stream.Length / IndexData.Size; i++)
                {
                    data.Lookup = reader.ReadUInt32();
                    data.Lenght = reader.ReadUInt32();
                    data.Extra = reader.ReadUInt32();

                    indexFile.list.Add(data);
                }

                Trace.WriteLine(String.Format("IndexFile: File \"{0}\" succesfully loaded.", file), "MulLib");
                return indexFile;
            }
            catch (Exception e)
            {
                throw new Exception("Error loading IndexFile.", e);
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
