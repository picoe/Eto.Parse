using System;
using System.Collections.Generic;
using System.Text;

namespace Eto.Parse.Samples.Markdown
{
	public class HtmlElementParser : Parser
	{
		readonly StringBuilder tagNameBuilder = new StringBuilder();

		public bool MatchContent { get; set; }

		static readonly HashSet<string> voidTags = new HashSet<string>
		{
			"area", "base", "br", "col", "command", "embed", "hr", "img", "input",
			"keygen", "link", "meta", "param", "source", "track", "wbr"
		};

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
							if (scanner.ReadChar() == -1)
							{
								scanner.Position = pos;
								return -1;
							}
							length++;
						}
						return length;
					}
					scanner.Position = pos;
					return -1;
				}
				tagNameBuilder.Clear();
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
				if (ch == ' ' || ch == '>' || ch == '/')
				{
					var isVoid = voidTags.Contains(tagNameBuilder.ToString());
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
					if (isVoid || prev == '/') // self closing, or a tag with no content
					{
						return length;
					}

					// now, read till the end tag
					if (MatchContent && Name != null)
						args.Push();
					var start = pos + length;
					var contentLength = 0;
					tagNameBuilder.Insert(0, "</");
					tagNameBuilder.Append(">");
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
							args.PopMatch(this, pos, length);
					}
					return length;
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
