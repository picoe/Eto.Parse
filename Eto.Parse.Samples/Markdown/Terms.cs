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
		public static Parser eol;
		public static Parser eof = Terminals.End;
		public static Parser eolorf;
		public static Parser ch = !Terminals.Set(0xA, 0xD, 0x9, 0x20);
		public static Parser blankLine;

		public static Parser word;
		public static Parser words;
		public static Parser ws;
		public static Parser ows;
		public static Parser sporht;

		public static Parser EndOfSection(Parser suffix = null)
		{
			if (suffix != null)
				return (+blankLine & suffix) | eol & eof | eof;
			else
				return blankLine.Repeat(2) | eol & eof | eof;
		}

		static Terms()
		{
			sporht = Terminals.Set(" \t");
			eol = Terminals.Literal("\r\n") | cr | lf;
			word = +ch;
			ws = +(sporht);
			ows = -(sporht);
			blankLine = ows & eol;
			eolorf = eol | eof;
			words = (+word).SeparatedBy(ws);
		}
	}
	
}
