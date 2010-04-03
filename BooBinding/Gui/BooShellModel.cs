using System;
using System.Threading;
using BooBinding.BooShell;
using System.IO;
using MonoDevelop.Core;
using System.Collections;

namespace BooBinding.Gui
{
	public class BooShellModel : IShellModel
	{
		private BooShellProperties props = new BooShellProperties();
		private Queue commandQueue = new Queue();
		private Queue outputQueue = new Queue();
		private Action outputHandler = null;
		private Thread thread = null;
		private BooShell.BooShell booShell = null;
		
		public string MimeType
		{
			get { return "text/x-boo"; }
		}
		public string LanguageName
		{
			get { return "Boo"; }
		}
		public string MimeTypeExtension
		{
			get { return "boo"; }
		}
		ShellProperties Properties
		{
			get { return props; }
		}
		IList References
		{
			get { return booShell.References; }
		}
		public BooShellModel()
		{
			getRemoteShellObject();
			try
			{
				booShell.Run();
			}
			catch { }
		}
		
		private void getRemoteShellObject()
		{
			var path = Path.Combine(Path.GetDirectoryName(typeof(BooShellModel).Assembly.Location), "BooShell.dll");
			booShell = (BooShell.BooShell)Runtime.ProcessService.CreateExternalProcessObject(path, "BooBinding.BooShell.BooShell", false);
			if (booShell == null)
			{
				throw new Exception("Unable to instantiate remote BooShell object.");
			}
		}
		
		public bool Reset()
		{
			try
			{
				booShell.Reset();
			}
			catch
			{
				return false;
			}
			return true;
		}
		
		public bool LoadAssembly(string assemblyPath)
		{
			try
			{
				booShell.LoadAssembly(assemblyPath);
			}
			catch
			{
				return false;
			}
			return true;
		}
		
		public string[] GetOutput()
		{
			lock (outputQueue)
			{
				if (outputQueue.Count > 0)
				{
					return outputQueue.ToArray() as string[];
				}
			}
			return new string[0];
		}
		
		public void QueueInput(string line)
		{
			lock (commandQueue)
			{
				commandQueue.Enqueue(line);
			}
		}
		
		public void ThreadRun()
		{
			while (true)
			{
				string com;
				lock (commandQueue)
				{
					if (commandQueue.Count == 0)
					{
						Monitor.Wait(commandQueue);
					}
					
					com = commandQueue.Dequeue() as string;
					
					if (com != null)
					{
						string[] lines = null;
						try
						{
							booShell.QueueInput(com);
							lines = booShell.GetOutput();
						}
						catch { }
						if (lines != null)
						{
							EnqueueOutput(lines);
						}
						com = null;
						lock (outputQueue)
						{
							if (outputHandler != null)
							{
								outputHandler();
							}
						}
					}
				}
			}
		}
		
		public void Run()
		{
			thread = new Thread(ThreadRun);
			thread.Start();
		}
		
		public void RegisterOutputHandler(Action handler)
		{
			outputHandler = handler;
		}
		
		public void EnqueueOutput(string[] lines)
		{
			lock (outputQueue)
			{
				foreach (var line in lines)
				{
					outputQueue.Enqueue(line);
				}
			}
		}
	
		public void Dispose()
		{
			thread.Abort();
			try
			{
				booShell.Dispose();
			}
			catch { }
		}
		
		public void print(object obj)
		{
			lock (outputQueue)
			{
				outputQueue.Enqueue(obj);
			}
		}
	}
}
