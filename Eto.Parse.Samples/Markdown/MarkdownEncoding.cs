using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;
using Eto.Parse.Samples.Markdown.Encodings;
using System.Linq;

namespace Eto.Parse.Samples.Markdown
{
	public class MarkdownEncoding : Grammar
	{
		ReplacementParser replacements;

		IEnumerable<IMarkdownReplacement> Replacements
		{
			get
			{
				yield return new HtmlEncoding();
				yield return new LinkEncoding();
				yield return new BoldEncoding();
				yield return new ItalicEncoding();
				yield return new CodeEncoding();
				yield return new EscapeEncoding();
			}
		}

		public MarkdownEncoding(MarkdownGrammar grammar)
			: base("encoding")
		{
			EnableMatchEvents = false;
			AllowPartialMatch = true;

			replacements = new ReplacementParser(grammar, Replacements);

			replacements.Add(Terminals.AnyChar);

			Inner = +replacements;
			SetError<Parser>(false);
		}

		public void Replace(string text, MarkdownReplacementArgs args)
		{
			if (string.IsNullOrEmpty(text))
				return;
			/**/
			var match = Match(text);
			if (match.Success)
			{
				ReplaceEncoding(text, match.Matches, args);
				return;
			}
			/**
			var matches = Matches(text);
			if (matches.Count > 0)
			{
				ReplaceEncoding(text, matches, args);
				return;
			}
			/**/
			args.Output.Append(text);
		}

		void ReplaceEncoding(string text, MatchCollection matches, MarkdownReplacementArgs args)
		{
			int last = 0;
			var count = matches.Count;
			for (int i = 0; i < count; i++)
			{
				var replacementMatch = matches[i];
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