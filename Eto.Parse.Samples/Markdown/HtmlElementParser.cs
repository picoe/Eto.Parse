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
			bool done;

			if (scanner.ReadChar(out ch))
			{
				if (ch == '<')
				{
					tagNameBuilder.Clear();
					tagNameBuilder.Append("</");
					length++;
					var first = true;
					while (done = scanner.ReadChar(out ch) && char.IsLetter(ch))
					{
						if (first && !char.IsLetter(ch))
						{
							scanner.SetPosition(pos);
							return ParseMatch.None;
						}
						first = false;
						tagNameBuilder.Append(ch);
						length++;
					}
					if (!done)
					{
						tagNameBuilder.Append('>');
						length++;
						char prev = ch;
						while (ch != '>' && (done = scanner.ReadChar(out ch)))
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

						if (!done)
						{
							if (MatchContent)
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
										args.PopMatch(this, new ParseMatch(start, contentLength), "content");
										args.Push();
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
							if (MatchContent)
							{
								if (contentLength > 0)
									args.PopMatch(this, new ParseMatch(start, contentLength), "content");
								else
									args.PopSuccess();
							}
							return new ParseMatch(pos, length);
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
