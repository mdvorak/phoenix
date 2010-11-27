using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Phoenix.Collections;
using System.Security;
using System.Security.Permissions;

namespace Phoenix.Runtime
{
    /*
    [SecurityPermission(SecurityAction.Deny,
        BindingRedirects = true,
        ControlAppDomain = true,
        ControlDomainPolicy = true,
        ControlEvidence = true,
        ControlPolicy = true,
        ControlThread = true,
        SkipVerification = true,
        Infrastructure = true)]*/
    public sealed class RuntimeObjectsLoader
    {
        private StringList fileList;
        private ListEx<Assembly> assemblyList;
        private StringList referenceList;
        private RuntimeObjectsLoaderReport report;

        private MethodInfo registerMethod;

        public RuntimeObjectsLoader()
        {
            fileList = new StringList();
            assemblyList = new ListEx<Assembly>();
            referenceList = new StringList();
            report = new RuntimeObjectsLoaderReport();

            registerMethod = typeof(RuntimeAttribute).GetMethod("Register", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(registerMethod != null, "Register method not found.");
        }

        /// <summary>
        /// Gets list of files to compile.
        /// </summary>
        public StringList FileList
        {
            get { return fileList; }
        }

        /// <summary>
        /// Gets list of assemblies to analyze.
        /// </summary>
        public ListEx<Assembly> AssemblyList
        {
            get { return assemblyList; }
        }

        /// <summary>
        /// List of referenced assemlbies.
        /// </summary>
        public StringList ReferenceList
        {
            get { return referenceList; }
        }

        /// <summary>
        /// Get compiler analyzer results.
        /// </summary>
        public RuntimeObjectsLoaderReport Report
        {
            get { return report; }
        }

        public void Run()
        {
            RunInternal();
        }

        private void RunInternal()
        {
            try {
                // Files compilation
                if (fileList.Count > 0) {
                    string[] fileArray = fileList.ToArray();
                    string[] referenceArray = referenceList.ToArray();

                    try {
                        CompilerResults csResult = Compiler.Compile(Compiler.Language.CSharp, fileArray, referenceArray);
                        if (csResult != null)
                            ProcessCompilerResult(csResult);
                        else
                            report.Output.Add("No C# scripts compiled.");
                    }
                    catch (Exception e) {
                        report.Output.Add("Unexpected error during C# scripts compilation. Exception:");
                        report.Output.Add(e.ToString());
                    }

                    try {
                        CompilerResults vbResult = Compiler.Compile(Compiler.Language.VisualBasic, fileArray, referenceArray);
                        if (vbResult != null)
                            ProcessCompilerResult(vbResult);
                        else
                            report.Output.Add("No VB scripts compiled.");
                    }
                    catch (Exception e) {
                        report.Output.Add("Unexpected error during VB scripts compilation. Exception:");
                        report.Output.Add(e.ToString());
                    }

                    try {
                        CompilerResults booResult = Compiler.Compile(Compiler.Language.Boo, fileArray, referenceArray);
                        if (booResult != null)
                            ProcessCompilerResult(booResult);
                        else
                            report.Output.Add("No Boo scripts compiled.");
                    }
                    catch (Exception e) {
                        report.Output.Add("Unexpected error during Boo scripts compilation. Exception:");
                        report.Output.Add(e.ToString());
                    }
                }
                else {
                    report.Output.Add("No files to compile.");
                }

                report.Output.Add("");

                // Assemblies analyzes
                if (assemblyList.Count > 0) {
                    while (assemblyList.Count > 0) {
                        Assembly a = assemblyList[0];
                        assemblyList.Remove(a);
                        report.LoadedAssemblies.Add(a);

                        AnalyzeAndRegister(a);
                    }
                }
                else {
                    report.Output.Add("No assemblies to analyze.");
                }
            }
            catch (Exception e) {
                report.Output.Add("Unhandled exception:");
                report.Output.Add(e.ToString());
            }

            if (report.CompilerErrors.Count > 0 || report.AnalyzerErrors.Count > 0)
                report.Output.Add(String.Format("========== Found {0} compiler errors or warnings and {1} analyzer errors ==========", report.CompilerErrors.Count, report.AnalyzerErrors.Count));
            else
                report.Output.Add("========== No errors found ==========");
        }

        private void ProcessCompilerResult(CompilerResults result)
        {
            report.Output.AddRange(result.Output);
            report.CompilerErrors.AddRange(result.Errors);

            if (!result.Errors.HasErrors) {
                try {
                    assemblyList.Add(result.CompiledAssembly);
                }
                catch { }
            }
        }

        private void AnalyzeAndRegister(Assembly assembly)
        {
            if (assembly == null)
                return;

            try {
                report.Output.Add(String.Format("Analyzation of assembly \"{0}\" started.", assembly));

                // FIX: GetTypes misto GetExportedTypes kvuli Boo
                List<Type> types = new List<Type>();

                foreach (Type t in assembly.GetTypes()) {
                    if (t.IsPublic)
                        types.Add(t);
                }

                report.Output.Add(String.Format("{0} exported types found.", types.Count));

                for (int i = 0; i < types.Count; i++) {
                    AnalyzeType(types[i]);
                }
            }
            catch (Exception e) {
                report.Output.Add("Unhandled exception during analyzation:");
                report.Output.Add(e.ToString());
            }
            finally {
                report.Output.Add(String.Format("Analyzation of assembly \"{0}\" finished.", assembly));
            }
        }

        private void AnalyzeType(Type type)
        {
            StringList output = new StringList();
            bool outputCommited = false;

            output.Add("Analyzing type " + type.ToString());

            Object target = null;

            // Check attribute on class itself
            if (type.IsDefined(typeof(RuntimeAttribute), true)) {
                // Commint output
                if (!outputCommited)
                    report.Output.AddRange(output);
                output = report.Output;
                outputCommited = true;

                AnalyzeClass(type, ref target);
            }
            else {
                output.Add("\tRuntimeAttribute not set.");
            }


            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            output.Add("\t" + methods.Length.ToString() + " public methods found.");

            foreach (MethodInfo mi in methods) {
                if (mi.IsDefined(typeof(RuntimeAttribute), true)) {
                    // Commint output
                    if (!outputCommited)
                        report.Output.AddRange(output);
                    output = report.Output;
                    outputCommited = true;

                    string methodPath = type.ToString() + "." + mi.Name + "()";

                    try {
                        report.Output.Add(String.Format("\tRegistering {0}..", mi));

                        if (!mi.IsStatic && target == null) {
                            try {
                                target = type.Assembly.CreateInstance(type.FullName);
                            }
                            catch (Exception e) {
                                throw new Exception("Error creating object instance.", e);
                            }
                        }

                        object[] attributes = mi.GetCustomAttributes(typeof(RuntimeAttribute), true);

                        foreach (object a in attributes) {
                            try {
                                string msg = (string)registerMethod.Invoke(a, new object[] { mi, target });

                                if (msg != null && msg.Length > 0)
                                    report.Output.Add("\t\t" + msg);
                            }
                            catch (TargetInvocationException e) {
                                report.AnalyzerErrors.Add(new AnalyzerError(methodPath, e.InnerException.Message, a.ToString()));
                                report.Output.Add("\t\tError: " + e.InnerException.ToString());
                            }
                            catch (Exception e) {
                                report.AnalyzerErrors.Add(new AnalyzerError(methodPath, e.Message, a.ToString()));
                                report.Output.Add("\t\tError: " + e.ToString());
                            }
                        }
                    }
                    catch (Exception e) {
                        report.AnalyzerErrors.Add(new AnalyzerError(methodPath, e.Message));
                        report.Output.Add("\t\tError: " + e.ToString());
                    }
                } // if
            } // foreach
        }

        private void AnalyzeClass(Type type, ref Object target)
        {
            if (type.IsClass && !type.IsAbstract) {
                try {
                    report.Output.Add("\tRuntimeAttribute found. Registering..");

                    try {
                        if (type == typeof(Control) || type.IsSubclassOf(typeof(Control))) {
                            // Create object on gui thread
                            target = Core.GuiThread.CreateObject(type);
                        }
                        else {
                            target = type.Assembly.CreateInstance(type.FullName);
                        }
                    }
                    catch (Exception e) {
                        throw new Exception("Error creating object instance.", e);
                    }

                    object[] attributes = type.GetCustomAttributes(typeof(RuntimeAttribute), true);

                    foreach (object a in attributes) {
                        try {
                            string msg = (string)registerMethod.Invoke(a, new object[] { type, target });

                            if (msg != null && msg.Length > 0)
                                report.Output.Add("\t\t" + msg);
                        }
                        catch (TargetInvocationException e) {
                            report.AnalyzerErrors.Add(new AnalyzerError(type.ToString(), e.InnerException.Message, a.ToString()));
                            report.Output.Add("\t\tError: " + e.InnerException.ToString());
                        }
                        catch (Exception e) {
                            report.AnalyzerErrors.Add(new AnalyzerError(type.ToString(), e.Message, a.ToString()));
                            report.Output.Add("\t\tError: " + e.ToString());
                        }
                    }

                    report.Output.Add("\t\tSuccessfully completed.");
                }
                catch (Exception e) {
                    report.AnalyzerErrors.Add(new AnalyzerError(type.ToString(), e.Message));
                    report.Output.Add("\t\tError: " + e.ToString());
                }
            }
            else {
                report.Output.Add("\t\tInvalid use of RuntimeAttribute. Type is not class or it is abstract.");
            }
        }
    }
}
