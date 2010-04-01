using System;
using MonoDevelop.Projects;
using MonoDevelop.Core.Serialization;

namespace BooBinding
{
	public class BooCompilerParameters : ConfigurationParameters
	{
		[ItemProperty("genwarnings")]
		public bool genwarnings = false;
		
		[ItemProperty("ducky")]
		public bool ducky = false;
	
		[ItemProperty("culture")]
		public string culture = "";
		
		public bool Ducky
		{
			get { return ducky; }
			set { ducky = value; }
		}
		public bool GenWarnings
		{
			get { return genwarnings; }
			set { genwarnings = value; }
		}
		public string Culture
		{
			get { return culture; }
			set { culture = value; }
		}
	}
}
