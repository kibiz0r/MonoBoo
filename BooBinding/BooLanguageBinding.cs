using System;
using System.Linq;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Dom;
using System.CodeDom.Compiler;
using System.IO;
using MonoDevelop.Core;
using System.Text;
using System.Collections.Generic;
using MonoDevelop.Core.ProgressMonitoring;
using System.Text.RegularExpressions;

namespace BooBinding
{
    public class BooLanguageBinding : IDotNetLanguageBinding
    {
        public ConfigurationParameters CreateCompilationParameters(System.Xml.XmlElement projectOptions)
        {
            return new BooCompilerParameters();
        }

        public ProjectParameters CreateProjectParameters(System.Xml.XmlElement projectOptions)
        {
            return new ProjectParameters();
        }

        public BuildResult Compile(ProjectItemCollection items, DotNetProjectConfiguration configuration, ConfigurationSelector configSelector, IProgressMonitor monitor)
        {
            try
            {
                var outWriter = new StringWriter();
                var arguments = String.Join(" ", items.GetAll<ProjectFile>().Select(pf => pf.FilePath.ToString()).ToArray());
                using (var processWrapper = MonoDevelop.Core.Runtime.ProcessService.StartProcess("booc", arguments, configuration.OutputDirectory.ToString(), outWriter, monitor.Log, null))
                {
                    using (var aggregatedOperationMonitor = new AggregatedOperationMonitor(monitor, processWrapper))
                    {
                        processWrapper.WaitForExit();
                        if (monitor.IsCancelRequested)
                        {
                            monitor.Log.WriteLine("Build canceled");
                            monitor.ReportError("Build canceled", null);
                        }
                        monitor.EndTask(); // Do we need this?
                        var output = outWriter.ToString();
                        var compilerResults = new CompilerResults(new TempFileCollection());
                        using (var stringReader = new StringReader(output))
                        {
                            while (true)
                            {
                                var line = stringReader.ReadLine();
                                if (line == null)
                                {
                                    break;
                                }
                                var error = ParseOutputLine(line);
                                if (error != null)
                                {
                                    compilerResults.Errors.Add(error);
                                }
                            }
                        }
                        return new BuildResult(compilerResults, output);
                    }
                }
            }
            catch (Exception ex)
            {
                monitor.Log.WriteLine(ex.ToString());
            }
            return null;
        }

        private CompilerError ParseOutputLine(string line)
        {
            var regex = new Regex(@"(?<file>.*\.boo)\s*\((?<line>\d+),\s?(?<column>\d+)\):\s*(?<message>.*)");
            var match = regex.Match(line);
            if (match.Success)
            {
                var error = new CompilerError();
                error.FileName = match.Groups["file"].Value;
                error.Line = int.Parse(match.Groups["line"].Value);
                error.Column = int.Parse(match.Groups["column"].Value);
                error.ErrorText = match.Groups["message"].Value;
                if (error.ErrorText.Contains("WARNING"))
                {
                    error.IsWarning = true;
                }
                return error;
            }
            return null;
        }

        public ClrVersion[] GetSupportedClrVersions()
        {
            return new ClrVersion[] { ClrVersion.Net_2_0 };
        }

        public System.CodeDom.Compiler.CodeDomProvider GetCodeDomProvider()
        {
            return new Boo.Lang.CodeDom.BooCodeProvider();
        }

        public string ProjectStockIcon
        {
            get { return "md-project"; }
        }

        public bool IsSourceCodeFile(string fileName)
        {
            return Path.GetExtension(fileName.ToLower()) == ".boo";
        }

        public string GetFileName(string fileNameWithoutExtension)
        {
            return fileNameWithoutExtension + ".boo";
        }

        public string Language
        {
            get { return "Boo"; }
        }

        public string SingleLineCommentTag
        {
            get { return "#"; }
        }

        public string BlockCommentStartTag
        {
            get { return "/*"; }
        }

        public string BlockCommentEndTag
        {
            get { return "*/"; }
        }

        public MonoDevelop.Projects.Dom.Parser.IParser Parser
        {
            get { return null; }
        }

        public MonoDevelop.Projects.CodeGeneration.IRefactorer Refactorer
        {
            get { return null; }
        }
    }
}
