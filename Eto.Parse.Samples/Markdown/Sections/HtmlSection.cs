using System;
using System.Text;
using Eto.Parse.Parsers;
using Eto.Parse.Samples.Markdown.Encodings;

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
			var text = match.Matches[0].Text;
			if (text.StartsWith("<script") || text.Contains("javascript:"))
				args.Output.AppendUnixLine(MarkdownEncoding.Encode(text));
			else
				args.Output.AppendUnixLine(text);
		}
	}
}

