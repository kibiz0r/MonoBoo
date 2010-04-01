using System;
using MonoDevelop.Ide.Gui.Content;
using System.IO;

namespace BooBinding.Gui
{
	public class BooEditorCompletion : CompletionTextEditorExtension
	{
		public override bool ExtendsEditor(MonoDevelop.Ide.Gui.Document doc, IEditableTextBuffer editor)
		{
			return doc.FileName.Extension == ".boo";
		}
	}
}
