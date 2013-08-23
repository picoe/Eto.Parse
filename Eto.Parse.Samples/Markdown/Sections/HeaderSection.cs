using System;

namespace Eto.Parse.Samples.Markdown.Sections
{
	public class HeaderSection : MarkdownReplacement
	{
		public override Parser GetParser(MarkdownGrammar grammar)
		{
			var prefix = +Terminals.Literal("#");
			var value = (+Terms.word).SeparatedBy(Terms.ws).Until(Terms.ows & -Terminals.Literal("#") & Terms.eolorf, true);
			var header = prefix & Terms.ows & value & -Terms.blankLine;
			prefix.Name = "prefix";
			value.Name = "value";
			header.Name = "header";
			return header;
		}

		public override void Replace(Match match, MarkdownReplacementArgs args)
		{
			var level = match.Matches[0].Length;
			args.Output.Append("<h");
			args.Output.Append(level);
			args.Output.Append(">");
			args.Encoding.Replace(match.Matches[1].StringValue, args);
			args.Output.Append("</h");
			args.Output.Append(level);
			args.Output.Append(">");
		}

		public override string Name { get { return "header"; } }
	}
}

