using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Phoenix.Utils;

namespace Phoenix.Logging
{
    public static class Log
    {
        public const int MaxFileIndex = 32;
        private static TimerEx flushTimer;

        static Log()
        {
            flushTimer = new TimerEx(-1);
            flushTimer.Callback += new EventHandler(flushTimer_Callback);
        }

        static void flushTimer_Callback(object sender, EventArgs e)
        {
            Trace.Flush();
        }

        /// <summary>
        /// Gets or sets auto flush interval. Use -1 to disable it. Initially is autoflush disabled.
        /// </summary>
        public static int AutoFlushInterval
        {
            get { return flushTimer.Interval; }
            set
            {
                flushTimer.Interval = value;

                if (value < 0)
                    flushTimer.Stop();
                else if (!flushTimer.Running)
                    flushTimer.Start();
            }
        }

        public static void CompressFile(string source, string dest)
        {
            FileStream sourceStream = null;

            try
            {
                sourceStream = File.OpenRead(source);
                CompressFile(sourceStream, dest);
            }
            finally
            {
                if (sourceStream != null)
                    sourceStream.Close();
            }
        }

        private static void CompressFile(FileStream sourceStream, string dest)
        {
            FileStream destStream = null;
            GZipStream gzipStream = null;

            try
            {
                sourceStream.Seek(0, SeekOrigin.Begin);
                destStream = File.OpenWrite(dest);
                gzipStream = new GZipStream(destStream, CompressionMode.Compress);

                byte[] buffer = new byte[1024 * 64];

                while (sourceStream.Position < sourceStream.Length)
                {
                    int remainingBytes = (int)(sourceStream.Length - sourceStream.Position);
                    int readBytes = sourceStream.Read(buffer, 0, Math.Min(buffer.Length, remainingBytes));

                    gzipStream.Write(buffer, 0, readBytes);
                }
            }
            finally
            {
                if (gzipStream != null)
                    gzipStream.Close();

                if (destStream != null)
                    destStream.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Default path to log file.</param>
        /// <param name="autoAddIndex">If true index is added to filename when it cant be opened.</param>
        /// <param name="archivePath">
        /// When non-null and file size is greater then maxSize,
        /// file is compressed to this file and original is deleted.
        /// </param>
        /// <param name="maxSize">
        /// Maximum size of file before it is archived, in bytes.
        /// If archivePath is null, parameter is ingored.
        /// If it is -1, no archiving is done either.
        /// </param>
        /// <returns>Stream containing opened file.</returns>
        public static Stream OpenFile(string path, bool autoAddIndex, string archivePath, int maxSize)
        {
            FileInfo file = new FileInfo(path);
            string directory = file.Directory.FullName;
            string filename = Path.GetFileNameWithoutExtension(path);
            string extension = file.Extension;
            int index = 0;

            Exception lastException = null;

            do
            {
                try
                {
                    if (file.Exists && archivePath != null && archivePath.Length > 0 && maxSize >= 0 && file.Length > maxSize)
                    {
                        try
                        {
                            CompressFile(file.FullName, archivePath);
                            file.Delete();
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine("Unable to compress file. Exception:\n" + e.ToString(), "Logging");
                        }
                    }

                    return file.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
                }
                catch (Exception e)
                {
                    if (autoAddIndex)
                        lastException = e;
                    else
                        throw e;
                }

                file = new FileInfo(Path.Combine(directory, filename + index.ToString() + extension));
                index++;
            } while (index < MaxFileIndex);

            Debug.Assert(lastException != null);
            throw lastException;
        }
    }
}
