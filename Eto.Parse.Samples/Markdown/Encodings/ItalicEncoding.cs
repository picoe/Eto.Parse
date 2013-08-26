using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class ItalicEncoding : AlternativeParser, IMarkdownReplacement
	{
		public ItalicEncoding()
		{
			Name = "italic";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			Add("*" & -Terms.sporht & Terms.sporht.Not() & new RepeatParser(1).Until(("*" & Terminals.Literal("*").Not()) | Terms.eolorf) & "*");
			Add("_" & -Terms.sporht & Terms.sporht.Not() & new RepeatParser(1).Until(("_" & Terminals.Literal("_").Not()) | Terms.eolorf) & "_");
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<em>");
			args.Encoding.Replace(args, match, 1, -2);
			args.Output.Append("</em>");
		}
	}
	
}