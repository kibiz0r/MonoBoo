using System;
using MonoDevelop.Core.Execution;
using System.IO;

namespace BooBinding
{
    public interface IProcessStarter
    {
        ProcessWrapper StartProcess(string command, string arguments, string workingDirectory, TextWriter outWriter, TextWriter errorWriter);
    }
}
