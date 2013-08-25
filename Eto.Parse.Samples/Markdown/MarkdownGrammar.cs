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
		MarkdownEncoding encoding;

		public IEnumerable<IMarkdownReplacement> GetReplacements()
		{
			yield return new HtmlSection();
			yield return new HeaderSection();
			yield return new ListSection();
			yield return new ReferenceSection();
			yield return new ParagraphSection();
			yield return new CodeSection();
		}

		public ReplacementParser Replacements { get { return replacements; } }

		public MarkdownGrammar()
			: base("markdown")
		{
			EnableMatchEvents = false;
			encoding = new MarkdownEncoding(this);
			replacements = new ReplacementParser(this, GetReplacements());

			this.Inner = -Terms.blankLine & -replacements & -Terminals.WhiteSpace;
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
					Encoding = encoding
				};
				WriteSection(match, args);
				return args.Output.ToString();
			}
			else
				throw new ArgumentOutOfRangeException("markdown", match.ErrorMessage);
		}

		void WriteSection(Match match, MarkdownReplacementArgs args)
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
				WriteReplacement(section, args);
			}
		}

		public void WriteReplacement(Match match, MarkdownReplacementArgs args)
		{
			var replacement = replacements.GetReplacement(match.Name);
			replacement.Replace(match, args);
		}
	}
}

