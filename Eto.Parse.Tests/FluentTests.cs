using System;
using NUnit.Framework;
using Eto.Parse;

namespace Eto.Parse.Tests
{
	[TestFixture]
	public class FluentTests
	{
		[Test]
		public void Simple()
		{
			var input = "  hello ( parsing world )  ";

			// optional repeating whitespace
			var ws = Terminals.WhiteSpace.Repeat(0);

			// parse a value with or without brackets
			var valueParser = Terminals.Set('(')
				.Then(ws)
				.Then(Terminals.Set(')').Inverse().Repeat().Until(ws.Then(')')).NonTerminal("value"))
				.Then(ws)
				.Then(Terminals.Set(')'))
				.Or(Terminals.WhiteSpace.Inverse().Repeat().NonTerminal("value"));

			// top level
			var parser =
				ws
				.Then(valueParser.NonTerminal("first"))
				.Then(ws)
				.Then(valueParser.NonTerminal("second"))
				.Then(ws)
				.Then(Terminals.End)
				.NonTerminal("top");

			var match = parser.Match(input);
			Assert.IsTrue(match.Success);
			Assert.AreEqual("hello", match["first"]["value"].Value);
			Assert.AreEqual("parsing world", match["second"]["value"].Value);
		}

		[Test]
		public void RepeatUntil()
		{
			var input = "abc def 1234";

			// optional repeating whitespace
			var ws = Terminals.WhiteSpace.Repeat(0);

			// repeat until we get a digit, and exclude any whitespace inbetween
			var repeat = Terminals.AnyChar.Repeat().Until(ws.Then(Terminals.Digit)).NonTerminal("top");

			var match = repeat.Match(input);
			Assert.IsTrue(match.Success);
			Assert.AreEqual("abc def", match.Value);
		}

	}
}

