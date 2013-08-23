using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class ListSection : MarkdownReplacement
	{
		public override string Name { get { return "list"; } }

		public override Parser GetParser(MarkdownGrammar grammar)
		{
			var prefix = Terms.sp.Repeat(0, 3);
			var star = ((Parser)"*" | "-" | "+").WithName("star");
			var num = (+Terminals.Digit).WithName("num") & ".";
			var content = new RepeatParser(1).Until(Terms.ows & (Terms.eol | Terms.eof), true);
			var line = (prefix & (num | star) & Terms.ows & content).WithName("line");
			var spacing = (-Terms.blankLine).WithName("spacing");
			var lineWithSpacing = line & Terms.blankLine & spacing;
			var list = (+lineWithSpacing).Until(Terms.EndOfSection(line.Not()), true).WithName(Name);
			content.Name = "content";
			return list;
		}

		public override void Replace(Match match, MarkdownReplacementArgs args)
		{
			args.Output.AppendLine("<ul>");
			string prefix = null;
			string suffix = null;
			bool lastSpace = false;
			var count = match.Matches.Count;
			for (int i = 0; i < count; i++)
			{
				var item = match.Matches[i];
				if (prefix == null)
				{
					if (item.Matches["num"])
					{
						prefix = "<ol>";
						suffix = "</ol>";
					}
					else
					{
						prefix = "<li>";
						suffix = "</li>";
					}
				}
				var addSpace = i < count - 1 && item["spacing"].Length > 0;
				args.Output.Append(prefix);
				if (addSpace || lastSpace)
					args.Output.Append("<p>");
				var content = item.Matches["content"];
				args.Encoding.Replace(content.Text, args);
				if (addSpace || lastSpace)
					args.Output.Append("</p>");
				args.Output.AppendLine(suffix);
				lastSpace = addSpace;
			}
			args.Output.AppendLine("</ul>");
		}
	}
}