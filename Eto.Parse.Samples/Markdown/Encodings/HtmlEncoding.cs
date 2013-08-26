using System;
using System.Text;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class HtmlEncoding : SequenceParser, IMarkdownReplacement
	{
		public HtmlEncoding()
		{
			Name = "html";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			Add(new HtmlElementParser { MatchContent = true, Name = "html" }, -Terms.blankLine);
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			var html = match.Matches[0];
			var text = html.Text;
			if (text.StartsWith("<script") || text.Contains("javascript:"))
			{
				args.Output.AppendUnixLine(MarkdownEncoding.Encode(text));
				return;
			}

			int last = 0;
			var matches = html.Matches;
			for (int i = 0; i < matches.Count; i++)
			{
				var content = matches[i];
				if (content.Index - match.Index > last)
				{
					args.Output.Append(text.Substring(last, content.Index - match.Index - last));
				}
				last = content.Index - match.Index + content.Length;

				args.Encoding.Replace(args, content);
			}
			if (last < text.Length)
				args.Output.Append(text.Substring(last, text.Length - last));
		}
	}
}

