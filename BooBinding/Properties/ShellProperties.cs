using System;
using MonoDevelop.Core;
using Pango;

namespace BooBinding
{
	public abstract class ShellProperties
	{
		private Properties properties = null;
		public abstract string PropertyName { get; }
		public Properties InternalProperties { get { return properties; } }
		public ShellProperties()
		{
			properties = PropertyService.Get<Properties>(PropertyName);
		}
		public string FontName
		{
			get { return properties.Get<string>("Font", "__default_monospace"); }
			set { properties.Set("Font", value); }
		}
		public FontDescription Font
		{
			get
			{
				if (FontName == "__default_monospace")
				{
					return FontDescription.FromString(new GConf.Client().Get("/desktop/gnome/interface/monospace_font_name") as string);
				}
				else
				{
					return FontDescription.FromString(FontName);
				}
			}
		}
		public bool AutoIndentBlocks
		{
			get { return properties.Get<bool>("AutoIndentBlocks", true); }
			set { properties.Set("AutoIndentBlocks", value); }
		}
		public bool ResetClearsScrollback
		{
			get { return properties.Get<bool>("ResetClearsScrollback", true); }
			set { properties.Set("ResetClearsScrollback", value); }
		}
		public bool ResetClearsHistory
		{
			get { return properties.Get<bool>("ResetClearsHistory", true); }
			set { properties.Set("ResetClearsHistory", value); }
		}
		public bool LoadAssemblyAfterBuild
		{
			get { return properties.Get<bool>("LoadAssemblyAfterBuild", true); }
			set { properties.Set("LoadAssemblyAfterBuild", value); }
		}
	}
}
