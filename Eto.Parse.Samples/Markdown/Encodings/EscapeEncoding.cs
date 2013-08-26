using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class EscapeEncoding : AlternativeParser, IMarkdownReplacement
	{
		public EscapeEncoding()
		{
			Name = "escape";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			this.Add(("&" & ~(+Terminals.Letter & ";")), Terminals.Set("<>\""), Terminals.Literal("\\") & Terminals.Set("&><\\`*_{}[]()#+-.!"));
		}

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			var ch = match.Text[0];
			if (ch == '<')
				args.Output.Append("&lt;");
			else if (ch == '>')
				args.Output.Append("&gt;");
			else if (ch == '\"')
				args.Output.Append("&quot;");
			else if (ch == '&')
				args.Output.Append("&amp;");
			else if (ch == '\\')
			{
				ch = match.Text[1];
				if (ch == '<')
					args.Output.Append("&lt;");
				else if (ch == '>')
					args.Output.Append("&gt;");
				else
					args.Output.Append(match.Text[1]);
			}
			else
				args.Output.Append(match.Text);
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

	}
	
}