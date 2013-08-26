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
			var linePre = Terms.sp.Repeat(0, 3);
			var linePost = -Terms.sp;
			var content = linePre & Terms.words & linePost;
			content.Name = "content";
			var line = content;
			Inner = line;
			this.Minimum = 1;
			this.SeparatedBy(Terms.eol).Until((Terms.eol & Terminals.Set("#>").NonCaptured()) | Terms.EndOfSection(minLines:2), true);
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
				var text = match.Matches[i].Text;
				if (i > 0)
					args.Output.AppendUnixLine();
				else
					text = text.TrimStart();
				args.Encoding.Replace(args, text);
			}
			args.Output.AppendUnixLine("</p>");
			args.Output.AppendUnixLine();
		}
	}
}