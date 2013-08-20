using System;
using NUnit.Framework;
using Eto.Parse;

namespace Eto.Parse.Tests.Grammars
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
				.Then(Terminals.Set(')').Inverse().Repeat().Until(ws.Then(')')).Named("value"))
				.Then(Terminals.Set(')'))
				.SeparatedBy(ws)
				.Or(Terminals.WhiteSpace.Inverse().Repeat().Named("value"));

			// our grammar
			var grammar = new Grammar(
				ws
				.Then(valueParser.Named("first"))
				.Then(valueParser.Named("second"))
				.Then(Terminals.End)
				.SeparatedBy(ws)
			);

			var match = grammar.Match(input);
			Assert.IsTrue(match.Success);
			Assert.AreEqual("hello", match["first"]["value"].Text);
			Assert.AreEqual("parsing world", match["second"]["value"].Text);
		}

		[Test]
		public void RepeatUntil()
		{
			var input = "abc def 1234";

			// optional repeating whitespace
			var ws = Terminals.WhiteSpace.Repeat(0);

			// repeat until we get a digit, and exclude any whitespace inbetween
			var repeat = new Grammar(Terminals.AnyChar.Repeat().Until(ws.Then(Terminals.Digit))) { AllowPartialMatch = true };

			var match = repeat.Match(input);
			Assert.IsTrue(match.Success);
			Assert.AreEqual("abc def", match.Text);
		}

		[Test]
		public void Except()
		{
			var ch = Terminals.AnyChar.Except(Terminals.WhiteSpace).Repeat();

			var input = "abc def 1234";

			var repeat = new Grammar(ch) { AllowPartialMatch = true };

			var match = repeat.Match(input);
			Assert.IsTrue(match.Success);
			Assert.AreEqual("abc", match.Text);
		}

	}
}

