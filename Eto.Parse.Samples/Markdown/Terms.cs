using System;

namespace Eto.Parse.Samples.Markdown
{
	static class Terms
	{
		public static Parser sp = Terminals.Literal(" ");
		public static Parser ht = Terminals.Literal("\t");
		public static Parser eol = Terminals.Eol;
		public static Parser eof = Terminals.End;
		public static Parser sporht = Terminals.Set(" \t");
		public static Parser indent = "    " | ht;
		public static Parser eolorf;
		public static Parser blankLine;
		public static Parser blankLineOrEof;

		public static Parser word;
		public static Parser words;
		public static Parser ws;
		public static Parser ows;

		public static Parser EndOfSection(Parser suffix = null, int? minLines = null)
		{
			if (suffix != null)
				return (blankLine.Repeat(minLines ?? 1) & suffix) | eol & eof | eof;
			else
				return blankLine.Repeat(minLines ?? 2) | eol & eof | eof;
		}

		static Terms()
		{
			word = Terminals.Repeat(c => c != ' ' && c != '\r' && c != '\n' && c != '\t', 1);
			ws = Terminals.Repeat(c => c == ' ' || c == '\t', 1);
			ows = Terminals.Repeat(c => c == ' ' || c == '\t', 0);
			blankLine = (ows & eol).Separate();
			blankLineOrEof = (blankLine | eof).Separate();
			eolorf = (Terminals.Eol | eof).Separate();
			words = (+word).SeparatedBy(ws);
		}
	}
	
}
