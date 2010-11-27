using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Phoenix.Configuration
{
    public class SettingsLoader : IEnumerable
    {
        #region Enumerator class

        class Enumerator : IEnumerator
        {
            private DirectoryInfo dir;
            private FileInfo[] files;
            private int index;

            public Enumerator(DirectoryInfo directory)
            {
                dir = directory;
                files = directory.GetFiles("*.xml", SearchOption.AllDirectories);
                index = -1;
            }

            #region IEnumerator Members

            public object Current
            {
                get
                {
                    string filename = files[index].FullName.Remove(0, dir.FullName.Length + 1);

                    if (filename.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                        filename = filename.Remove(filename.Length - 4, 4);

                    return filename;
                }
            }

            public bool MoveNext()
            {
                index++;
                return index < files.Length;
            }

            public void Reset()
            {
                index = -1;
            }

            #endregion
        }

        #endregion

        private DirectoryInfo directory;

        public SettingsLoader(string folder)
        {
            Folder = folder;
        }

        public string Folder
        {
            get { return directory.FullName; }
            set
            {
                directory = new DirectoryInfo(value);
                if (!directory.Exists) directory.Create();
            }
        }

        public string GetProfilePath(string name)
        {
            if (!name.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                name += ".xml";

            return Path.Combine(directory.FullName, name);
        }

        public SynchronizedSettings Load(string name, string rootname)
        {
            if (!name.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                name += ".xml";

            string path = Path.Combine(directory.FullName, name);

            SynchronizedSettings settings = new SynchronizedSettings(rootname);
            settings.Path = path;
            settings.Load();
            return settings;
        }

        public string[] Enumerate()
        {
            List<string> files = new List<string>();

            foreach (string file in this)
                files.Add(file);

            return files.ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(directory);
        }
    }
}
