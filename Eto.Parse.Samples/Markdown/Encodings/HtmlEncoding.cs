using System;
using System.Text;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class HtmlEncoding : MarkdownReplacement
	{
		public override Parser GetParser(MarkdownGrammar grammar)
		{
			return new HtmlElementParser { Name = Name, MatchContent = true } & -Terms.blankLine;
		}

		public override void Replace(Match match, MarkdownReplacementArgs args)
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

		public override string Name { get { return "htmlcontent"; } }
	}
}

