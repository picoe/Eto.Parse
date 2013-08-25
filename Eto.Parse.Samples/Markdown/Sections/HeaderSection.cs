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
			var prefix = +Terminals.Literal("#");
			var value = (+Terms.word).SeparatedBy(Terms.ws).Until(Terms.ows & -Terminals.Literal("#") & Terms.eolorf, true);
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

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			var level = match.Matches[0].Length;
			args.Output.Append("<h");
			args.Output.Append(level);
			args.Output.Append(">");
			args.Encoding.Replace(match.Matches[1].Text, args);
			args.Output.Append("</h");
			args.Output.Append(level);
			args.Output.AppendLine(">");
			args.Output.AppendLine();
		}
	}
}

