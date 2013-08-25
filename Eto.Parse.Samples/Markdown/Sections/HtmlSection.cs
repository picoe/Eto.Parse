using System;
using System.Text;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class HtmlSection : SequenceParser, IMarkdownReplacement
	{
		public HtmlSection()
		{
			Name = "html";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			Add(new HtmlElementParser { Name = "html" }, -Terms.blankLine);
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			args.Output.AppendLine(match.Text);
			args.Output.AppendLine();
		}
	}
}

