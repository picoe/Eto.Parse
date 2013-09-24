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
			length++;
			if (ch == '<')
			{
				ch = scanner.ReadChar();
				length++;
				if (ch == '!')
				{
					length = 7;
					// read comment
					if (scanner.ReadString("--", true))
					{
						while (!scanner.ReadString("-->", true))
						{
							if (scanner.Advance(1) < 0)
							{
								scanner.Position = pos;
								return -1;
							}
							length++;
						}
						length += 3;
						return length;
					}
					scanner.Position = pos;
					return -1;
				}
				tagNameBuilder.Clear();
				tagNameBuilder.Append("</");
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
					ch = scanner.ReadChar();
					length++;
				}
				if (ch == ' ' || ch == '>')
				{
					tagNameBuilder.Append('>');
					int prev = ch;
					while (ch != '>' && ch != -1)
					{
						prev = ch;
						if (ch == '\'' || ch == '"')
						{
							var quote = ch;
							ch = scanner.ReadChar();
							length++;
							while (ch != quote && ch != -1)
							{
								if (ch == '\n')
								{
									scanner.Position = pos;
									return -1;
								}
								ch = scanner.ReadChar();
								length++;
							}
						}
						ch = scanner.ReadChar();
						length++;
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
							ch = scanner.Peek();
							if (ch == '<')
							{
								if (MatchContent && contentLength > 0)
								{
									args.AddMatch(this, start, contentLength, "content");
								}
								var inner = Parse(args);
								if (inner < 0)
								{
									scanner.Position = pos;
									return -1;
								}
								length += inner;
								start = pos + length;
								contentLength = 0;
								continue;
							}
							if (scanner.Advance(1) >= 0)
							{
								length++;
								contentLength++;
							}
							else
							{
								scanner.Position = pos;
								return -1;
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
