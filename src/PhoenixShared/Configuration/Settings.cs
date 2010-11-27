using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Phoenix.Configuration
{
    #region AttributeInfo and ElementInfo class

    public struct AttributeInfo
    {
        public string Name;
        public object Value;

        public AttributeInfo(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return String.Format("\"{0}\"=\"{1}\"", Name, Value);
        }

        public static explicit operator KeyValuePair<String, Object>(AttributeInfo info)
        {
            return new KeyValuePair<string, object>(info.Name, info.Value);
        }

        public static explicit operator AttributeInfo(KeyValuePair<String, Object> pair)
        {
            return new AttributeInfo(pair.Key, pair.Value);
        }
    }

    /// <summary>
    /// Specifies more detailed element info.
    /// </summary>
    public class ElementInfo
    {
        #region AttributeInfoCollection

        public class AttributeInfoCollection : ICollection<AttributeInfo>
        {
            private class Enumerator : IEnumerator<AttributeInfo>
            {
                private IEnumerator<KeyValuePair<String, Object>> enumerator;

                public Enumerator(IDictionary<String, Object> dictionary)
                {
                    enumerator = dictionary.GetEnumerator();
                }

                public AttributeInfo Current
                {
                    get { return (AttributeInfo)enumerator.Current; }
                }

                public void Dispose()
                {
                    enumerator.Dispose();
                }

                object IEnumerator.Current
                {
                    get { return (AttributeInfo)enumerator.Current; }
                }

                public bool MoveNext()
                {
                    return enumerator.MoveNext();
                }

                public void Reset()
                {
                    enumerator.Reset();
                }
            }

            private Dictionary<String, Object> list;

            internal AttributeInfoCollection()
            {
                list = new Dictionary<string, object>();
            }

            public void Add(string key, object value)
            {
                list.Add(key, value);
            }

            public bool Contains(string key)
            {
                return list.ContainsKey(key);
            }

            public bool Remove(string key)
            {
                return list.Remove(key);
            }

            public bool TryGetValue(string key, out object value)
            {
                return list.TryGetValue(key, out value);
            }

            public object this[string key]
            {
                get { return list[key]; }
                set { list[key] = value; }
            }

            public void Add(AttributeInfo item)
            {
                ((ICollection<KeyValuePair<String, Object>>)list).Add((KeyValuePair<String, Object>)item);
            }

            public void Clear()
            {
                list.Clear();
            }

            public bool Contains(AttributeInfo item)
            {
                return ((ICollection<KeyValuePair<String, Object>>)list).Contains((KeyValuePair<String, Object>)item);
            }

            void ICollection<AttributeInfo>.CopyTo(AttributeInfo[] array, int arrayIndex)
            {
                int i = arrayIndex;
                foreach (KeyValuePair<String, Object> pair in list)
                {
                    array[i++] = (AttributeInfo)pair;
                }
            }

            public int Count
            {
                get { return list.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(AttributeInfo item)
            {
                return ((ICollection<KeyValuePair<String, Object>>)list).Remove((KeyValuePair<String, Object>)item);
            }

            #region IEnumerable<AttributeInfo> Members

            public IEnumerator<AttributeInfo> GetEnumerator()
            {
                return new Enumerator(list);
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(list);
            }

            #endregion
        }

        #endregion

        private string name = null;
        private string value = null;

        private AttributeInfoCollection attributes = new AttributeInfoCollection();

        /// <summary>
        /// Default constuctor.
        /// </summary>
        public ElementInfo()
        {
        }

        /// <summary>
        /// Initializes object with specified values.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Custom attributes.</param>
        public ElementInfo(string name, params AttributeInfo[] attributes)
        {
            this.name = name;

            for (int i = 0; i < attributes.Length; i++)
            {
                this.attributes.Add(attributes[i]);
            }
        }

        public ElementInfo(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets or sets element name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets element value.
        /// </summary>
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets list of attribute Name-Value pairs.
        /// </summary>
        public AttributeInfoCollection Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// Returns element name and value, if any.
        /// </summary>
        /// <returns>Element name string.</returns>
        public override string ToString()
        {
            if (value != null)
                return name + "=" + value;
            else
                return name;
        }
    }

    #endregion

    /// <summary>
    /// Settings manager.
    /// </summary>
    public class Settings : ISettings, ICloneable
    {
        private string path;

        private XmlDocument document;

        /// <summary>
        /// Root name. Set this in inherited class ctor to change it.
        /// </summary>
        private readonly string rootName;

        /// <summary>
        /// Initilalizes new target of object.
        /// </summary>
        public Settings()
        {
            rootName = "Settings";
            document = EmptyDocument;
        }

        /// <summary>
        /// Initilalizes new target of object.
        /// </summary>
        public Settings(string rootName)
        {
            if (rootName == null) throw new ArgumentNullException("rootName", "Root name cannot be null");

            this.rootName = rootName;
            document = EmptyDocument;
        }

        /// <summary>
        /// Gets or sets path, where is file saved and load from. Value is automaticly converted into fullpath.
        /// </summary>
        public string Path
        {
            get { return path; }
            set { path = System.IO.Path.GetFullPath(value); }
        }

        /// <summary>
        /// Gets root node name.
        /// </summary>
        protected string RootName
        {
            get { return rootName; }
        }

        /// <summary>
        /// Assembles path from specified nodes using '/' delimiter.
        /// </summary>
        /// <param name="nodes">Node list.</param>
        /// <returns>String containing path.</returns>
        protected string GetPath(object[] nodes)
        {
            string path = rootName;

            foreach (object obj in nodes)
            {
                path += "/" + obj.ToString();
            }

            return path;
        }

        /// <summary>
        /// Returns and creates if doesnt exist child node with specified name.
        /// </summary>
        /// <param name="node">Root node.</param>
        /// <param name="name">Requested node name.</param>
        /// <param name="autoCreate">When true element will be created if doesn't exist.</param>
        /// <returns>Child XmlElement.</returns>
        private XmlElement GetNode(XmlNode node, string name, bool autoCreate)
        {
            if (node.ChildNodes.Count == 1 && node.FirstChild is XmlText)
            {
                throw new InvalidOperationException("Cannot add element to node with text.");
            }

            XmlElement xmlElement = node[name];
            if (xmlElement == null && autoCreate)
            {
                xmlElement = document.CreateElement(name);
                node.AppendChild(xmlElement);
            }
            return xmlElement;
        }

        /// <summary>
        /// Checks that attributes in list exist at specified element and have right value.
        /// </summary>
        private bool CheckAttributes(XmlElement elem, ElementInfo.AttributeInfoCollection list)
        {
            foreach (AttributeInfo entry in list)
            {
                XmlAttribute xmlAttr = elem.Attributes[entry.Name];
                if (xmlAttr == null || xmlAttr.Value != entry.Value.ToString())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns and creates if doesnt exist child node with specified name.
        /// </summary>
        /// <param name="node">Root node.</param>
        /// <param name="info">Requested node information.</param>
        /// <param name="autoCreate">When true element will be created if doesn't exist.</param>
        /// <returns>Child XmlElement.</returns>
        private XmlElement GetNode(XmlNode node, ElementInfo info, bool autoCreate)
        {
            if (node.ChildNodes.Count == 1 && node.FirstChild is XmlText)
            {
                throw new InvalidOperationException("Cannot add element to node with text.");
            }

            XmlElement xmlElement = null;

            foreach (XmlElement elem in node.ChildNodes)
            {
                if (elem.Name != info.Name)
                    continue;

                if (!CheckAttributes(elem, info.Attributes))
                    continue;

                xmlElement = elem;
                break;
            }

            if (xmlElement == null && autoCreate)
            {
                xmlElement = document.CreateElement(info.Name);

                foreach (KeyValuePair<String, Object> entry in info.Attributes)
                {
                    xmlElement.SetAttribute(entry.Key, entry.Value.ToString());
                }

                node.AppendChild(xmlElement);
            }
            return xmlElement;
        }

        /// <summary>
        /// Finds/create element at requested path.
        /// </summary>
        /// <param name="autoCreate">When true element will be created if doesn't exist.</param>
        /// <param name="nodes">Element path</param>
        /// <returns>Found/created element.</returns>
        protected XmlElement GetElementInternal(bool autoCreate, params object[] nodes)
        {
            XmlElement xmlElement = GetNode(document, rootName, false);

            Debug.Assert(xmlElement != null, "Cannot find root element (" + RootName + ").");

            foreach (object node in nodes)
            {
                if (node == null)
                    throw new ArgumentNullException("nodes", "One of path arguments is null.");

                if (xmlElement == null)
                    return null;

                if (node is ElementInfo)
                {
                    xmlElement = GetNode(xmlElement, (ElementInfo)node, autoCreate);
                }
                else
                {
                    xmlElement = GetNode(xmlElement, node.ToString(), autoCreate);
                }
            }

            return xmlElement;
        }

        /// <summary>
        /// Reads element at specified path. Element is created if not exist.
        /// </summary>
        /// <param name="defaultValue">Default value if element is not found.</param>
        /// <param name="nodes">Element path</param>
        /// <returns>String containing read value or default value.</returns>
        public virtual string GetElement(string defaultValue, params object[] nodes)
        {
            XmlElement xmlElement = GetElementInternal(true, nodes);

            if (xmlElement.ChildNodes.Count > 1 || (xmlElement.HasChildNodes && !(xmlElement.FirstChild is XmlText)))
            {
                throw new InvalidOperationException("Cannot set element string on node with children.");
            }

            if (xmlElement.InnerText.Length == 0)
            {
                xmlElement.InnerText = defaultValue;
            }

            return xmlElement.InnerText;
        }

        /// <summary>
        /// Reads element at specified path. Element is created if not exist.
        /// </summary>
        /// <param name="defaultValue">Default value if element is not found.</param>
        /// <param name="nodes">Element path.</param>
        /// <returns>Int32 containing read value or default value.</returns>
        public virtual int GetElement(int defaultValue, params object[] nodes)
        {
            try
            {
                return Int32.Parse(GetElement(defaultValue.ToString(), nodes));
            }
            catch (FormatException e)
            {
                Trace.WriteLine(String.Format("Cannot convert node {0} into integer. Default value used. Exception: {1}", GetPath(nodes), e.Message), "Configuration");
                return defaultValue;
            }
        }

        /// <summary>
        /// Reads element at specified path. Element is created if not exist.
        /// </summary>
        /// <param name="defaultValue">Default value if element is not found.</param>
        /// <param name="nodes">Element path.</param>
        /// <returns>Single containing read value or default value.</returns>
        public virtual float GetElement(float defaultValue, params object[] nodes)
        {
            try
            {
                return Single.Parse(GetElement(defaultValue.ToString(), nodes));
            }
            catch (FormatException e)
            {
                Trace.WriteLine(String.Format("Cannot convert node {0} into float. Default value used. Exception: {1}", GetPath(nodes), e.Message), "Configuration");
                return defaultValue;
            }
        }

        /// <summary>
        /// Reads element at specified path. Element is created if not exist.
        /// </summary>
        /// <param name="defaultValue">Default value if element is not found.</param>
        /// <param name="nodes">Element path.</param>
        /// <returns>Boolean containing read value or default value.</returns>
        public virtual bool GetElement(bool defaultValue, params object[] nodes)
        {
            try
            {
                return Boolean.Parse(GetElement(defaultValue.ToString(), nodes));
            }
            catch (FormatException e)
            {
                Trace.WriteLine(String.Format("Cannot convert node {0} into boolean. Default value used. Exception: {1}", GetPath(nodes), e.Message), "Configuration");
                return defaultValue;
            }
        }

        /// <summary>
        /// Reads element at specified path. Element is created if not exist.
        /// </summary>
        /// <typeparam name="T">Enum type which should be read.</typeparam>
        /// <param name="defaultValue">Default value if element is not found.</param>
        /// <param name="nodes">Element path.</param>
        /// <returns>T containing read value or default value.</returns>
        public virtual T GetEnumElement<T>(T defaultValue, params object[] nodes)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), GetElement(defaultValue.ToString(), nodes), true);
            }
            catch (ArgumentException e)
            {
                Trace.WriteLine(String.Format("Cannot convert node {0} into enumerator {1}. Default value used. Exception: {2}", GetPath(nodes), typeof(T).Name, e.Message), "Configuration");
                return defaultValue;
            }
        }


        /// <summary>
        /// Writes element at specified path.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="nodes">Element path.</param>
        public virtual void SetElement(object value, params object[] nodes)
        {
            XmlElement xmlElement = GetElementInternal(true, nodes);

            if (xmlElement.ChildNodes.Count > 1 || (xmlElement.HasChildNodes && !(xmlElement.FirstChild is XmlText)))
            {
                throw new InvalidOperationException("Cannot set element string on node with children.");
            }

            xmlElement.InnerText = value.ToString();
        }

        /// <summary>
        /// Removes specified element and all it's children if exists.
        /// </summary>
        /// <param name="nodes">Element path.</param>
        public virtual void RemoveElement(params object[] nodes)
        {
            XmlElement xmlElement = GetElementInternal(false, nodes);
            if (xmlElement != null)
            {
                xmlElement.ParentNode.RemoveChild(xmlElement);
            }
        }

        /// <summary>
        /// Reads attribute at specified path.
        /// </summary>
        /// <param name="defaultValue">Default value if element is not found.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="nodes">Element path</param>
        /// <returns>String containing read value or default value.</returns>
        public virtual string GetAttribute(string defaultValue, string attribute, params object[] nodes)
        {
            XmlElement xmlElement = GetElementInternal(true, nodes);

            XmlAttribute xmlAttribute = xmlElement.Attributes[attribute];
            if (xmlAttribute == null)
            {
                xmlAttribute = document.CreateAttribute(attribute);
                xmlElement.Attributes.Append(xmlAttribute);
                xmlAttribute.Value = defaultValue;
            }

            return xmlAttribute.Value;
        }

        /// <summary>
        /// Reads attribute at specified path.
        /// </summary>
        /// <param name="defaultValue">Default value if element is not found.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="nodes">Element path</param>
        /// <returns>Int32 containing read value or default value.</returns>
        public virtual int GetAttribute(int defaultValue, string attribute, params object[] nodes)
        {
            try
            {
                return Int32.Parse(GetAttribute(defaultValue.ToString(), attribute, nodes));
            }
            catch (FormatException e)
            {
                Trace.WriteLine(String.Format("Cannot convert node {0} into integer. Default value used. Exception: {1}", GetPath(nodes), e.Message), "Configuration");
                return defaultValue;
            }
        }

        /// <summary>
        /// Reads attribute at specified path.
        /// </summary>
        /// <param name="defaultValue">Default value if element is not found.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="nodes">Element path</param>
        /// <returns>Single containing read value or default value.</returns>
        public virtual float GetAttribute(float defaultValue, string attribute, params object[] nodes)
        {
            try
            {
                return Single.Parse(GetAttribute(defaultValue.ToString(), attribute, nodes));
            }
            catch (FormatException e)
            {
                Trace.WriteLine(String.Format("Cannot convert node {0} into float. Default value used. Exception: {1}", GetPath(nodes), e.Message), "Configuration");
                return defaultValue;
            }
        }

        /// <summary>
        /// Reads attribute at specified path.
        /// </summary>
        /// <param name="defaultValue">Default value if element is not found.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="nodes">Element path</param>
        /// <returns>Boolean containing read value or default value.</returns>
        public virtual bool GetAttribute(bool defaultValue, string attribute, params object[] nodes)
        {
            try
            {
                return Boolean.Parse(GetAttribute(defaultValue.ToString(), attribute, nodes));
            }
            catch (FormatException e)
            {
                Trace.WriteLine(String.Format("Cannot convert node {0} into boolean. Default value used. Exception: {1}", GetPath(nodes), e.Message), "Configuration");
                return defaultValue;
            }
        }

        /// <summary>
        /// Reads attribute at specified path.
        /// </summary>
        /// <typeparam name="T">Enum type which should be read.</typeparam>
        /// <param name="defaultValue">Default value if element is not found.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="nodes">Element path</param>
        /// <returns>T containing read value or default value.</returns>
        public virtual T GetEnumAttribute<T>(T defaultValue, string attribute, params object[] nodes)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), GetAttribute(defaultValue.ToString(), attribute, nodes), true);
            }
            catch (ArgumentException e)
            {
                Trace.WriteLine(String.Format("Cannot convert node {0} into enumerator {1}. Default value used. Exception: {2}", GetPath(nodes), typeof(T).Name, e.Message), "Configuration");
                return defaultValue;
            }
        }

        /// <summary>
        /// Writes attribute to specified node.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="nodes">path.</param>
        public virtual void SetAttribute(object value, string attribute, params object[] nodes)
        {
            XmlElement xmlElement = GetElementInternal(true, nodes);

            XmlAttribute xmlAttribute = xmlElement.Attributes[attribute];
            if (xmlAttribute == null)
            {
                xmlAttribute = document.CreateAttribute(attribute);
                xmlElement.Attributes.Append(xmlAttribute);
            }

            if (value == null) value = "";
            xmlAttribute.Value = value.ToString();
        }

        /// <summary>
        /// Removes attribute from specified element if exists.
        /// </summary>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="nodes">Element path.</param>
        public virtual void RemoveAttribute(string attribute, params object[] nodes)
        {
            XmlElement xmlElement = GetElementInternal(false, nodes);
            if (xmlElement != null)
            {
                xmlElement.Attributes.RemoveNamedItem(attribute);
            }
        }

        /// <summary>
        /// Returns True if specified element exists.
        /// </summary>
        /// <param name="nodes">Element path.</param>
        /// <returns>True if element exists.</returns>
        public virtual bool ElementExists(params object[] nodes)
        {
            return GetElementInternal(false, nodes) != null;
        }

        /// <summary>
        /// Returns True if specified attribute exists.
        /// </summary>
        /// <param name="attribute">Attribute name.</param>
        /// <param name="nodes">Element path.</param>
        /// <returns>True if attribute exists.</returns>
        public virtual bool AttributeExists(string attribute, params object[] nodes)
        {
            XmlElement xmlElement = GetElementInternal(false, nodes);

            XmlAttribute attr = null;
            if (xmlElement != null)
            {
                attr = xmlElement.Attributes[attribute];
            }

            return attr != null;
        }

        /// <summary>
        /// Enumaretes elements at specified path.
        /// </summary>
        /// <param name="nodes">Path to elements. Last is the searched one.</param>
        /// <returns>Array of ElementInfo objects. Remember that all values are in string.</returns>
        public virtual ElementInfo[] EnumarateElements(params object[] nodes)
        {
            XmlElement xmlElement = GetElementInternal(false, nodes);

            if (xmlElement == null)
                return new ElementInfo[0];

            XmlNode parent = xmlElement.ParentNode;
            int last = nodes.Length - 1;

            List<ElementInfo> list = new List<ElementInfo>();
            foreach (XmlElement e in parent.ChildNodes)
            {
                if (nodes[last] is ElementInfo)
                {
                    ElementInfo ei = (ElementInfo)nodes[last];
                    if (e.Name != ei.Name || !CheckAttributes(e, ei.Attributes))
                        continue;
                }
                else if (e.Name != nodes[last].ToString())
                    continue;

                ElementInfo info;

                if (e.ChildNodes.Count > 1 || (e.HasChildNodes && !(e.FirstChild is XmlText)))
                    info = new ElementInfo(e.Name);
                else
                    info = new ElementInfo(e.Name, e.InnerText);

                foreach (XmlAttribute a in e.Attributes)
                {
                    info.Attributes.Add(a.Name, a.Value);
                }

                list.Add(info);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Copies and changes specified element to another path. 
        /// </summary>
        /// <param name="sourceElement">Source element.</param>
        /// <param name="targetPath">Target path. New element will be added as child to this element.</param>
        /// <param name="attributes">New element attributes.</param>
        public virtual void CopyElement(object[] sourceElement, object[] targetPath, params AttributeInfo[] attributes)
        {
            XmlElement xmlElement = GetElementInternal(true, sourceElement);
            XmlElement clone = (XmlElement)xmlElement.Clone();

            foreach (AttributeInfo pair in attributes)
            {
                clone.SetAttribute(pair.Name, pair.Value.ToString());
            }

            XmlElement targetElement = GetElementInternal(true, targetPath);
            targetElement.AppendChild(clone);
        }

        protected void LoadInternal(string path)
        {
            try
            {
                document = new XmlDocument();
                document.Load(path);

                Trace.WriteLine(String.Format("{0} loaded.", path), "Configuration");
            }
            catch (Exception e)
            {
                if (File.Exists(path))
                {
                    string invalidFile = path + ".invalid";

                    if (File.Exists(invalidFile))
                        File.Delete(invalidFile);

                    File.Move(path, invalidFile);
                }

                string backupPath = path + ".backup";

                if (File.Exists(backupPath))
                {
                    Trace.WriteLine(String.Format("Unknown error occurred while loading {0}.\nMessage: {1}", System.IO.Path.GetFileName(path), e.Message), "Configuration");
                    Trace.WriteLine(String.Format("Loading settings backup ({0}).", System.IO.Path.GetFileName(backupPath)), "Configuration");

                    LoadInternal(backupPath);
                }
                else
                {
                    Trace.WriteLine(String.Format("Unknown error occurred while loading {0}. Default values will be loaded.\nMessage: {1}", System.IO.Path.GetFileName(path), e.Message), "Configuration");
                    document = EmptyDocument;
                }
            }
        }

        /// <summary>
        /// Loads settings from XML file.
        /// </summary>
        public virtual void Load()
        {
            try {
                LoadInternal(path);
            }
            catch (Exception e) {
                Trace.WriteLine(String.Format("Unknown error occurred while loading {0}. Default values will be loaded.\nMessage: {1}", System.IO.Path.GetFileName(path), e.Message), "Configuration");
                document = EmptyDocument;
            }

            try
            {
                OnLoaded(EventArgs.Empty);
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("Unknown error occurred in settings Loaded event. Exception: {0}", e.Message), "Configuration");
            }
        }

        /// <summary>
        /// Saves settings as XML file.
        /// </summary>
        public virtual void Save()
        {
            try
            {
                OnSaving(EventArgs.Empty);
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("Unknown error occurred in settings Saving event. Exception: {0}", e.Message), "Configuration");
                return;
            }

            SaveInternal();
        }

        protected void SaveInternal()
        {
            try
            {
                string newPath = path + ".new";

                document.Save(newPath);

                if (File.Exists(path))
                {
                    string backupPath = path + ".backup";

                    if (File.Exists(backupPath))
                        File.Delete(backupPath);

                    File.Move(path, backupPath);
                }

                File.Move(newPath, path);

                Trace.WriteLine(String.Format("{0} saved.", path), "Configuration");
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("Unknown error occurred while saving {0}. Exception: {1}", path, e.Message), "Configuration");
            }
        }

        /// <summary>
        /// Returns empty XmlDocument with root.
        /// </summary>
        private XmlDocument EmptyDocument
        {
            get
            {
                XmlDocument document = new XmlDocument();
                document.AppendChild(document.CreateElement(rootName));
                return document;
            }
        }

        public Settings Clone()
        {
            Settings settings = new Settings(rootName);
            settings.path = path;
            settings.document = (XmlDocument)document.Clone();
            return settings;
        }

        #region Events

        /// <summary>
        /// Raised after settings were loaded.
        /// </summary>
        public event EventHandler Loaded;

        /// <summary>
        /// Raised before settings are saved to file.
        /// </summary>
        public event EventHandler Saving;

        /// <summary>
        /// Raises Loaded event.
        /// </summary>
        protected virtual void OnLoaded(EventArgs e)
        {
            SyncEvent.Invoke(Loaded, this, e);
        }

        /// <summary>
        /// Raises Saving event.
        /// </summary>
        protected virtual void OnSaving(EventArgs e)
        {
            SyncEvent.Invoke(Saving, this, e);
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}
