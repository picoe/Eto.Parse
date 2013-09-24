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
		IEnumerable<IMarkdownReplacement> sharedReplacements;
		ReplacementParser sharedReplacementParser;
		MarkdownGrammar grammar;

		IEnumerable<IMarkdownReplacement> GetReplacements()
		{
			yield return new BreakEncoding();
			yield return new HtmlEncoding();
			yield return new LinkEncoding();
			yield return new BoldEncoding();
			yield return new ItalicEncoding();
			yield return new EscapeEncoding();
			yield return new CodeEncoding();
		}

		public ReplacementParser Replacements()
		{
			return sharedReplacementParser;
		}

		public ReplacementParser ReplacementsWithAnyChar()
		{
			return replacements;
		}

		public ReplacementParser ReplacementsExcept<T>()
			where T: IMarkdownReplacement
		{
			var rep = new ReplacementParser(grammar);
			rep.Add(sharedReplacements.Where(r => !typeof(T).IsAssignableFrom(r.GetType())), false);
			return rep;
		}

		public MarkdownEncoding()
			: base("encoding")
		{
			EnableMatchEvents = false;
			AllowPartialMatch = true;
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			this.grammar = grammar;
			sharedReplacements = GetReplacements().ToArray();
			sharedReplacementParser = new ReplacementParser(grammar);
			sharedReplacementParser.Add(sharedReplacements);
			replacements = new ReplacementParser(grammar);
			replacements.Add(GetReplacements());
			replacements.Add(Terminals.AnyChar);
			Inner = +replacements;
			SetError<Parser>(false);
		}

		public void Transform(MarkdownReplacementArgs args, StringScanner scanner, int index, int length)
		{
			if (length <= 0)
				return;
			var text = scanner.Value;
			var match = Match(new StringScanner(text, index, length));
			if (match.Success)
			{
				ReplaceEncoding(index, index + length, scanner, match.Matches, args);
				return;
			}
			args.Output.Append(text, index, length);
		}

		public void Transform(MarkdownReplacementArgs args, Match match)
		{
			Transform(args, (StringScanner)match.Scanner, match.Index, match.Length);
		}

		public void Transform(MarkdownReplacementArgs args, Match match, int indexOffset, int lengthOffset)
		{
			Transform(args, (StringScanner)match.Scanner, match.Index + indexOffset, match.Length + lengthOffset);
		}

		public void Transform(MarkdownReplacementArgs args, string text)
		{
			if (string.IsNullOrEmpty(text))
				return;
			var scanner = new StringScanner(text);
			var match = Match(scanner);
			if (match.Success)
			{
				ReplaceEncoding(0, text.Length, scanner, match.Matches, args);
				return;
			}
			args.Output.Append(text);
		}

		public void ReplaceEncoding(MarkdownReplacementArgs args, Match match)
		{
			if (match.Success)
				ReplaceEncoding(match.Index, match.Index + match.Length, match.Scanner, match.Matches, args);
			else
				args.Output.Append(match.Text);
		}

		public void ReplaceEncoding(int index, int end, Scanner scanner, MatchCollection matches, MarkdownReplacementArgs args)
		{
			int last = index;
			var count = matches.Count;
			for (int i = 0; i < count; i++)
			{
				var replacementMatch = matches[i];
				var replacement = replacements.GetReplacement(replacementMatch.Name);

				if (replacementMatch.Index > last)
				{
					args.Output.Append(scanner.Substring(last, replacementMatch.Index - last));
				}
				last = replacementMatch.Index + replacementMatch.Length;

				replacement.Transform(replacementMatch, args);
			}
			if (last < end)
				args.Output.Append(scanner.Substring(last, end - last));
		}

		static Regex apersands = new Regex(@"&(?!((#[0-9]+)|(#[xX][a-fA-F0-9]+)|([a-zA-Z][a-zA-Z0-9]*));)",  RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		static Regex backslashes = new Regex(@"(?<=[^\\])[\\]([\\`*_{}[\]()#+-.!])", RegexOptions.Compiled);

		internal static string EncodeAttribute(string attribute)
		{
			attribute = attribute.Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;");
			attribute = backslashes.Replace(attribute, "$1");
			attribute = apersands.Replace(attribute, "&amp;");
			return attribute;
		}

		internal static string Encode(string attribute)
		{
			attribute = attribute.Replace("\t", "    ").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;");
			attribute = apersands.Replace(attribute, "&amp;");
			attribute = backslashes.Replace(attribute, "$1");
			return attribute;
		}
	}
}