using System;
using Gtk;
using MonoDevelop.Projects.Gui.Dialogs;
using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace BooBinding.Gui.OptionPanels
{
	public class CodeCompilationPanel : MultiConfigItemOptionsPanel
	{
		private Label codeGenerationLabel = new Label();
		private Label labelWarnings = new Label();
		private Label labelOutputDir = new Label();
		private Label outputLabel = new Label();
		private Label labelCompiler = new Label();
		private Label labelCulture = new Label();
		private Label labelCompileTarget = new Label();
		private ComboBox compileTargetCombo = new ComboBox();
		private CheckButton checkDebug = new CheckButton(GettextCatalog.GetString("Enable debug"));
		private CheckButton checkDucky = new CheckButton(GettextCatalog.GetString("Enable ducky mode"));
		private Entry outputAssembly = new Entry();
		private Entry outputDirectory = new Entry();
		private Entry compilerPath = new Entry();
		private Entry culture = new Entry();
		private BooCompilerParameters compilerParameters = null;
		private DotNetProjectConfiguration configuration = null;
		private DotNetProject project = null;
		private VBox vbox = null;
		
		public CodeCompilationPanel()
		{
			InitializeComponent();
			vbox = new VBox();
			var hboxTmp = new HBox();
			hboxTmp.PackStart (codeGenerationLabel, false, false, 0);
			vbox.PackStart (hboxTmp, false, false, 12);
			
			hboxTmp = new HBox();
			var tableOutputOptions = new Table (4, 2, false);
			tableOutputOptions.Attach (outputLabel, 0, 1, 0, 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			tableOutputOptions.Attach (outputAssembly, 1, 2, 0, 1, AttachOptions.Fill | AttachOptions.Expand, AttachOptions.Fill, 0, 3);
			tableOutputOptions.Attach (labelOutputDir, 0, 1, 1, 2, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			tableOutputOptions.Attach (outputDirectory, 1, 2, 1, 2, AttachOptions.Fill | AttachOptions.Expand , AttachOptions.Fill, 0, 3);
			tableOutputOptions.Attach (labelCompileTarget, 0, 1, 2, 3, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			tableOutputOptions.Attach (compileTargetCombo, 1, 2, 2, 3, AttachOptions.Fill | AttachOptions.Expand, AttachOptions.Fill, 0, 3);
			tableOutputOptions.Attach (labelCulture, 0, 1, 3, 4, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			tableOutputOptions.Attach (culture, 1, 2, 3, 4, AttachOptions.Fill | AttachOptions.Expand, AttachOptions.Fill, 0, 3);
			hboxTmp.PackStart (tableOutputOptions, true, true, 6);
			vbox.PackStart (hboxTmp, false, false, 0);
			
			hboxTmp = new HBox ();
			hboxTmp.PackStart (labelWarnings, false, false, 0);
			vbox.PackStart (hboxTmp, false, false, 12);
			hboxTmp = new HBox();
			hboxTmp.PackStart (checkDebug, false, false, 6);
			vbox.PackStart (hboxTmp, false, false, 0);
			hboxTmp = new HBox();
			hboxTmp.PackStart (checkDucky, false, false, 6);
			vbox.PackStart (hboxTmp, false, false, 0);
			
			vbox.ShowAll ();
		}
		
		public void InitializeComponent()
		{
			codeGenerationLabel.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Code Generation"));
			labelOutputDir.Markup = String.Format ("{0} :", GettextCatalog.GetString ("Output Path"));
			labelOutputDir.Layout.Alignment = Pango.Alignment.Right;
			outputAssembly = new Entry();
			
			outputLabel.Markup = String.Format ("{0} :", GettextCatalog.GetString ("Output Assembly"));
			outputLabel.Layout.Alignment = Pango.Alignment.Right;
			labelWarnings.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Warnings and Compiler Options"));
			
			labelCompiler.Markup = String.Format ("<b>{0}</b>", GettextCatalog.GetString ("Compiler"));
			labelCulture.Markup = String.Format ("{0} :", GettextCatalog.GetString ("Culture"));
			labelCulture.Layout.Alignment = Pango.Alignment.Right;
			labelCompileTarget.Markup = String.Format ("{0} :", GettextCatalog.GetString ("Output Assembly"));
			
	
			var store = new ListStore(typeof(string));
			
			var stringArray = new string[1];
			stringArray[0] = GettextCatalog.GetString ("Executable");
			store.AppendValues (stringArray);
	
			stringArray = new string[1];
			stringArray[0] = GettextCatalog.GetString ("Library");
			store.AppendValues (stringArray);
	
			stringArray = new string[1];
			stringArray[0] = GettextCatalog.GetString ("Executable with GUI");
			store.AppendValues (stringArray);
	
			compileTargetCombo.Model = store;
			var cr = new CellRendererText();
			compileTargetCombo.PackStart(cr, true);
			compileTargetCombo.AddAttribute(cr, "text", 0);
		}
		
		public override void LoadConfigData ()
		{
			//FIXME: BOO COMPILER CAN'T RESOLVE OVERLOADS OF GENERIC METHODS
			//configuration = (cast(MonoDevelop.Core.Properties,CustomizationObject)).Get [of DotNetProjectConfiguration] ("Config")
			configuration = CurrentConfiguration as DotNetProjectConfiguration;
			project = ConfiguredProject as DotNetProject;
			if (configuration == null)
			{
				throw new InvalidOperationException ("Invalid program state as a result of Boo compiler bug http://jira.codehaus.org/browse/BOO-856");
			}
			
			compilerParameters = configuration.CompilationParameters as BooCompilerParameters;
	
			checkDebug.Active = configuration.DebugMode;
			checkDucky.Active = compilerParameters.Ducky;
			outputAssembly.Text = configuration.OutputAssembly;
			//outputDirectory.DefaultPath = configuration.OutputDirectory
			outputDirectory.Text = configuration.OutputDirectory;
			
			culture.Text = compilerParameters.Culture;
			compileTargetCombo.Active = (int)configuration.CompileTarget;
			if (project.IsLibraryBasedProjectType)
			{
				compileTargetCombo.Sensitive = false;
			}
		}
		
		public override void ApplyChanges()
		{
			if (compilerParameters == null)
			{
				return;
			}
	
			project.CompileTarget = (CompileTarget)compileTargetCombo.Active;
			configuration.DebugMode = checkDebug.Active;
			configuration.OutputAssembly = outputAssembly.Text;
			configuration.OutputDirectory = outputDirectory.Text;
			//configuration.OutputDirectory = outputDirectory.Path
	
			compilerParameters.Ducky = checkDucky.Active;
			compilerParameters.Culture = culture.Text;
		}
		
		public override Widget CreatePanelWidget()
		{
			return vbox;
		}
	}
}
