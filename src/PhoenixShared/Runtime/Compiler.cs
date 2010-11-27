using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Boo.Lang.Compiler.Pipelines;
using Microsoft.Win32;

namespace Phoenix.Runtime
{
    public static class Compiler
    {
        public enum Language
        {
            CSharp,
            VisualBasic,
            Boo
        }

        public static Version DotNetVersion
        {
            get
            {
                List<Version> versions = new List<Version>();

                string fwSetup = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\";
                string HKLM = Registry.LocalMachine.ToString() + "\\";
                using (RegistryKey fwInstallKey = Registry.LocalMachine.OpenSubKey(fwSetup)) {
                    foreach (string vers in fwInstallKey.GetSubKeyNames()) {
                        string fwVersion = (string)Registry.GetValue(HKLM + fwSetup + vers, "Version", null);
                        if (fwVersion != null) {
                            try {
                                // Trace.WriteLine(fwVersion, "Runtime");
                                versions.Add(new Version(fwVersion));
                            }
                            catch { }
                        }
                    }
                }

                if (versions.Count > 0) {
                    versions.Sort();
                    return versions[versions.Count - 1];
                }
                else {
                    return new Version();
                }
            }
        }

        public static bool Net35
        {
            get
            {
                return DotNetVersion >= new Version(3, 5);
            }
        }

        public static CompilerResults Compile(Language lang, string[] sourceFiles, string[] referencedAssemblies)
        {
            CodeDomProvider provider;

            Dictionary<string, string> args = new Dictionary<string, string>();

            Trace.WriteLine(".NET runtime version: " + Environment.Version.ToString(), "Runtime");
            Trace.WriteLine(".NET installed version: " + DotNetVersion.ToString(), "Runtime");
            if (Net35) {
                args.Add("CompilerVersion", "v3.5");
                Trace.WriteLine("Using v3.5 compiler", "Runtime");
            }

            switch (lang) {
                case Language.CSharp:
                    provider = new CSharpCodeProvider(args);
                    Debug.Assert(provider.FileExtension.ToLowerInvariant() == "cs");
                    break;

                case Language.VisualBasic:
                    provider = new VBCodeProvider(args);
                    Debug.Assert(provider.FileExtension.ToLowerInvariant() == "vb");
                    break;

                case Language.Boo:
                    return CompileBoo(sourceFiles, referencedAssemblies);

                default:
                    throw new ArgumentException("Invalid language.");
            }

            CompilerParameters options = new CompilerParameters();
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;
            options.IncludeDebugInformation = true;
            options.WarningLevel = 3;

            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            options.ReferencedAssemblies.Add("System.Drawing.dll");
            options.ReferencedAssemblies.Add("System.Xml.dll");
            options.ReferencedAssemblies.Add("System.Data.dll");
            options.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");

            if (Net35) {
                options.ReferencedAssemblies.Add("System.Core.dll");
                options.ReferencedAssemblies.Add("System.Xml.Linq.dll");
            }

            foreach (string reference in referencedAssemblies) {
                options.ReferencedAssemblies.Add(reference);
            }

            CompilerResults result = CompileLanguage(sourceFiles, provider, options);

            provider.Dispose();

            return result;
        }

        private static CompilerResults CompileLanguage(string[] sourceFiles, CodeDomProvider provider, CompilerParameters options)
        {
            options.EmbeddedResources.Clear();
            options.LinkedResources.Clear();

            int codeFileCount = 0;
            List<String> files = new List<String>();

            foreach (string file in sourceFiles) {
                string ext = Path.GetExtension(file);
                if (String.Compare(ext, "." + provider.FileExtension, StringComparison.InvariantCultureIgnoreCase) == 0) {
                    files.Add(file);
                    codeFileCount++;
                }
                else if (ext.ToLowerInvariant() == ".resx") {
                    options.EmbeddedResources.Add(file);
                }
            }

            if (codeFileCount > 0) {
                try {
                    return provider.CompileAssemblyFromFile(options, files.ToArray());
                }
                catch (Exception ex) {
                    Trace.WriteLine("Unexpected error during compilation:\n" + ex.ToString(), "Runtime");
                }
            }

            return null;
        }

        private static Assembly coreAssembly;

        private static CompilerResults CompileBoo(string[] sourceFiles, string[] referencedAssemblies)
        {
            // Prepare parameters
            var cp = new Boo.Lang.Compiler.CompilerParameters();
            cp.Pipeline = new CompileToMemory();
            cp.Ducky = true;
            cp.Debug = true;
            cp.OutputType = Boo.Lang.Compiler.CompilerOutputType.Library;
            cp.DefaultMethodVisibility = Boo.Lang.Compiler.Ast.TypeMemberModifiers.Public;

            // References
            cp.References.Add(typeof(System.Windows.Forms.Form).Assembly);
            cp.References.Add(typeof(System.Drawing.Color).Assembly);
            cp.References.Add(typeof(System.Xml.XmlDocument).Assembly);

            // .NET 3.5
            if (Net35) {
                if (coreAssembly == null) {
                    coreAssembly = Assembly.LoadWithPartialName("System.Core");
                }
                cp.References.Add(coreAssembly);
            }

            // Add input files
            foreach (string file in sourceFiles) {
                string ext = Path.GetExtension(file);
                if (String.Compare(ext, ".boo", StringComparison.InvariantCultureIgnoreCase) == 0) {
                    cp.Input.Add(new Boo.Lang.Compiler.IO.FileInput(file));
                }
                else if (ext.ToLowerInvariant() == ".resx") {
                    cp.Resources.Add(new Boo.Lang.Compiler.Resources.EmbeddedFileResource(file));
                }
            }

            if (cp.Input.Count == 0) {
                // Nothing to compile
                return null;
            }

            // Create compiler
            Boo.Lang.Compiler.BooCompiler booc = new Boo.Lang.Compiler.BooCompiler(cp);

            var ctx = booc.Run();

            // Convert result
            CompilerResults result = new CompilerResults(null);
            result.CompiledAssembly = ctx.GeneratedAssembly;

            result.Output.Add(booc.GetType().AssemblyQualifiedName);
            result.Output.Add("");

            foreach (Boo.Lang.Compiler.CompilerWarning w in ctx.Warnings) {
                result.Output.Add(w.ToString());
            }

            foreach (Boo.Lang.Compiler.CompilerError e in ctx.Errors) {
                result.Output.Add(e.ToString());
                result.Errors.Add(new CompilerError(e.LexicalInfo.FileName, e.LexicalInfo.Line, e.LexicalInfo.Column, e.Code, e.Message));
            }

            return result;
        }
    }
}
