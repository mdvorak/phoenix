using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Phoenix.Gui.Editor
{
    enum FileSystemObjectType
    {
        File,
        Directory
    }

    class FileSystemObject
    {
        private FileSystemInfo info;

        public FileSystemObject(FileInfo path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            info = path;
        }

        public FileSystemObject(DirectoryInfo path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            info = path;
        }

        public FileSystemInfo Info
        {
            get { return info; }
        }

        public FileSystemObjectType Type
        {
            get { return info.GetType() == typeof(FileInfo) ? FileSystemObjectType.File : FileSystemObjectType.Directory; }
        }

        public DirectoryInfo Parent
        {
            get
            {
                if (info.GetType() == typeof(FileInfo))
                {
                    return ((FileInfo)info).Directory;
                }
                else
                {
                    return ((DirectoryInfo)info).Parent;
                }
            }
        }

        public void MoveTo(string path)
        {
            try
            {
                if (info.GetType() == typeof(FileInfo))
                {
                    ((FileInfo)info).MoveTo(path);
                }
                else
                {
                    ((DirectoryInfo)info).MoveTo(path);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Error");
            }
        }

        public void CopyTo(string path)
        {
            try
            {
                if (info.GetType() == typeof(FileInfo))
                {
                    ((FileInfo)info).CopyTo(path, true);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Not supported yet.");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString(), "FileSystemObject");
                System.Windows.Forms.MessageBox.Show(e.Message, "Error");
            }
        }

        public void Rename(string name)
        {
            string newPath = Path.Combine(Parent.FullName, name);

            if (String.Compare(info.FullName, newPath, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                MoveTo(newPath);
            }
        }

        public void Delete()
        {
            try
            {
                if (info.GetType() == typeof(FileInfo))
                {
                    ((FileInfo)info).Delete();
                }
                else
                {
                    ((DirectoryInfo)info).Delete(true);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString(), "FileSystemObject");
                System.Windows.Forms.MessageBox.Show(e.Message, "Error");
            }
        }

        public static explicit operator FileSystemObject(FileInfo path)
        {
            if (path != null)
                return new FileSystemObject(path);
            else
                return null;
        }

        public static explicit operator FileSystemObject(DirectoryInfo path)
        {
            if (path != null)
                return new FileSystemObject(path);
            else
                return null;
        }
    }
}