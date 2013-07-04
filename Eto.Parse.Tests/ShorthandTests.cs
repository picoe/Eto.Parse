using System;
using NUnit.Framework;
using Eto.Parse;

namespace Eto.Parse.Tests
{
	[TestFixture]
	public class ShorthandTests
	{
		[Test]
		public void Simple()
		{
			var input = "  hello ( parsing world )  ";

			// optional repeating whitespace
			var ws = -Terminals.WhiteSpace;

			// parse a value with or without brackets
			Parser valueParser = ('(' & ws & (+(!(Parser)')') - (ws & ')')).NonTerminal("value") & ws & ')')
				| (+!Terminals.WhiteSpace).NonTerminal("value");

			// top level
			var parser = ws & valueParser.NonTerminal("first") & ws & valueParser.NonTerminal("second") & ws & Terminals.End;
			var match = parser.NonTerminal("top").Match(input);
			Assert.IsTrue(match.Success);
			Assert.AreEqual("hello", match["first"]["value"].Value);
			Assert.AreEqual("parsing world", match["second"]["value"].Value);
		}

		[Test]
		public void RepeatUntil()
		{
			var input = "abc def 1234";

			// optional repeating whitespace
			var ws = -Terminals.WhiteSpace;

			// repeat until we get a digit, and exclude any whitespace inbetween
			var repeat = +Terminals.AnyChar - (ws & Terminals.Digit);

			var match = repeat.NonTerminal("top").Match(input);
			Assert.IsTrue(match.Success);
			Assert.AreEqual("abc def", match.Value);
		}
	}
}

