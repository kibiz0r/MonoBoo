using System;
using MonoDevelop.Core.Gui.Dialogs;
using Gtk;
using MonoDevelop.Core;

namespace BooBinding
{
	public class GeneralShellPanel : OptionsPanel
	{
		private Label generalOptionsLabel = new Label();
		private CheckButton autoIndentCheckButton = new CheckButton();
		private CheckButton resetClearsScrollbackCheckButton = new CheckButton();
		private CheckButton resetClearsHistoryCheckButton = new CheckButton();
		private CheckButton loadAssemblyCheckButton = new CheckButton();
		
		private Label fontOptionsLabel = new Label();
		private FontButton fontButton = new FontButton();
		private RadioButton defaultMonoRadio = null;
		private RadioButton customFontRadio = null;
		
		public virtual ShellProperties ShellProperties
		{
			get { return null; }
		}
		
		private void InitializeComponent()
		{
			generalOptionsLabel.Markup = String.Format("<b>{0}</b>", GettextCatalog.GetString("General Options"));
			autoIndentCheckButton.Label = GettextCatalog.GetString("Automatically indent new lines in code blocks");
			resetClearsScrollbackCheckButton.Label = GettextCatalog.GetString("Shell reset clears scrollback");
			resetClearsHistoryCheckButton.Label = GettextCatalog.GetString("Shell reset clears command history");
			loadAssemblyCheckButton.Label = GettextCatalog.GetString("Load project assemblies after building them (Causes shell reset)");
			fontOptionsLabel.Markup = String.Format("<b>{0}</b>", GettextCatalog.GetString("Font"));
			defaultMonoRadio = new RadioButton(GettextCatalog.GetString("Use default monospace font"));
			customFontRadio = new RadioButton(defaultMonoRadio, GettextCatalog.GetString("Use custom font:"));
			defaultMonoRadio.Toggled += ItemToggled;
			customFontRadio.Toggled += ItemToggled;
		}
		
		private void ItemToggled(object sender, EventArgs args)
		{
			fontButton.Sensitive = customFontRadio.Active;
		}
		
		public override Widget CreatePanelWidget()
		{
			InitializeComponent();
			var vbox = new VBox();
			var hboxTmp = new HBox();
			hboxTmp.PackStart(generalOptionsLabel, false, false, 0);
			vbox.PackStart(hboxTmp, false, false, 12);
			hboxTmp = new HBox();
			hboxTmp.PackStart(autoIndentCheckButton, false, false, 6);
			vbox.PackStart(hboxTmp, false, false, 0);
			hboxTmp = new HBox();
			hboxTmp.PackStart(resetClearsScrollbackCheckButton, false, false, 6);
			vbox.PackStart(hboxTmp, false, false, 0);
			hboxTmp = new HBox();
			hboxTmp.PackStart (resetClearsHistoryCheckButton, false, false, 6);
			vbox.PackStart (hboxTmp, false, false, 0);
			hboxTmp = new HBox();
			hboxTmp.PackStart (loadAssemblyCheckButton, false, false, 6);
			vbox.PackStart (hboxTmp, false, false, 0);
			hboxTmp = new HBox();
			hboxTmp.PackStart (fontOptionsLabel, false, false, 0);
			vbox.PackStart (hboxTmp, false, false, 12);
			hboxTmp = new HBox();
			hboxTmp.PackStart(defaultMonoRadio, false, false, 6);
			vbox.PackStart (hboxTmp, false, false, 0);
			hboxTmp = new HBox();
			hboxTmp.PackStart (customFontRadio, false, false, 6);
			hboxTmp.PackStart (fontButton, false, false, 0);
			vbox.PackStart (hboxTmp, false, false, 0);
	
			var s = ShellProperties.FontName;
	
			if (s == "__default_monospace")
			{
				defaultMonoRadio.Active = true;
			}
			else
			{
				fontButton.FontName = s;
				customFontRadio.Active = true;
			}
	
			fontButton.Sensitive = customFontRadio.Active;
			autoIndentCheckButton.Active = ShellProperties.AutoIndentBlocks;
			resetClearsScrollbackCheckButton.Active = ShellProperties.ResetClearsScrollback;
			resetClearsHistoryCheckButton.Active = ShellProperties.ResetClearsHistory;
			loadAssemblyCheckButton.Active =  ShellProperties.LoadAssemblyAfterBuild;
			vbox.ShowAll();
			return vbox;
		}
		
		public override void ApplyChanges()
		{
			if (customFontRadio.Active)
			{
				ShellProperties.FontName = fontButton.FontName;
			}
			else if (defaultMonoRadio.Active)
			{
				ShellProperties.FontName = "__default_monospace";
			}
			if (ShellProperties.AutoIndentBlocks != autoIndentCheckButton.Active)
			{
				ShellProperties.AutoIndentBlocks = autoIndentCheckButton.Active;
			}
			if (ShellProperties.ResetClearsScrollback != resetClearsScrollbackCheckButton.Active)
			{
				ShellProperties.ResetClearsScrollback = resetClearsScrollbackCheckButton.Active;
			}
			if (ShellProperties.ResetClearsHistory != resetClearsHistoryCheckButton.Active)
			{
				ShellProperties.ResetClearsHistory = resetClearsHistoryCheckButton.Active;
			}
			if (ShellProperties.LoadAssemblyAfterBuild != loadAssemblyCheckButton.Active)
			{
				ShellProperties.LoadAssemblyAfterBuild = loadAssemblyCheckButton.Active;
			}
		}
	}
}
