using System;
using Eto.Parse.Parsers;
using Eto.Parse.Testers;
using System.Linq;

namespace Eto.Parse
{
	public static class Terminals
	{
		public static CharParser AnyChar { get { return new CharParser(new AnyCharTester()); } }
		public static CharParser Digit { get { return new CharParser(new DigitTester()); } }
		public static CharParser HexDigit { get { return new CharParser(new HexDigitTester()); } }
		public static CharParser Letter { get { return new CharParser(new LetterTester()); } }
		public static CharParser LetterOrDigit { get { return new CharParser(new LetterOrDigitTester()); } }
		public static CharParser WhiteSpace { get { return new CharParser(new WhiteSpaceTester()); } }
		public static CharParser SingleLineWhiteSpace { get { return new CharParser(new SingleLineWhiteSpaceTester()); } }
		public static CharParser Punctuation { get { return new CharParser(new PunctuationTester()); } }
		public static CharParser ControlCodes { get { return new CharParser(new ControlTester()); } }
		public static CharParser Symbol { get { return new CharParser(new SymbolTester()); } }
		public static CharParser Eol { get { return new CharParser(new EolTester()); } }
		public static CharParser Set(params int[] chars) { return new CharParser(new CharSetTester(chars.Select(r => (char)r).ToArray())); }
		public static CharParser Set(params char[] chars) { return new CharParser(new CharSetTester(chars)); }
		public static CharParser Set(string chars) { return new CharParser(new CharSetTester(chars.ToCharArray())); }
		public static CharParser Range(char start, char end) { return new CharParser(new RangeTester(start, end)); }
		public static CharParser Range(int start, int end) { return new CharParser(new RangeTester((char)start, (char)end)); }
		public static CharParser Printable { get { return new CharParser(new ControlTester()) { Inverse = true }; } }

		public static StartParser Start { get { return new StartParser(); } }
		public static EndParser End { get { return new EndParser(); } }

		public static StringParser String(string matchValue) { return new StringParser(matchValue); }
	}
}

