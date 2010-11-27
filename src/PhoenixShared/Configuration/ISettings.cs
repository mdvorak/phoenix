using System;
using System.Collections.Generic;

namespace Phoenix.Configuration
{
    public interface ISettings
    {
        bool AttributeExists(string attribute, params object[] nodes);
        bool ElementExists(params object[] nodes);
        ElementInfo[] EnumarateElements(params object[] nodes);
        int GetAttribute(int defaultValue, string attribute, params object[] nodes);
        string GetAttribute(string defaultValue, string attribute, params object[] nodes);
        bool GetAttribute(bool defaultValue, string attribute, params object[] nodes);
        float GetAttribute(float defaultValue, string attribute, params object[] nodes);
        float GetElement(float defaultValue, params object[] nodes);
        int GetElement(int defaultValue, params object[] nodes);
        string GetElement(string defaultValue, params object[] nodes);
        bool GetElement(bool defaultValue, params object[] nodes);
        T GetEnumAttribute<T>(T defaultValue, string attribute, params object[] nodes);
        T GetEnumElement<T>(T defaultValue, params object[] nodes);
        void RemoveAttribute(string attribute, params object[] nodes);
        void RemoveElement(params object[] nodes);
        void SetAttribute(object value, string attribute, params object[] nodes);
        void SetElement(object value, params object[] nodes);
        void CopyElement(object[] sourceElement, object[] targetPath, params AttributeInfo[] attributes);

        event EventHandler Loaded;
        event EventHandler Saving;
    }
}
