using System;
using NUnit.Framework;
using Eto.Parse;

namespace Eto.Parse.Tests.Grammars
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
			Parser valueParser = 
				('(' & ws & (+(Terminals.AnyChar) - (ws & ')')).Named("value") & ws & ')')
				| (+!Terminals.WhiteSpace).Named("value");

			// our grammar
			var grammar = new Grammar(ws & valueParser.Named("first") & ws & valueParser.Named("second") & ws & Terminals.End);

			var match = grammar.Match(input);
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

			var match = new Grammar(repeat) { AllowPartialMatch = true }.Match(input);
			Assert.IsTrue(match.Success);
			Assert.AreEqual("abc def", match.Value);
		}
	}
}

