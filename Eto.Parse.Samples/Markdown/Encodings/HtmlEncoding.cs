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
			Add(new HtmlElementParser { Name = Name, MatchContent = true }, -Terms.blankLine);
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Replace(Match match, MarkdownReplacementArgs args)
		{
			int last = 0;
			var text = match.Text;
			var count = match.Matches.Count;
			for (int i = 0; i < count; i++)
			{
				var content = match.Matches[i];
				if (content.Index - match.Index > last)
				{
					args.Output.Append(text.Substring(last, content.Index - match.Index - last));
				}
				last = content.Index - match.Index + content.Length;

				args.Encoding.Replace(content.Text, args);
			}
			if (last < text.Length)
				args.Output.Append(text.Substring(last, text.Length - last));
		}
	}
}

