using System;
using Eto.Parse.Parsers;
using System.Text;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Eto.Parse.Samples.Markdown.Sections;

namespace Eto.Parse.Samples.Markdown
{
	static class Terms
	{
		public static Parser sp = Terminals.Literal(" ");
		public static Parser ht = Terminals.Literal("\t");
		public static Parser cr = Terminals.Literal("\r");
		public static Parser lf = Terminals.Literal("\n");
		public static Parser eol = Terminals.Eol;
		public static Parser eof = Terminals.End;
		public static Parser ch = !Terminals.Set(" \r\n\t");
		public static Parser sporht = Terminals.Set(" \t");
		public static Parser indent = Terms.sp.Repeat(4, 4) | ht;
		public static Parser eolorf;
		public static Parser blankLine;

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
			word = +ch;
			ws = +(sporht);
			ows = -(sporht);
			blankLine = (ows & eol).Separate();
			eolorf = (Terminals.Eol | eof).Separate();
			words = (+word).SeparatedBy(ws);
		}
	}
	
}
