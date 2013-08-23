using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class BoldEncoding : MarkdownReplacement
	{
		public override string Name { get { return "bold"; } }

		public override Parser GetParser(MarkdownGrammar grammar)
		{
			return ("**" & (+Terminals.AnyChar).Until("**") & "**").WithName(Name);
		}

		public override void Replace(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<strong>");
			var text = match.Text;
			args.Encoding.Replace(text.Substring(2, text.Length - 4) , args);
			args.Output.Append("</strong>");
		}
	}
	
}