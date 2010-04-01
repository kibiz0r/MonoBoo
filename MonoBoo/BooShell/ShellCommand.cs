using System;

namespace BooBinding
{
	public struct ShellCommand
	{
		public ShellCommandType Type;
		public readonly string Data;
		
		public ShellCommand(ShellCommandType type, string data)
		{
			Type = type;
			Data = data;
		}
	}
}
