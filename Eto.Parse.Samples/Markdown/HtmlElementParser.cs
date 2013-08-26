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
			char ch;
			bool hasChar;

			if (scanner.ReadChar(out ch))
			{
				if (ch == '<')
				{
					hasChar = scanner.ReadChar(out ch);
					if (hasChar && ch == '!')
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
									scanner.SetPosition(pos);
									return ParseMatch.None;
								}
							}
							return new ParseMatch(pos, length);
						}
						scanner.SetPosition(pos);
						return ParseMatch.None;

					}
					tagNameBuilder.Clear();
					tagNameBuilder.Append("</");
					length++;
					var first = true;
					while (char.IsLetter(ch) && hasChar)
					{
						if (first && !char.IsLetter(ch))
						{
							scanner.SetPosition(pos);
							return ParseMatch.None;
						}
						first = false;
						tagNameBuilder.Append(ch);
						length++;
						hasChar = scanner.ReadChar(out ch);
					}
					if (hasChar && (ch == ' ' || ch == '>'))
					{
						tagNameBuilder.Append('>');
						length++;
						char prev = ch;
						while (ch != '>' && (hasChar = scanner.ReadChar(out ch)))
						{
							length++;
							prev = ch;
						}
						if (ch != '>')
						{
							scanner.SetPosition(pos);
							return ParseMatch.None;
						}
						if (prev == '/') // self closing
						{
							return new ParseMatch(pos, length);
						}

						if (hasChar)
						{
							if (MatchContent && Name != null)
								args.Push();
							var start = pos + length;
							var contentLength = 0;
							var tagName = tagNameBuilder.ToString();
							while (!scanner.ReadString(tagName, true))
							{
								if (!scanner.ReadChar(out ch))
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
				scanner.SetPosition(pos);
			}
			return ParseMatch.None;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			throw new NotImplementedException();
		}
	}
	
}
