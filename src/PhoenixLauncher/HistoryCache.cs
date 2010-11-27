using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace PhoenixLauncher
{
    static class HistoryCache
    {
        private static Dictionary<String, List<String>> cache = new Dictionary<string, List<string>>();

        public static void ClearCache()
        {
            cache.Clear();
        }

        public static void Add(string cls, string value)
        {
            if (cls == null || cls.Length == 0 || value == null || value.Length == 0)
                return;

            List<String> classList;

            if (!cache.TryGetValue(cls, out classList))
            {
                classList = new List<string>();
                cache[cls] = classList;
            }

            if (!classList.Contains(value))
            {
                classList.Add(value);
            }
        }

        public static string[] GetPosibilities(string cls)
        {
            List<String> classList;

            if (cache.TryGetValue(cls, out classList))
            {
                return classList.ToArray();
            }
            else
            {
                return new string[0];
            }
        }

        public static void Load(string file)
        {
            Stream stream = null;

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                stream = File.OpenRead(file);
                cache = (Dictionary<String, List<String>>)formatter.Deserialize(stream);

                Trace.TraceInformation("HistoryCache from file {0} loaded.", file);
            }
            catch (Exception e)
            {
                Trace.TraceWarning("Error loading HistoryCache from file {0}. Exception: {1}", file, e.Message);
            }
            finally
            {
                if (stream != null) stream.Close();
            }
        }

        public static void Save(string file)
        {
            Stream stream = null;

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                stream = File.OpenWrite(file);
                formatter.Serialize(stream, cache);

                Trace.TraceInformation("HistoryCache saved to file {0}.", file);
            }
            catch (Exception e)
            {
                Trace.TraceWarning("Error saving HistoryCache to file {0}. Exception: {1}", file, e.Message);
            }
            finally
            {
                if (stream != null) stream.Close();
            }
        }
    }
}
