using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace PhoenixLauncher
{
    public struct UOKey
    {
        public string Key1;
        public string Key2;
        public int GameEncryption;
    }

    public class UOKeysLoader
    {
        private Dictionary<String, UOKey> list = new Dictionary<string, UOKey>();

        public UOKeysLoader()
        {
        }

        public UOKeysLoader(string path)
        {
            Load(path);
        }

        public bool Load(string path)
        {
            StreamReader reader = null;

            try
            {
                reader = new StreamReader(path);

                list.Clear();

                Regex regex = new Regex(@"\s*""(?<Name>.+)""\s+(?<Key1>\w+)\s+(?<Key2>\w+)\s+(?<GameEnc>\d)", RegexOptions.Compiled);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (line.Contains("#"))
                        line = line.Remove(line.IndexOf('#')).Trim();

                    if (line.Length > 0)
                    {
                        Match match = regex.Match(line);

                        if (match.Success)
                        {
                            string name = match.Groups["Name"].Value;

                            UOKey key = new UOKey();
                            key.Key1 = match.Groups["Key1"].Value;
                            key.Key2 = match.Groups["Key2"].Value;
                            key.GameEncryption = Int32.Parse(match.Groups["GameEnc"].Value);

                            list.Add(name, key);
                        }
                    }
                }

                Trace.TraceInformation("Loaded {0} keys.", list.Count);

                return list.Count > 0;
            }
            catch (Exception e)
            {
                Trace.TraceError("Error loading UOKeys. Exception: {0}", e.Message);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        public Dictionary<String, UOKey> List
        {
            get { return list; }
        }
    }
}
