using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class ItalicEncoding : AlternativeParser, IMarkdownReplacement
	{
		bool inItalic;

		public ItalicEncoding()
		{
			Name = "italic";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var inner = grammar.Encoding.Replacements();
			Add("*" & Terms.ows & +((inner | "**" | Terminals.Set("*\n\r").Inverse())) & "*");
			Add("_" & Terms.ows & +((inner | Terminals.Set("_\n\r").Inverse())) & "_");
		}

		protected override int InnerParse(ParseArgs args)
		{
			if (inItalic)
				return -1;
			inItalic = true;
			var match = base.InnerParse(args);
			inItalic = false;
			return match;
		}

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<em>");
			if (match.HasMatches)
				args.Encoding.ReplaceEncoding(match.Index + 1, match.Index + match.Length - 1, match.Scanner, match.Matches, args);
			else
				args.Output.Append(match.Scanner.Substring(match.Index + 1, match.Length - 2));
			args.Output.Append("</em>");
		}
	}
	
}