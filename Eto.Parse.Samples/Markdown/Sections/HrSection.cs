using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class HrSection : SequenceParser, IMarkdownReplacement
	{
		public HrSection()
		{
			Name = "hr";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var prefix = Terms.sporht.Repeat(0, 3);
			var dash = Terminals.Literal("-").Repeat(3).SeparatedBy(Terms.ows).Until(Terms.eolorf);
			var star = Terminals.Literal("*").Repeat(3).SeparatedBy(Terms.ows).Until(Terms.eolorf);
			var underscore = Terminals.Literal("_").Repeat(3).SeparatedBy(Terms.ows).Until(Terms.eolorf);
			this.Add(prefix, dash | star | underscore, -Terms.blankLine);
		}

		#if PERF_TEST
		protected override int InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			args.Output.AppendUnixLine("<hr />");
		}
	}
}

