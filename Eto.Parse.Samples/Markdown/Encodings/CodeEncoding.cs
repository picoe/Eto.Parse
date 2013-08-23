using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class CodeEncoding : MarkdownReplacement
	{
		public override string Name { get { return "code"; } }

		public class BalancedParser : Parser
		{
			public Parser Surrounding { get; set; }

			protected override ParseMatch InnerParse(ParseArgs args)
			{
				var scanner = args.Scanner;
				var m = Surrounding.Parse(args);
				if (m.Success)
				{
					var pos = m.Index;
					var length = m.Length;
					var str = scanner.SubString(m.Index, m.Length);
					while (!scanner.ReadString(str, true))
					{
						if (scanner.Advance(1) == -1)
						{
							scanner.SetPosition(pos);
							return args.NoMatch;
						}
						length++;
					}
					length += str.Length;
					return new ParseMatch(pos, length);
				}
				return args.NoMatch;
			}

			public override Parser Clone(ParserCloneArgs args)
			{
				throw new NotImplementedException();
			}
		}

		public override Parser GetParser(MarkdownGrammar grammar)
		{
			var parens = +Terminals.Literal("`");
			var code = new BalancedParser { Surrounding = parens } | (parens & new RepeatParser().Until(Terminals.Literal("`"), true) & parens);
			code.Name = "code";
			return code;
		}

		public override void Replace(Match match, MarkdownReplacementArgs args)
		{
			var text = match.Text;
			int start = 0;
			int end = text.Length - 1;
			while (start < text.Length && text[start] == '`')
				start++;
			while (end >= 0 && text[end] == '`')
				end--;
			var len = end - start + 1;
			end = text.Length - end - 1;
			var count = Math.Min(start, end);
			args.Output.Append("<code>");
			if (start > count)
				args.Output.Append(text.Substring(0, start - count));
			args.Output.Append(text.Substring(start, len));
			if (end > count)
				args.Output.Append(text.Substring(start + len, end - count));
			args.Output.Append("</code>");
		}
	}
	
}