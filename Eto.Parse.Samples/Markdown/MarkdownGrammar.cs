using System;
using Eto.Parse.Parsers;
using System.Text;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Eto.Parse.Samples.Markdown.Sections;

namespace Eto.Parse.Samples.Markdown
{

	public class MarkdownGrammar : Grammar
	{
		ReplacementParser replacements;
		ReplacementParser replacementsOnly;
		MarkdownEncoding encoding;

		IEnumerable<IMarkdownReplacement> GetReplacements()
		{
			yield return new CodeSection();
			yield return new HeaderSection();
			yield return new DoubleLineHeaderSection();
			yield return new HrSection();
			yield return new ListSection();
			yield return new BlockQuoteSection();
			yield return new HtmlSection();
			yield return new ReferenceSection();
			yield return new ParagraphSection();
		}

		public MarkdownEncoding Encoding { get { return encoding; } }

		public ReplacementParser Replacements { get { return replacements; } }

		class InnerReplacements : RepeatParser
		{
			protected override int InnerParse(ParseArgs args)
			{
				((MarkdownGrammar)args.Grammar).IndentLevel++;
				var match = base.InnerParse(args);
				((MarkdownGrammar)args.Grammar).IndentLevel--;
				return match;
			}
		}

		public Parser CreateInnerReplacements()
		{
			var rep = new InnerReplacements();
			rep.Minimum = 1;
			rep.Inner = replacementsOnly;
			return rep;
		}

		RepeatParser indent;
		RepeatParser prefix;
		RepeatParser prefixsp;

		public int IndentLevel
		{
			get { return prefix.Minimum; }
			set {
				indent.Minimum = indent.Maximum = value + 1; 
				prefix.Minimum = prefix.Maximum = value;
				prefixsp.Minimum = prefixsp.Maximum = value;
			}
		}

		public Parser Indent { get { return indent; } }

		public Parser Prefix { get { return prefix; } }

		public Parser PrefixSpOrHt { get { return prefixsp; } }

		public MarkdownGrammar()
			: base("markdown")
		{
			EnableMatchEvents = false;
			indent = new RepeatParser(Terms.indent, 1, 1);
			prefix = new RepeatParser(Terms.indent, 0, 0);
			prefixsp = new RepeatParser(Terms.sporht, 0, 0);
			encoding = new MarkdownEncoding();
			encoding.Initialize(this);

			replacementsOnly = new ReplacementParser(this);
			replacements = new ReplacementParser(this);

			var reps = GetReplacements().ToArray();
			replacementsOnly.Add(reps, false);

			replacements.Add(reps, true);
			replacements.Add(Terminals.AnyChar);

			this.Inner = -replacements;
			SetError<Parser>(false);
		}

		public string Transform(string markdown)
		{
			var match = Match(markdown);
			if (match.Success)
			{
				var args = new MarkdownReplacementArgs
				{
					Output = new StringBuilder(markdown.Length),
					Grammar = this,
					Encoding = encoding
				};
				Transform(match, args);
				return args.Output.ToString();
			}
			else
				return markdown;
		}

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			var reference = new ReferenceSection();
			foreach (var refMatch in match.Find(reference.Name))
			{
				reference.AddReference(refMatch, args);
			}

			var count = match.Matches.Count;
			for (int i = 0; i < count; i++)
			{
				var section = match.Matches[i];
				var replacement = replacements.GetReplacement(section.Name);
				replacement.Transform(section, args);
			}
		}

		public void TransformReplacement(Match match, MarkdownReplacementArgs args)
		{
			var replacement = replacements.GetReplacement(match.Name);
			replacement.Transform(match, args);
		}
	}
}

