using System;
using Eto.Parse.Parsers;
using System.Text;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class BlockQuoteSection : RepeatParser, IMarkdownReplacement
	{
		MarkdownGrammar grammar;
		public BlockQuoteSection()
		{
			Name = "bq";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			this.grammar = grammar;
			var prefix = (grammar.Prefix & -Terms.sp) & Terminals.Literal(">");
			var value = new RepeatParser(1).Until(Terms.eolorf, true);
			prefix.Name = "prefix";
			value.Name = "value";
			this.Inner =  prefix.Separate() & Terms.ows & value & -Terms.blankLine;
			this.Minimum = 1;
		}

		#if PERF_TEST
		protected override int InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			args.Output.AppendUnixLine("<blockquote>");
			var str = new StringBuilder(match.Length);
			for (int i = 0; i < match.Matches.Count - 1; i += 2)
			{
				var prefix = match.Matches[i];
				var line = match.Matches[i+1];
				str.AppendUnixLine(line.Text);
				//args.Encoding.Replace(line.Text, args);
			}
			args.Output.Append(grammar.Transform(str.ToString()));
			args.Output.AppendUnixLine("</blockquote>");
		}
	}
}

