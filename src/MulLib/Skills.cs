using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MulLib
{
    /// <summary>
    /// Skill data.
    /// </summary>
    public struct SkillData
    {
        /// <summary>
        /// Specify if skill has shown button.
        /// </summary>
        public bool Action;
        /// <summary>
        /// Some extra information about skill, use unknown.
        /// </summary>
        public uint Extra;
        /// <summary>
        /// Name of the skill.
        /// </summary>
        public string Name;
    }

    /// <summary>
    /// List of skills.
    /// </summary>
    public class Skills : IEnumerable<SkillData>, IDisposable
    {
        private readonly object syncRoot;
        private List<SkillData> list;

        /// <summary>
        /// Initializes new empty Skills object.
        /// </summary>
        public Skills()
        {
            syncRoot = new object();
            list = new List<SkillData>(64);
        }

        /// <summary>
        /// Gets synchronization object.
        /// </summary>
        public object SyncRoot
        {
            get { return syncRoot; }
        } 

        /// <summary>
        /// Gets count of skills.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public int Count
        {
            get
            {
                lock (syncRoot)
                {
                    if (Disposed)
                        throw new ObjectDisposedException("Skills");

                    return list.Count;
                }
            }
        }

        /// <summary>
        /// Gets or sets skill data of specified index.
        /// </summary>
        /// <param name="index">Skill index.</param>
        /// <returns>Skill data object.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when index is less than zero or equal to or greater than Count.</exception>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public SkillData this[int index]
        {
            get
            {
                lock (syncRoot)
                {
                    if (Disposed)
                        throw new ObjectDisposedException("Skills");

                    return list[index];
                }
            }
            set
            {
                lock (syncRoot)
                {
                    if (Disposed)
                        throw new ObjectDisposedException("Skills");

                    list[index] = value;
                }
            }
        }

        /// <summary>
        /// Adds new skill to the end of the list.
        /// </summary>
        /// <param name="data">New skill data.</param>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void Add(SkillData data)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("Skills");

                list.Add(data);
            }
        }

        /// <summary>
        /// Removes last skill at the list.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void RemoveLast()
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("Skills");

                list.RemoveAt(list.Count - 1);
            }
        }

        /// <summary>
        /// Returns index of specified data or -1 if not found.
        /// </summary>
        /// <param name="data">Searched data.</param>
        /// <returns>Index of skill in list or -1.</returns>
        public int IndexOf(SkillData data)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("Skills");

                return list.IndexOf(data);
            }
        }

        /// <summary>
        /// Returns index of skill with specified name or -1 if not found. Comparsion is case sensitive.
        /// </summary>
        /// <param name="skillName">Searched skill name.</param>
        /// <returns>Index of skill in list or -1.</returns>
        public int IndexOf(string skillName)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("Skills");

                for (int i = 0; i < list.Count; i++)
                {
                    if (skillName == list[i].Name)
                    {
                        return i;
                    }
                }

                return -1;
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
            lock (syncRoot)
            {
                if (list != null)
                {
                    list.Clear();
                    list = null;
                }
            }
        }

        /// <summary>
        /// Gets skills enumerator.
        /// </summary>
        /// <returns>IEnumerator implementation.</returns>
        public IEnumerator<SkillData> GetEnumerator()
        {
            lock (syncRoot)
            {
                return list.GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (syncRoot)
            {
                return list.GetEnumerator();
            }
        }

        /// <summary>
        /// Stores this object to specified files.
        /// </summary>
        /// <param name="idxFile">Index file.</param>
        /// <param name="mulFile">Data file.</param>
        /// <exception cref="System.ObjectDisposedException">Object has been disposed.</exception>
        public void Save(string idxFile, string mulFile)
        {
            lock (syncRoot)
            {
                if (Disposed)
                    throw new ObjectDisposedException("Skills");

                IndexFile indexFile = null;
                Stream stream = null;
                BinaryWriter writer = null;
                Stream indexStream = null;

                try
                {
                    indexStream = File.Open(idxFile, FileMode.Create, FileAccess.Write, FileShare.None);

                    indexFile = new IndexFile();
                    stream = File.OpenWrite(mulFile);
                    writer = new BinaryWriter(stream);

                    IndexData indexData = new IndexData();

                    foreach (SkillData skillData in list)
                    {
                        indexData.Lookup = (uint)stream.Position;
                        indexData.Lenght = (uint)skillData.Name.Length + 1;
                        indexData.Extra = skillData.Extra;

                        writer.Write(skillData.Action);
                        writer.Write(Encoding.ASCII.GetBytes(skillData.Name));

                        indexFile.Add(indexData);
                    }

                    indexFile.Resize(256);
                    indexFile.Save(indexStream);
                    Trace.WriteLine(String.Format("IndexFile: File \"{0}\" succesfully saved.", idxFile), "MulLib");

                    writer.Flush();

                    Trace.WriteLine(String.Format("Skills: File \"{0}\" succesfully saved.", mulFile), "MulLib");
                }
                catch (Exception e)
                {
                    throw new Exception("Error saving Skills.", e);
                }
                finally
                {
                    if (indexFile != null)
                        indexFile.Dispose();

                    if (indexStream != null)
                        indexStream.Close();

                    if (writer != null)
                        writer.Close();

                    if (stream != null)
                        stream.Close();
                }
            }
        }

        /// <summary>
        /// Loads skills from files.
        /// </summary>
        /// <param name="idxFile">Index file.</param>
        /// <param name="mulFile">Data file.</param>
        /// <returns>New Skills object with loaded data.</returns>
        public static Skills Load(string idxFile, string mulFile)
        {
            IndexFile indexFile = null;
            Stream stream = null;
            BinaryReader reader = null;

            try
            {
                indexFile = IndexFile.Load(idxFile);

                stream = File.OpenRead(mulFile);
                reader = new BinaryReader(stream);

                Skills skills = new Skills();

                IndexData indexData;
                SkillData skillData = new SkillData();

                for (int i = 0; i < indexFile.Count; i++)
                {
                    indexData = indexFile[i];

                    if (indexData.IsValid)
                    {
                        stream.Seek(indexData.Lookup, SeekOrigin.Begin);

                        skillData.Action = reader.ReadByte() != 0;
                        skillData.Extra = indexData.Extra;

                        byte[] nameBytes = reader.ReadBytes((int)indexData.Lenght - 1);
                        skillData.Name = Encoding.ASCII.GetString(nameBytes).TrimEnd(' ', '\0');

                        skills.list.Add(skillData);
                    }
                    else
                    {
                        Trace.WriteLine("Skills: Found " + i.ToString() + " skills.", "MulLib");
                        break;
                    }
                }

                Trace.WriteLine(String.Format("Skills: File \"{0}\" succesfully loaded.", mulFile), "MulLib");
                return skills;
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Skills.", e);
            }
            finally
            {
                if (indexFile != null)
                    indexFile.Dispose();

                if (reader != null)
                    reader.Close();

                if (stream != null)
                    stream.Close();
            }
        }
    }
}
