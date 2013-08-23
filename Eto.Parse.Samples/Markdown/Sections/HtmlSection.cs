using System;
using System.Text;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class HtmlSection : MarkdownReplacement
	{
		public override Parser GetParser(MarkdownGrammar grammar)
		{
			return new HtmlElementParser { Name = "html" } & -Terms.blankLine;
		}

		public override void Replace(Match match, MarkdownReplacementArgs args)
		{
			args.Output.AppendLine(match.Text);
		}

		public override string Name { get { return "html"; } }
	}
}

