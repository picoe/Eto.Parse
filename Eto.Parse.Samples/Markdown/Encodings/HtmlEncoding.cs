using System;
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
		protected override int InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
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
					args.Output.Append(text, last, content.Index - match.Index - last);
				}
				last = content.Index - match.Index + content.Length;

				args.Encoding.Transform(args, content);
			}
			if (last < text.Length)
				args.Output.Append(text, last, text.Length - last);
		}
	}
}

