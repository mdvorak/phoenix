using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Configuration
{
    public class SettingsFragment : ISettings
    {
        private ISettings baseSettings;
        private object[] path;
        private DefaultPublicEvent loaded = new DefaultPublicEvent();
        private DefaultPublicEvent saving = new DefaultPublicEvent();

        public SettingsFragment(ISettings baseSettings, params object[] path)
        {
            if (baseSettings == null)
                throw new ArgumentNullException("baseSettings");

            if (path == null)
                throw new ArgumentNullException("path");

            baseSettings.Loaded += new EventHandler(baseSettings_Loaded);
            baseSettings.Saving += new EventHandler(baseSettings_Saving);

            this.baseSettings = baseSettings;
            this.path = path;
        }

        void baseSettings_Saving(object sender, EventArgs e)
        {
            saving.Invoke(this, e);
        }

        void baseSettings_Loaded(object sender, EventArgs e)
        {
            loaded.Invoke(this, e);
        }

        public event EventHandler Loaded
        {
            add { loaded.AddHandler(value); }
            remove { loaded.RemoveHandler(value); }
        }

        public event EventHandler Saving
        {
            add { saving.AddHandler(value); }
            remove { saving.RemoveHandler(value); }
        }

        private object[] ConcateArrays(object[] a1, object[] a2)
        {
            object[] a = new object[a1.Length + a2.Length];
            Array.Copy(a1, a, a1.Length);
            Array.Copy(a2, 0, a, a1.Length, a2.Length);
            return a;
        }

        public bool AttributeExists(string attribute, params object[] nodes)
        {
            return baseSettings.AttributeExists(attribute, ConcateArrays(path, nodes));
        }

        public bool ElementExists(params object[] nodes)
        {
            return baseSettings.ElementExists(ConcateArrays(path, nodes));
        }

        public ElementInfo[] EnumarateElements(params object[] nodes)
        {
            return baseSettings.EnumarateElements(ConcateArrays(path, nodes));
        }

        public int GetAttribute(int defaultValue, string attribute, params object[] nodes)
        {
            return baseSettings.GetAttribute(defaultValue, attribute, ConcateArrays(path, nodes));
        }

        public string GetAttribute(string defaultValue, string attribute, params object[] nodes)
        {
            return baseSettings.GetAttribute(defaultValue, attribute, ConcateArrays(path, nodes));
        }

        public bool GetAttribute(bool defaultValue, string attribute, params object[] nodes)
        {
            return baseSettings.GetAttribute(defaultValue, attribute, ConcateArrays(path, nodes));
        }

        public float GetAttribute(float defaultValue, string attribute, params object[] nodes)
        {
            return baseSettings.GetAttribute(defaultValue, attribute, ConcateArrays(path, nodes));
        }

        public float GetElement(float defaultValue, params object[] nodes)
        {
            return baseSettings.GetElement(defaultValue, ConcateArrays(path, nodes));
        }

        public int GetElement(int defaultValue, params object[] nodes)
        {
            return baseSettings.GetElement(defaultValue, ConcateArrays(path, nodes));
        }

        public string GetElement(string defaultValue, params object[] nodes)
        {
            return baseSettings.GetElement(defaultValue, ConcateArrays(path, nodes));
        }

        public bool GetElement(bool defaultValue, params object[] nodes)
        {
            return baseSettings.GetElement(defaultValue, ConcateArrays(path, nodes));
        }

        public T GetEnumAttribute<T>(T defaultValue, string attribute, params object[] nodes)
        {
            return baseSettings.GetEnumAttribute<T>(defaultValue, attribute, ConcateArrays(path, nodes));
        }

        public T GetEnumElement<T>(T defaultValue, params object[] nodes)
        {
            return baseSettings.GetEnumElement<T>(defaultValue, ConcateArrays(path, nodes));
        }

        public void RemoveAttribute(string attribute, params object[] nodes)
        {
            baseSettings.RemoveAttribute(attribute, ConcateArrays(path, nodes));
        }

        public void RemoveElement(params object[] nodes)
        {
            baseSettings.RemoveElement(ConcateArrays(path, nodes));
        }

        public void SetAttribute(object value, string attribute, params object[] nodes)
        {
            baseSettings.SetAttribute(value, attribute, ConcateArrays(path, nodes));
        }

        public void SetElement(object value, params object[] nodes)
        {
            baseSettings.SetElement(value, ConcateArrays(path, nodes));
        }

        public void CopyElement(object[] sourceElement, object[] targetPath, params AttributeInfo[] attributes)
        {
            baseSettings.SetElement(ConcateArrays(path, sourceElement), ConcateArrays(path, targetPath), attributes);
        }
    }
}
