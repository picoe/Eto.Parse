using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class EscapeEncoding : MarkdownReplacement
	{
		public override string Name { get { return "escape"; } }

		public override Parser GetParser(MarkdownGrammar grammar)
		{
			return (("&" & ~(+Terminals.Letter & ";")) | "<").WithName(Name);
		}

		public override void Replace(Match match, MarkdownReplacementArgs args)
		{
			if (match.Text == "<")
				args.Output.Append("&lt;");
			else if (match.Text == "&")
				args.Output.Append("&amp;");
			else
				args.Output.Append(match.Text);
		}

		//static Regex apersands = new Regex(@"&(?!((#[0-9]+)|(#[xX][a-fA-F0-9]+)|([a-zA-Z][a-zA-Z0-9]*));)",  RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		//static Regex ltangles = new Regex(@"<(?![A-Za-z/?\$!])", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		public static string Encode(string val)
		{
			//val = apersands.Replace(val, "&amp;");
			//val = ltangles.Replace(val, "&lt;");
			return val;
		}


	}
	
}