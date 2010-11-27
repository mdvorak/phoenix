using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MulLib
{
    /// <summary>
    /// List of colors used in minimap.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// For tiles without statics, use map tile id. Otherwise use first static id + 16384.
    /// </remarks>
    public class RadarCol : IDisposable
    {
        /// <summary>
        /// Constant count of colors.
        /// </summary>
        public const int MaxIndex = 0x10000;

        private readonly object syncRoot;
        private ushort[] data;

        /// <summary>
        /// Constructs empty RadarCol object.
        /// </summary>
        public RadarCol()
        {
            syncRoot = new object();
            data = new ushort[MaxIndex];
        }

        /// <summary>
        /// Gets synchronization object.
        /// </summary>
        public object SyncRoot
        {
            get { return syncRoot; }
        } 

        /// <summary>
        /// Gets count of colors.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public int Count
        {
            get
            {
                if (Disposed)
                    throw new ObjectDisposedException("RadarCol");

                return data.Length;
            }
        }

        /// <summary>
        /// Gets or sets color of specified id.
        /// </summary>
        /// <param name="index">Radar color id.</param>
        /// <returns>Color in X1R5G5B5 format.</returns>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public ushort this[int index]
        {
            get
            {
                lock (syncRoot)
                {
                    if (Disposed)
                        throw new ObjectDisposedException("RadarCol");

                    return data[index];
                }
            }
            set
            {
                lock (syncRoot)
                {
                    if (Disposed)
                        throw new ObjectDisposedException("RadarCol");

                    data[index] = value;
                }
            }
        }

        /// <summary>
        /// Returns True if object has been disposed; otherwise false.
        /// </summary>
        public bool Disposed
        {
            get { return data == null; }
        }

        /// <summary>
        /// Frees all resources holded by this object.
        /// </summary>
        public void Dispose()
        {
            lock (syncRoot)
            {
                data = null;
            }
        }

        /// <summary>
        /// Stores this object to specified file in RadarCol.mul format.
        /// </summary>
        /// <param name="file">File path.</param>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void Save(string file)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("RadarCol");

                Stream stream = null;
                BinaryWriter writer = null;

                try
                {
                    stream = File.OpenWrite(file);
                    writer = new BinaryWriter(stream);

                    stream.SetLength(MaxIndex * 2);

                    for (int i = 0; i < data.Length; i++)
                    {
                        writer.Write(data[i]);
                    }

                    Trace.WriteLine(String.Format("RadarCol: File \"{0}\" succesfully saved.", file), "MulLib");
                }
                catch (Exception e)
                {
                    throw new Exception("Error saving RadarCol.", e);
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
        /// Loads RadarCol object from file in RadarCol.mul format.
        /// </summary>
        /// <param name="file">File path.</param>
        public static RadarCol Load(string file)
        {
            Stream stream = null;
            BinaryReader reader = null;

            try
            {
                stream = File.OpenRead(file);
                reader = new BinaryReader(stream);

                if (stream.Length != MaxIndex * 2)
                    throw new Exception("Invalid file size. Expected: 131072 bytes.");

                RadarCol radarCol = new RadarCol();

                for (int i = 0; i < radarCol.data.Length; i++)
                {
                    radarCol.data[i] = reader.ReadUInt16();
                }

                Trace.WriteLine(String.Format("RadarCol: File \"{0}\" succesfully loaded.", file), "MulLib");
                return radarCol;
            }
            catch (Exception e)
            {
                throw new Exception("Error loading RadarCol.", e);
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
