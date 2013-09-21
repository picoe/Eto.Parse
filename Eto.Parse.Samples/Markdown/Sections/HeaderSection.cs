using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class HeaderSection : SequenceParser, IMarkdownReplacement
	{
		public HeaderSection()
		{
			Name = "header";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var prefix = Terminals.Literal("#").Repeat(1);
			var value = Terminals.Set("\n\r").Inverse().Repeat(1).Until(Terms.ows & -Terminals.Literal("#") & Terms.eolorf, true);
			prefix.Name = "prefix";
			value.Name = "value";
			this.Add(prefix, Terms.ows, value, -Terms.blankLine);
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			var level = Math.Min(match.Matches[0].Length, 6);
			args.Output.Append("<h");
			args.Output.Append(level);
			args.Output.Append(">");
			args.Encoding.Transform(args, match.Matches[1]);
			args.Output.Append("</h");
			args.Output.Append(level);
			args.Output.AppendUnixLine(">");
			args.Output.AppendUnixLine();
		}
	}
}

