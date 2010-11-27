using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Runtime
{
    public struct AnalyzerError
    {
        private string path;
        private string description;
        private string attribute;

        public AnalyzerError(string path, string description)
        {
            this.path = path;
            this.description = description;
            this.attribute = "";
        }

        public AnalyzerError(string path, string description, string attribute)
        {
            this.path = path;
            this.description = description;
            this.attribute = attribute;
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Attribute
        {
            get { return attribute; }
            set { attribute = value; }
        }

        public override string ToString()
        {
            return String.Format("Desc: {0} Path: {1} Attribute: {2}", description, path, attribute != null ? attribute : "null");
        }
    }
}
