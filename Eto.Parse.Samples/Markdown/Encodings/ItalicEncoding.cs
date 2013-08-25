using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class ItalicEncoding : SequenceParser, IMarkdownReplacement
	{
		public ItalicEncoding()
		{
			Name = "italic";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			Add("*", new RepeatParser(1).Until("*" | Terms.eolorf), "*");
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
			var text = match.Text;
			args.Encoding.Replace(text.Substring(1, text.Length - 2) , args);
			args.Output.Append("</em>");
		}
	}
	
}