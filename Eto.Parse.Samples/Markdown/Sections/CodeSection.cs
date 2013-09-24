using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;
using Eto.Parse.Samples.Markdown.Encodings;

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
			var content = new RepeatParser().Until(Terms.eolorf);
			content.Name = "content";
			var line = grammar.Indent & content;
			Inner = line;
			this.Minimum = 1;
			this.SeparatedBy(+(Terms.ows & Terms.eol.Named("sep"))).Until(Terms.EndOfSection(line.Not()), true);
		}

		#if PERF_TEST
		protected override int InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<pre><code>");
			var count = match.Matches.Count;
			for (int i = 0; i < count; i++)
			{
				var line = match.Matches[i];
				if (line.Name == "sep")
					args.Output.AppendUnixLine();
				else
					args.Output.Append(CodeEncoding.Encode(line.Text));
			}
			args.Output.AppendUnixLine();
			args.Output.AppendUnixLine("</code></pre>");
			args.Output.AppendUnixLine();
		}
	}
	
}