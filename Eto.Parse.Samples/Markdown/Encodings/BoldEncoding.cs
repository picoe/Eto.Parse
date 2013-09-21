using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class BoldEncoding : AlternativeParser, IMarkdownReplacement
	{
		public bool AddLinesBefore { get { return true; } }

		public BoldEncoding()
		{
			Name = "bold";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var inner = grammar.Encoding.ReplacementsExcept<BoldEncoding>();
			Add("**" & -Terms.sporht & Terms.sporht.Not() & +((inner | Terminals.AnyChar.Except("**"))) & "**");
			Add("__" & -Terms.sporht & Terms.sporht.Not() & +((inner | Terminals.AnyChar.Except("__"))) & "__");
		}

#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<strong>");
			//args.Encoding.Transform(args, match, 2, -4);
			if (match.HasMatches)
				args.Encoding.ReplaceEncoding(args, match);
			else
				args.Output.Append(match.Scanner.Substring(match.Index + 2, match.Length - 4));
			args.Output.Append("</strong>");
		}
	}
}