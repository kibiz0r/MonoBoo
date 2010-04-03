using System;
using System.Collections.Generic;

namespace BooBinding.Gui
{
	public interface IShellModel
	{
		bool Reset();
		bool LoadAssembly(string assemblyPath);
		void RegisterOutputHandler(Action handler);
		void Run();
		string[] GetOutput();
		void QueueInput(string line);
		void Dispose();
		//ShellProperties Properties { get; }
		string LanguageName { get; }
		string MimeType { get; }
		string MimeTypeExtension { get; }
		//IList<string> References { get; }
	}
}
