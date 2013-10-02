using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class CodeEncoding : AlternativeParser, IMarkdownReplacement
	{
		public CodeEncoding()
		{
			Name = "code";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var parens = +(Terminals.Set("\\").Not() & Terminals.Literal("`"));
			Add(new BalancedParser { Surrounding = parens }/*, (parens & new RepeatParser().Until(Terminals.Literal("`"), true) & parens)*/);
		}

		#if PERF_TEST
		protected override int InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public class BalancedParser : Parser
		{
			public Parser Surrounding { get; set; }

			protected override int InnerParse(ParseArgs args)
			{
				var scanner = args.Scanner;
				var pos = scanner.Position;
				var length = Surrounding.Parse(args);
				if (length >= 0)
				{
					var str = scanner.Substring(pos, length);
					while (!scanner.ReadString(str, true))
					{
						if (scanner.Advance(1) < 0)
						{
							scanner.Position = pos;
							return -1;
						}
						length++;
					}
					length += str.Length;
					return length;
				}
				return -1;
			}

			public override void Initialize(ParserInitializeArgs args)
			{
				base.Initialize(args);
				if (Surrounding != null && args.Push(this))
				{
					Surrounding.Initialize(args);
					args.Pop();
				}
			}

			public override Parser Clone(ParserCloneArgs args)
			{
				throw new NotImplementedException();
			}
		}

		public void Transform(Match match, MarkdownReplacementArgs args)
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
				args.Output.Append(text, 0, start - count);
			var lines = text.Substring(start, len).Trim(' ');
			args.Output.Append(Encode(lines));
			if (end > count)
				args.Output.Append(text, start + len, end - count);
			args.Output.Append("</code>");
		}

		public static string Encode(string code)
		{
			return code.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("\t", "    ");
		}
	}
	
}