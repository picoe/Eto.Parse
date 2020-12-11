using NUnit.Framework;
using Eto.Parse.Parsers;

namespace Eto.Parse.Tests.Parsers
{
	[TestFixture]
	public class CharSetTerminalTest
	{
		[Test]
		public void TestBasic()
		{
			var sample = "a";

			var parser = new CharSetTerminal('a','b','c');
			var grammar = new Grammar(parser);
			var match = grammar.Match(sample);

			Assert.IsTrue(match.Success,match.ErrorMessage);
		}

		[Test]
		public void TestCaseInvariance()
		{
			var sample = "A";

			var parser = new CharSetTerminal('a','b','c');
			var grammar = new Grammar(parser)
			{
				CaseSensitive = false
			};
			var match = grammar.Match(sample);

			Assert.IsTrue(match.Success,match.ErrorMessage);
		}
	}
}