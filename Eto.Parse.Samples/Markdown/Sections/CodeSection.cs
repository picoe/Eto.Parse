using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class CodeSection : MarkdownReplacement
	{
		public override string Name { get { return "code"; } }

		public override Parser GetParser(MarkdownGrammar grammar)
		{
			var indent = (Terms.sp * 4) | Terms.ht;

			var content = -indent & new RepeatParser().Until(Terms.ows & (Terms.eol | Terms.eof));
			var line = indent & content & -Terms.sp;
			var code = (+line).SeparatedBy(Terms.eol).Until(Terms.EndOfSection(line.Not()), true);
			code.Name = Name;
			content.Name = "content";

			return code;
		}

		public override void Replace(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<pre><code>");
			var count = match.Matches.Count;
			for (int i = 0; i < count; i++)
			{
				args.Output.AppendLine(match.Matches[i].Text);
			}
			args.Output.Append("</code></pre>");
		}
	}
	
}