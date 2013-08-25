using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class ParagraphSection : RepeatParser, IMarkdownReplacement
	{
		public ParagraphSection()
		{
			Name = "para";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var linePost = -Terms.sp;
			var linePre = Terms.sp.Repeat(0, 3);
			var content = Terms.words & linePost;
			content.Name = "content";
			var line =  linePre & content;
			Inner = line;
			this.Minimum = 1;
			this.SeparatedBy(Terms.eol).Until(Terms.EndOfSection(), true);
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<p>");
			var count = match.Matches.Count;
			for (int i = 0; i < count; i++)
			{
				if (i > 0)
					args.Output.AppendLine();
				args.Encoding.Replace(match.Matches[i].Text, args);
			}
			args.Output.AppendLine("</p>");
			args.Output.AppendLine();
		}
	}
}