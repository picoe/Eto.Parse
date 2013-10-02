using System;
using Eto.Parse.Parsers;
using System.Linq;

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
			var star = ((Parser)"*" | "-" | "+").WithName("star") & Terms.ws;
			var num = (+Terminals.Digit).WithName("num") & "." & Terms.ows;
			var start = (prefix & (num | star)).Separate();

			var content = grammar.Encoding.ReplacementsWithAnyChar().Repeat().Until(Terms.blankLine & (Terms.blankLine | Terms.ows & start));
			content.Name = "content";
			var line = (grammar.PrefixSpOrHt & start & content & Terms.blankLineOrEof).WithName("line");
			var inner = grammar.CreateInnerReplacements().WithName("inner");
			var spacing = (-Terms.blankLine).WithName("spacing");
			var lineWithSpacing = line & ~(-Terms.blankLine & inner) & spacing;
			Inner = lineWithSpacing;
			Minimum = 1;

			this.Until(Terms.EndOfSection(line.Not()), capture: true);
		}
		#if PERF_TEST
		protected override int InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif
		public void Transform(Match match, MarkdownReplacementArgs args)
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
				var inner = item["inner"];

				var hasInnerList = inner.Success && inner.HasMatches && inner.Matches[0].Name == Name;
				var addSpace = (inner.Success && !hasInnerList) || (i < count - 1 && item["spacing"].Length > 0);
				args.Output.Append("<li>");
				if (addSpace || lastSpace)
					args.Output.Append("<p>");
				var content = item.Matches["content"];
				//args.Encoding.Transform(args, content);
				args.Encoding.ReplaceEncoding(args, content);
				if (addSpace || lastSpace)
					args.Output.Append("</p>");

				if (inner.Success)
				{
					args.Grammar.Transform(inner, args);
				}
				args.Output.AppendUnixLine("</li>");
				lastSpace = addSpace;
			}
			args.Output.AppendUnixLine(suffix);
			args.Output.AppendUnixLine();
			args.Output.AppendUnixLine();
		}
	}
}