using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class ReferenceSection : SequenceParser, IMarkdownReplacement
	{
		public ReferenceSection()
		{
			Name = "ref";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var prefix = "[" & (+Terminals.Set("]\n\r").Inverse()).WithName("id") & "]:";
			var link = (+Terminals.WhiteSpace.Inverse()).WithName("link");
			var title = new StringParser { Name = "title", BeginQuoteCharacters = "\"'(".ToCharArray(), EndQuoteCharacters = "\"')".ToCharArray() };

			Add(Terms.sp.Repeat(0, 3), prefix, Terms.ows, link, Terms.ows, ~title, +Terms.blankLine | Terms.eof);
		}

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			// blank!
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void AddReference(Match match, MarkdownReplacementArgs args)
		{
			var reference = new MarkdownReference { Url = match.Matches[1].Text };
			if (match.Matches.Count > 2)
				reference.Title = match.Matches[2].StringValue;
			args.References[match.Matches[0].Text] = reference;
		}
	}
}