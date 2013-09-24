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
			this.Add(("&" & ~(+Terminals.Letter & ";")), Terminals.Eol, Terminals.Set("<>\""), Terminals.Literal("\\") & Terminals.Set("&><\\`*_{}[]()#+-.!"));
		}

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			var text = match.Text;
			var ch = text[0];
			if (ch == '<')
				args.Output.Append("&lt;");
			else if (ch == '>')
				args.Output.Append("&gt;");
			else if (ch == '\"')
				args.Output.Append("&quot;");
			else if (ch == '&')
				args.Output.Append("&amp;");
			else if (ch == '\r')
				args.Output.Append('\n');
			else if (ch == '\t')
				args.Output.Append("    ");
			else if (ch == '\\')
			{
				ch = text[1];
				if (ch == '<')
					args.Output.Append("&lt;");
				else if (ch == '>')
					args.Output.Append("&gt;");
				else if (ch == 'r')
					args.Output.AppendUnixLine();
				else
					args.Output.Append(ch);
			}
			else
				args.Output.Append(text);
		}

		#if PERF_TEST
		protected override int InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

	}
	
}