using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using Phoenix.Collections;

namespace Phoenix.Runtime
{
    public class RuntimeObjectsLoaderReport
    {
        private string name = "Report";
        private StringList output = (StringList)new StringList().CreateSynchronized();
        private ListEx<Assembly> loadedAssemblies = new ListEx<Assembly>().CreateSynchronized();
        private CompilerErrorList compilerErrors = (CompilerErrorList)new CompilerErrorList().CreateSynchronized();
        private ListEx<AnalyzerError> analyzerErrors = new ListEx<AnalyzerError>().CreateSynchronized();

        public RuntimeObjectsLoaderReport()
        {
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool HasErrors
        {
            get { return compilerErrors.Count > 0 || analyzerErrors.Count > 0; }
        }

        public StringList Output
        {
            get { return output; }
        }

        public ListEx<Assembly> LoadedAssemblies
        {
            get { return loadedAssemblies; }
        }

        public CompilerErrorList CompilerErrors
        {
            get { return compilerErrors; }
        }

        public ListEx<AnalyzerError> AnalyzerErrors
        {
            get { return analyzerErrors; }
        }

        public override string ToString()
        {
            return name;
        }
    }
}
