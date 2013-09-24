using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class ParagraphSection : SequenceParser, IMarkdownReplacement
	{
		public ParagraphSection()
		{
			Name = "para";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			var prefix = Terms.sp.Repeat(0, 3);
			var content = (~prefix & Terms.words & -Terms.sp);
			var dlheader = new DoubleLineHeaderSection();
			dlheader.Initialize(grammar);
			var finish = Terminals.Set("#>") | dlheader;
			var lines = content.Repeat(1).SeparatedBy(Terms.eol).Until((Terms.eol & finish.NonCaptured()) | Terms.EndOfSection(minLines: 2), true);
			Add(grammar.Prefix, prefix, lines.Named("lines"));
		}

		#if PERF_TEST
		protected override int InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<p>");
			args.Encoding.Transform(args, match.Matches[0]);
			args.Output.AppendUnixLine("</p>");
			args.Output.AppendUnixLine();
		}
	}
}