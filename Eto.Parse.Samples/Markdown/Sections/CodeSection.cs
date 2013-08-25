using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class CodeSection : RepeatParser, IMarkdownReplacement
	{
		public CodeSection()
		{
			Name = "code";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var indent = (Terms.sp * 4) | Terms.ht;

			var content = -indent & new RepeatParser().Until(Terms.ows & (Terms.eol | Terms.eof));
			var line = indent & content & -Terms.sp;
			Inner = line;
			this.Minimum = 1;
			this.SeparatedBy(Terms.eol).Until(Terms.EndOfSection(line.Not()), true);
			content.Name = "content";
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<pre><code>");
			var count = match.Matches.Count;
			for (int i = 0; i < count; i++)
			{
				args.Output.AppendLine(match.Matches[i].Text);
			}
			args.Output.AppendLine("</code></pre>");
			args.Output.AppendLine();
		}
	}
	
}