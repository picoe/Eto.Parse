using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class ReferenceSection : MarkdownReplacement
	{
		public override string Name { get { return "ref"; } }

		public ReferenceSection()
		{
			this.AddLinesBefore = false;
		}

		public override Parser GetParser(MarkdownGrammar grammar)
		{
			var prefix = "[" & (+Terminals.Set(']').Inverse().Except(Terms.eol)).WithName("id") & "]:";
			var link = (+Terminals.WhiteSpace.Inverse()).WithName("link");
			var title = new StringParser { Name = "title", BeginQuoteCharacters = "\"'(".ToCharArray(), EndQuoteCharacters = "\"')".ToCharArray() };

			return (Terms.sp.Repeat(0, 3) & prefix & Terms.ows & link & ~(Terms.ows & title)).WithName(Name) & (+Terms.blankLine | Terms.eof);
		}

		public override void Replace(Match match, MarkdownReplacementArgs args)
		{
			// blank!
		}

		public void AddReference(Match match, MarkdownReplacementArgs args)
		{
			var reference = new MarkdownReference { Url = match.Matches[1].Text };
			if (match.Matches.Count > 2)
				reference.Title = match.Matches[2].StringValue;
			args.References[match.Matches[0].Text] = reference;
		}
	}
}