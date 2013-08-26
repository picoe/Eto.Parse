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
			Add("**" & -Terms.sporht & Terms.sporht.Not() & new RepeatParser(1).Until(("**" & Terminals.Literal("*").Not()) | Terms.eolorf) & "**");
			Add("__" & -Terms.sporht & Terms.sporht.Not() & new RepeatParser(1).Until(("__" & Terminals.Literal("_").Not()) | Terms.eolorf) & "__");
		}

#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
#endif

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<strong>");
			args.Encoding.Replace(args, match, 2, -4);
			args.Output.Append("</strong>");
		}
	}
}