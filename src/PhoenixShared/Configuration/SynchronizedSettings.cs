using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Phoenix.Configuration
{
    public class SynchronizedSettings : Settings
    {
        private readonly object syncRoot = new object();

        public SynchronizedSettings()
        {
        }

        public SynchronizedSettings(string rootName)
            : base(rootName)
        {
        }

        public object SyncRoot
        {
            get { return syncRoot; }
        }

        public override bool AttributeExists(string attribute, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.AttributeExists(attribute, nodes);
            }
        }

        public override bool ElementExists(params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.ElementExists(nodes);
            }
        }

        public override ElementInfo[] EnumarateElements(params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.EnumarateElements(nodes);
            }
        }

        public override int GetAttribute(int defaultValue, string attribute, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.GetAttribute(defaultValue, attribute, nodes);
            }
        }

        public override string GetAttribute(string defaultValue, string attribute, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.GetAttribute(defaultValue, attribute, nodes);
            }
        }

        public override bool GetAttribute(bool defaultValue, string attribute, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.GetAttribute(defaultValue, attribute, nodes);
            }
        }

        public override float GetAttribute(float defaultValue, string attribute, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.GetAttribute(defaultValue, attribute, nodes);
            }
        }

        public override float GetElement(float defaultValue, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.GetElement(defaultValue, nodes);
            }
        }

        public override int GetElement(int defaultValue, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.GetElement(defaultValue, nodes);
            }
        }

        public override string GetElement(string defaultValue, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.GetElement(defaultValue, nodes);
            }
        }

        public override bool GetElement(bool defaultValue, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.GetElement(defaultValue, nodes);
            }
        }

        public override T GetEnumAttribute<T>(T defaultValue, string attribute, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.GetEnumAttribute(defaultValue, attribute, nodes);
            }
        }

        public override T GetEnumElement<T>(T defaultValue, params object[] nodes)
        {
            lock (syncRoot)
            {
                return base.GetEnumElement(defaultValue, nodes);
            }
        }

        public override void RemoveAttribute(string attribute, params object[] nodes)
        {
            lock (syncRoot)
            {
                base.RemoveAttribute(attribute, nodes);
            }
        }

        public override void RemoveElement(params object[] nodes)
        {
            lock (syncRoot)
            {
                base.RemoveElement(nodes);
            }
        }

        public override void SetAttribute(object value, string attribute, params object[] nodes)
        {
            lock (syncRoot)
            {
                base.SetAttribute(value, attribute, nodes);
            }
        }

        public override void SetElement(object value, params object[] nodes)
        {
            lock (syncRoot)
            {
                base.SetElement(value, nodes);
            }
        }

        public override void CopyElement(object[] sourceElement, object[] targetPath, params AttributeInfo[] attributes)
        {
            lock (syncRoot)
            {
                base.CopyElement(sourceElement, targetPath, attributes);
            }
        }

        public override void Load()
        {
            lock (syncRoot)
            {
                LoadInternal(Path);
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

        public override void Save()
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

            lock (syncRoot)
            {
                SaveInternal();
            }
        }
    }
}
