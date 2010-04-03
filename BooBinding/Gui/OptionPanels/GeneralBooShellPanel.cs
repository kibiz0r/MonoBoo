using System;

namespace BooBinding.Gui.OptionPanels
{
	public class GeneralBooShellPanel : GeneralShellPanel
	{
		public override ShellProperties ShellProperties
		{
			get { return new BooShellProperties(); }
		}
	}
}
