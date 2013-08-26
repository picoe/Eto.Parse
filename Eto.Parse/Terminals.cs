using System;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse
{
	public static class Terminals
	{
		public static Parser AnyChar { get { return new AnyCharTerminal(); } }
		public static CharTerminal Digit { get { return new DigitTerminal(); } }
		public static CharTerminal HexDigit { get { return new HexDigitTerminal(); } }
		public static CharTerminal Letter { get { return new LetterTerminal(); } }
		public static CharTerminal LetterOrDigit { get { return new LetterOrDigitTerminal(); } }
		public static CharTerminal WhiteSpace { get { return new WhiteSpaceTerminal(); } }
		public static CharTerminal SingleLineWhiteSpace { get { return new SingleLineWhiteSpaceTerminal(); } }
		public static CharTerminal Punctuation { get { return new PunctuationTerminal(); } }
		public static CharTerminal ControlCodes { get { return new ControlTerminal(); } }
		public static CharTerminal Symbol { get { return new SymbolTerminal(); } }
		public static Parser Eol { get { return new EolTerminal(); } }
		public static CharTerminal Set(params int[] chars) { return new CharSetTerminal(chars.Select(r => (char)r).ToArray()) { Reusable = true }; }
		public static CharTerminal Set(params char[] chars) { return new CharSetTerminal(chars); }
		public static CharTerminal Set(string chars) { return new CharSetTerminal(chars.ToCharArray()); }
		public static CharTerminal Range(char start, char end) { return new CharRangeTerminal(start, end); }
		public static CharTerminal Range(int start, int end) { return new CharRangeTerminal((char)start, (char)end); }
		public static CharTerminal Printable { get { return new ControlTerminal() { Inverse = true }; } }

		public static StartParser Start { get { return new StartParser(); } }
		public static EndParser End { get { return new EndParser(); } }

		public static LiteralTerminal Literal(string matchValue) { return new LiteralTerminal(matchValue); }
	}
}

