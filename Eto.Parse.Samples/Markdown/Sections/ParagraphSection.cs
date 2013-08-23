using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class ParagraphSection : MarkdownReplacement
	{
		public override string Name { get { return "paragraph"; } }

		public override Parser GetParser(MarkdownGrammar grammar)
		{
			var linePost = -Terms.sp;
			var linePre = Terms.sp.Repeat(0, 3);
			var content = Terms.words & linePost;
			var line =  linePre & content;
			var paragraph = (+line).SeparatedBy(Terms.eol).Until(Terms.EndOfSection(), true);
			paragraph.Name = "paragraph";
			content.Name = "content";

			return paragraph;
		}

		public override void Replace(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<p>");
			var count = match.Matches.Count;
			for (int i = 0; i < count; i++)
			{
				if (i > 0)
					args.Output.AppendLine();
				args.Encoding.Replace(match.Matches[i].Text, args);
			}
			args.Output.Append("</p>");
		}
	}
}