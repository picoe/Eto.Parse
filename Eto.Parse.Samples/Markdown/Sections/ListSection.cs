using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class ListSection : RepeatParser, IMarkdownReplacement
	{
		public ListSection()
		{
			Name = "list";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var prefix = Terms.sp.Repeat(0, 3);
			var star = ((Parser)"*" | "-" | "+").WithName("star");
			var num = (+Terminals.Digit).WithName("num") & ".";
			var content = new RepeatParser(1).Until(Terms.ows & Terms.eolorf);
			content.Name = "content";
			var line = (prefix & (num | star) & Terms.ows & content).WithName("line");
			var spacing = (-Terms.blankLine).WithName("spacing");
			var lineWithSpacing = line & Terms.blankLine & spacing;
			Inner = lineWithSpacing;
			Minimum = 1;
			this.Until(Terms.EndOfSection(line.Not()));
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			string suffix = null;
			bool lastSpace = false;
			var count = match.Matches.Count;
			for (int i = 0; i < count; i++)
			{
				var item = match.Matches[i];
				if (i == 0)
				{
					if (item["num"])
					{
						args.Output.AppendUnixLine("<ol>");
						suffix = "</ol>";
					}
					else
					{
						args.Output.AppendUnixLine("<ul>");
						suffix = "</ul>";
					}
				}
				var addSpace = i < count - 1 && item["spacing"].Length > 0;
				args.Output.Append("<li>");
				if (addSpace || lastSpace)
					args.Output.Append("<p>");
				var content = item.Matches["content"];
				args.Encoding.Replace(args, content);
				if (addSpace || lastSpace)
					args.Output.Append("</p>");
				args.Output.AppendUnixLine("</li>");
				lastSpace = addSpace;
			}
			args.Output.AppendUnixLine(suffix);
			args.Output.AppendUnixLine();
			args.Output.AppendUnixLine();
		}
	}
}