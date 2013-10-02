using System;
using Eto.Parse.Parsers;
using System.Linq;
using System.Collections.Generic;

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

		public static CharTerminal Set(params int[] chars)
		{
			if (chars.Length == 1)
				return new SingleCharTerminal((char)chars[0]);
			else
				return new CharSetTerminal(chars.Select(r => (char)r).ToArray()) { Reusable = true };
		}

		public static CharTerminal Set(params char[] chars)
		{ 
			if (chars.Length == 1)
				return new SingleCharTerminal(chars[0]);
			else
				return new CharSetTerminal(chars);
		}

		public static CharTerminal Set(string chars)
		{
			return Set(chars.ToCharArray());
		}

		public static CharTerminal Range(char start, char end)
		{
			return new CharRangeTerminal(start, end);
		}

		public static CharTerminal Range(int start, int end)
		{
			return new CharRangeTerminal((char)start, (char)end);
		}

		public static CharTerminal Printable { get { return new ControlTerminal { Inverse = true }; } }

		public static StartParser Start { get { return new StartParser(); } }

		public static EndParser End { get { return new EndParser(); } }

		public static LiteralTerminal Literal(string matchValue)
		{
			return new LiteralTerminal(matchValue);
		}

		public static RepeatCharTerminal Repeat(Func<char, bool> test, int minimum, int maximum = int.MaxValue)
		{
			return new RepeatCharTerminal(new RepeatCharItem(test, minimum, maximum));
		}

		public static RepeatCharTerminal Repeat(params RepeatCharItem[] items)
		{
			return new RepeatCharTerminal(items);
		}

		public static RepeatCharTerminal Repeat(IEnumerable<RepeatCharItem> items)
		{
			return new RepeatCharTerminal(items);
		}
	}
}

