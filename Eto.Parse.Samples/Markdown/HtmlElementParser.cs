using System;
using Eto.Parse.Parsers;
using System.Text;
using System.Linq;
using System.Net;

namespace Eto.Parse.Samples.Markdown
{
	public class HtmlElementParser : Parser
	{
		StringBuilder tagNameBuilder = new StringBuilder();
		public bool MatchContent { get; set; }

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			var pos = scanner.Position;
			int length = 0;
			int ch;

			ch = scanner.ReadChar();
			if (ch == '<')
			{
				ch = scanner.ReadChar();
				if (ch == '!')
				{
					length = 7;
					// read comment
					if (scanner.ReadString("--", true))
					{
						while (!scanner.ReadString("-->", true))
						{
							length++;
							if (scanner.Advance(1) == -1)
							{
								scanner.Position = pos;
								return ParseMatch.None;
							}
						}
						return new ParseMatch(pos, length);
					}
					scanner.Position = pos;
					return ParseMatch.None;
				}
				tagNameBuilder.Clear();
				tagNameBuilder.Append("</");
				length++;
				var first = true;
				while (ch != -1 && char.IsLetter((char)ch))
				{
					if (first && !char.IsLetter((char)ch))
					{
						scanner.Position = pos;
						return ParseMatch.None;
					}
					first = false;
					tagNameBuilder.Append((char)ch);
					length++;
					ch = scanner.ReadChar();
				}
				if (ch == ' ' || ch == '>')
				{
					tagNameBuilder.Append('>');
					length++;
					int prev = ch;
					while (ch != '>' && (ch = scanner.ReadChar()) != -1)
					{
						length++;
						prev = ch;
					}
					if (ch != '>')
					{
						scanner.Position = pos;
						return ParseMatch.None;
					}
					if (prev == '/') // self closing
					{
						return new ParseMatch(pos, length);
					}

					if (ch != -1)
					{
						if (MatchContent && Name != null)
							args.Push();
						var start = pos + length;
						var contentLength = 0;
						var tagName = tagNameBuilder.ToString();
						while (!scanner.ReadString(tagName, true))
						{
							ch = scanner.ReadChar();
							if (ch == -1)
								break;
							length++;
							contentLength++;
							if (ch == '<')
							{
								if (MatchContent && contentLength > 0)
								{
									args.AddMatch(this, new ParseMatch(start, contentLength), "content");
								}
								var inner = Parse(args);
								if (!inner.Success)
									break;
								length += inner.Length;
								start = pos + length;
								contentLength = 0;
							}
						}
						length += tagName.Length;
						var match = new ParseMatch(pos, length);
						if (MatchContent)
						{
							if (contentLength > 0)
							{
								args.AddMatch(this, new ParseMatch(start, contentLength), "content");
							}
							if (Name != null)
								args.PopMatch(this, match, Name);
						}
						return match;
					}
				}
			}
			scanner.Position = pos;
			return ParseMatch.None;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			throw new NotImplementedException();
		}
	}
	
}
