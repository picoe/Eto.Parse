using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;
using Eto.Parse.Samples.Markdown.Encodings;
using System.Linq;
using Eto.Parse.Scanners;

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
				yield return new EscapeEncoding();
				yield return new CodeEncoding();
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

		public void Replace(MarkdownReplacementArgs args, StringScanner scanner, int index, int length)
		{
			if (length <= 0)
				return;
			var text = scanner.Value;
			var match = Match(new StringScanner(text, index, length));
			if (match.Success)
			{
				ReplaceEncoding(index, index + length, text, match.Matches, args);
				return;
			}
			args.Output.Append(text.Substring(index, length));
		}

		public void Replace(MarkdownReplacementArgs args, Match match)
		{
			Replace(args, (StringScanner)match.Scanner, match.Index, match.Length);
		}

		public void Replace(MarkdownReplacementArgs args, Match match, int indexOffset, int lengthOffset)
		{
			Replace(args, (StringScanner)match.Scanner, match.Index + indexOffset, match.Length + lengthOffset);
		}

		public void Replace(MarkdownReplacementArgs args, string text)
		{
			if (string.IsNullOrEmpty(text))
				return;
			var match = Match(text);
			if (match.Success)
			{
				ReplaceEncoding(0, text.Length, text, match.Matches, args);
				return;
			}
			args.Output.Append(text);
		}

		void ReplaceEncoding(int index, int length, string text, MatchCollection matches, MarkdownReplacementArgs args)
		{
			int last = index;
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
			if (last < length)
				args.Output.Append(text.Substring(last, length - last));
		}

		static Regex apersands = new Regex(@"&(?!((#[0-9]+)|(#[xX][a-fA-F0-9]+)|([a-zA-Z][a-zA-Z0-9]*));)",  RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		static Regex backslashes = new Regex(@"[^\\][\\]([\\`*_{}[\]()#+-.!])", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		internal static string EncodeAttribute(string attribute)
		{
			attribute = attribute.Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;");
			attribute = apersands.Replace(attribute, "&amp;");
			return attribute;
		}

		internal static string Encode(string attribute)
		{
			attribute = attribute.Replace("\t", "    ").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;");
			attribute = apersands.Replace(attribute, "&amp;");
			attribute = backslashes.Replace(attribute, r => r.Value[1].ToString());
			return attribute;
		}
	}
}