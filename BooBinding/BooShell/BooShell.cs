using System;
using MonoDevelop.Core.Execution;
using Boo.Lang.Interpreter;
using System.Collections;
using System.Threading;
using Boo.Lang.Compiler.TypeSystem;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Gtk;

namespace BooBinding.BooShell
{
	public class BooShell : RemoteProcessObject
	{
		private InteractiveInterpreter interpreter = null;
		private Queue commandQueue = new Queue();
		private Queue outputQueue = new Queue();
		private Thread thread = null;
		private bool processing = true;
		
		public BooShell()
		{
			interpreter = new InteractiveInterpreter()
			{
				RememberLastValue = true,
				Print = print
			};
		}
		
		public override object InitializeLifetimeService()
		{
			return null;
		}
		
		public bool Reset()
		{
			EnqueueCommand(new ShellCommand (ShellCommandType.Reset, null));
			return true;
		}
		
		public bool LoadAssembly(string assemblyPath)
		{
			EnqueueCommand(new ShellCommand (ShellCommandType.Load, assemblyPath));
			return true;
		}
		
		public IList References
		{
			get
			{
				var list = new List<string>();
				lock (interpreter)
				{
					foreach (Assembly assembly in interpreter.References)
					{
						try
						{
							var location = assembly.Location;
							list.Add(location);
						}
						catch
						{
							continue;
						}
					}
				}
				return list;
			}
		}
		
		public string[] GetOutput()
		{
			lock (outputQueue)
			{
				if (processing)
				{
					Monitor.Wait(outputQueue);
				}
				return outputQueue.ToArray() as string[];
			}
		}
		
		private void print(object obj)
		{
			lock (outputQueue)
			{
				outputQueue.Enqueue(obj.ToString());
			}
		}
		
		private void kickOffGuiThread()
		{
			thread = new Thread(ThreadRun);
			thread.IsBackground = true;
			thread.Start();
		}
		
		private void EnqueueCommand(ShellCommand command)
		{
			if (!thread.IsAlive)
			{
				kickOffGuiThread();
			}

			lock (commandQueue)
			{
				commandQueue.Enqueue(command);
				lock (outputQueue)
				{
					processing = true;
				}
			}
		}
		
		public override void Dispose()
		{
			if (thread.IsAlive)
			{
				thread.Abort();
			}
			base.Dispose();
		}
		
		public void QueueInput(string line)
		{
			EnqueueCommand(new ShellCommand(ShellCommandType.Eval, line));
		}
		
		public void ThreadRun()
		{
			Application.Init();
			GLib.Idle.Add(ProcessCommands);
			try
			{
				Application.Run();
			}
			catch
			{
			}
		}
		
		public void Run()
		{
			kickOffGuiThread();
		}
		
		private bool ProcessCommands()
		{
			ShellCommand com;
			
			lock (commandQueue)
			{
				if (commandQueue.Count == 0)
				{
					Monitor.Exit(commandQueue);
					Thread.Sleep(100);
					return true;
				}
				com = (ShellCommand)commandQueue.Dequeue();
				
				lock (interpreter)
				{
					if (com.Type == ShellCommandType.Eval)
					{
						if (com.Data != null)
						{
							try
							{
								interpreter.LoopEval(com.Data);
							}
							catch (Exception ex)
							{
								var exception = ex.InnerException;
								var message = exception.ToString();
								var reg = new Regex(@"((.*\n)*).*Input\dModule.*", RegexOptions.Multiline);
								var match = reg.Match(exception.StackTrace);
								if (match != null)
								{
									if (match.Groups.Count >= 3)
									{
										message = String.Format("{0}: {1}\n{2}", exception.GetType(),
										                        exception.Message,
										                        match.Groups[1].Value);
									}
								}
								print(message);
							}
						}
					}
					else if (com.Type == ShellCommandType.Reset)
					{
						interpreter.Reset();
					}
					else if (com.Type == ShellCommandType.Load)
					{
						if (com.Data != null)
						{
							interpreter.load(com.Data);
						}
					}
				}
				com.Type = ShellCommandType.NoOp;
				
				if (commandQueue.Count == 0)
				{
					lock (outputQueue)
					{
						processing = false;
					}
				}
			}
			return true;
		}
	}
}
