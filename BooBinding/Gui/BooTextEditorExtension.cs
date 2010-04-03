using System;
using MonoDevelop.Ide.Gui.Content;
using System.IO;
using MonoDevelop.Projects.Gui.Completion;

namespace BooBinding
{
	public class BooTextEditorExtension : CompletionTextEditorExtension
	{
		public override bool ExtendsEditor(MonoDevelop.Ide.Gui.Document doc, IEditableTextBuffer editor)
		{
			return doc.FileName.Extension == ".boo";
		}
		
		public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar)
		{
			throw new Exception("blarg");
			/*if (completionChar != '.' && completionChar != ' ')
			{
				return null;
			}
			var expressionFinder = new ExpressionFinder();
			var caretLine = completionContext.TriggerLine + 1;
			var caretCol = completionContext.TriggerLineOffset + 1;
			var i = completionContext.TriggerLineOffset;
			if (find_previous_token("=", ref i))
			{
				var p_ctx = GetParserContext();
				var expr = expressionFinder.FindExpression(Editor.GetText(0, i), i, -2).Expression;
				var dataProvider = new CodeCompletionDataProvider(p_ctx, GetAmbience());
				var resolver = new Resolver(p_ctx);
				return dataProvider;
			}
			expr = expressionFinder.FindExpression(Editor.GetText(0, completionContext.TriggerOffset), completionContext.TriggerLineOffset - 2).Expression;
			if (!expr)
			{
				return null;
			}
			p_ctx = GetParserContext();
			completion_prov = new CodeCompletionDataProvider(p_ctx, GetAmbience());
			
			if (completionChar == ' ')
			{
				if (expr == "is" || expr == "as")
				{
					expr = expressionFinder.FindExpression(Editor.GetText(0, completionContext.TriggerOffset), completionContext.TriggerLineOffset - 5).Expression;
					if (expr.Length > 0)
					{
						var res = new Resolver(p_ctx);
						completion_prov.AddResolveResults(res.IsAsResolve(expr, caretLine, caretCol, FileName, Editor.Text, false));
					}
				}
				else if (expr == "import" || expr.EndsWith(" import") || expr.EndsWith("\timport") || expr.EndsWith("\nimport") || expr.EndsWith("\rimport"))
				{
					var namespaces = p_ctx.GetNamespaceList("", true, true);
					completion_prov.AddResolveResults(new ResolveResult(namespaces));
				}
			}
			else
			{
				var resolveResult = p_ctx.Resolve(expr, caretLine, caretCol, FileName, Editor.Text);
				completion_prov.AddResolveResults(resolveResult, false);
			}
			if (competion_prov.IsEmpty)
			{
				return null;
			}
			return completion_prov;*/
		}
		
		private bool find_previous_token(string token, ref int i)
		{
			var s = Editor.GetText(i - 1, i);
			while (s.Length > 0 && (s[0] == ' ' || s[0] == '\t'))
			{
				i--;
				s = Editor.GetText(i - 1, i);
			}
			if (s.Length == 0)
			{
				return false;
			}
			i -= token.Length;
			return Editor.GetText(i, i + token.Length) == token;
		}
	}
}
