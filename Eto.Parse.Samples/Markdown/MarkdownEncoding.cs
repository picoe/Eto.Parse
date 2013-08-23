using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;
using Eto.Parse.Samples.Markdown.Encodings;

namespace Eto.Parse.Samples.Markdown
{
	public class MarkdownEncoding : Grammar
	{
		ReplacementParser replacements;

		IEnumerable<MarkdownReplacement> Replacements
		{
			get
			{
				yield return new HtmlEncoding();
				yield return new LinkEncoding();
				yield return new BoldEncoding();
				yield return new CodeEncoding();
				yield return new EscapeEncoding();
			}
		}

		public MarkdownEncoding(MarkdownGrammar grammar)
			: base("encoding")
		{
			EnableMatchEvents = false;

			replacements = new ReplacementParser(grammar, Replacements);

			replacements.Add(Terminals.AnyChar);

			Inner = +replacements;
		}

		public void Replace(string text, MarkdownReplacementArgs args)
		{
			if (string.IsNullOrEmpty(text))
				return;
			var match = Match(text);
			if (match.Success)
			{
				ReplaceEncoding(text, match, args);
			}
			else
				args.Output.Append(text);
		}

		public string Replace(string text)
		{
			var args = new MarkdownReplacementArgs
			{
				Output = new StringBuilder(),
				Encoding = this
			};
			Replace(text, args);
			return args.Output.ToString();
		}

		void ReplaceEncoding(string text, Match match, MarkdownReplacementArgs args)
		{
			int last = 0;
			var count = match.Matches.Count;
			for (int i = 0; i < count; i++)
			{
				var replacementMatch = match.Matches[i];
				var replacement = replacements.GetReplacement(replacementMatch.Name);

				if (replacementMatch.Index > last)
				{
					args.Output.Append(text.Substring(last, replacementMatch.Index - last));
				}
				last = replacementMatch.Index + replacementMatch.Length;

				replacement.Replace(replacementMatch, args);
			}
			if (last < text.Length)
				args.Output.Append(text.Substring(last, text.Length - last));
		}

		internal static string HtmlAttributeEncode(string attribute)
		{
			return attribute.Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;");
		}
	}
}