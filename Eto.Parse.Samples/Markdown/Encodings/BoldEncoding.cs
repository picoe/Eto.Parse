using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class BoldEncoding : AlternativeParser, IMarkdownReplacement
	{
		public bool AddLinesBefore { get { return true; } }

		bool inBold;

		public BoldEncoding()
		{
			Name = "bold";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var inner = grammar.Encoding.Replacements();
			Add("**" & Terms.ows & +((inner | Terminals.AnyChar.Except(Terminals.Set("*\n\r")))) & "**");
			Add("__" & Terms.ows & +((inner | Terminals.AnyChar.Except(Terminals.Set("_\n\r")))) & "__");
		}

		protected override int InnerParse(ParseArgs args)
		{
			if (inBold)
				return -1;
			inBold = true;
			var match = base.InnerParse(args);
			inBold = false;
			return match;
		}

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<strong>");
			//args.Encoding.Transform(args, match, 2, -4);
			if (match.HasMatches)
				args.Encoding.ReplaceEncoding(match.Index + 2, match.Index + match.Length - 2, match.Scanner, match.Matches, args);
			else
				args.Output.Append(match.Scanner.Substring(match.Index + 2, match.Length - 4));
			args.Output.Append("</strong>");
		}
	}
}