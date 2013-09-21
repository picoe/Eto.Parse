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
			var inner = grammar.Encoding.ReplacementsExcept<ItalicEncoding>();
			Add("*" & -Terms.sporht & Terms.sporht.Not() & +(inner | Terminals.AnyChar.Except("*")) & "*");
			Add("_" & -Terms.sporht & Terms.sporht.Not() & +(inner | Terminals.AnyChar.Except("_")) & "_");
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<em>");
			args.Encoding.Transform(args, match, 1, -2);
			args.Output.Append("</em>");
		}
	}
	
}