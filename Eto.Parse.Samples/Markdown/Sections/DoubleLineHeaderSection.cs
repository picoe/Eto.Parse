using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class DoubleLineHeaderSection : SequenceParser, IMarkdownReplacement
	{
		public DoubleLineHeaderSection()
		{
			Name = "dlheader";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var prefix = Terms.sp.Repeat(0, 3);
			var value = Terminals.Set("\n\r").Inverse().Repeat(1).WithName("value");
			var suffix = Terminals.Eol & ((+Terminals.Literal("=")).Named("h1") | (+Terminals.Literal("-")).Named("h2")) & Terms.ows & Terminals.Eol;
			this.Add(prefix, value, suffix, -Terms.blankLine);
		}

		#if PERF_TEST
		protected override int InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			var level = match.Matches[1];
			args.Output.Append("<");
			args.Output.Append(level.Name);
			args.Output.Append(">");
			args.Encoding.Transform(args, match.Matches[0]);
			args.Output.Append("</");
			args.Output.Append(level.Name);
			args.Output.AppendUnixLine(">");
			args.Output.AppendUnixLine();
		}
	}
}

