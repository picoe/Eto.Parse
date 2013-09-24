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

		protected override int InnerParse(ParseArgs args)
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
							if (scanner.Advance(1) < 0)
							{
								scanner.Position = pos;
								return -1;
							}
						}
						return length;
					}
					scanner.Position = pos;
					return -1;
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
						return -1;
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
						return -1;
					}
					if (prev == '/') // self closing
					{
						return length;
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
									args.AddMatch(this, start, contentLength, "content");
								}
								var inner = Parse(args);
								if (inner < 0)
									break;
								length += inner;
								start = pos + length;
								contentLength = 0;
							}
						}
						length += tagName.Length;
						if (MatchContent)
						{
							if (contentLength > 0)
							{
								args.AddMatch(this, start, contentLength, "content");
							}
							if (Name != null)
								args.PopMatch(this, pos, length, Name);
						}
						return length;
					}
				}
			}
			scanner.Position = pos;
			return -1;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			throw new NotImplementedException();
		}
	}
	
}
