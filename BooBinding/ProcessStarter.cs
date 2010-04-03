using System;
using MonoDevelop.Core.Execution;
using System.IO;
using MonoDevelop.Core;

namespace BooBinding
{
    public class ProcessStarter : IProcessStarter
    {
        public ProcessWrapper StartProcess(string command, string arguments, string workingDirectory, TextWriter outWriter, TextWriter errorWriter)
        {
            return Runtime.ProcessService.StartProcess(command, arguments, workingDirectory, outWriter, errorWriter, null);
        }
    }
}
