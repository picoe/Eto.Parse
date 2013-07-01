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
			var input = "  hello (parsing world)  ";

			// repeating whitespace
			var ws = +Terminals.WhiteSpace;

			// parse a value with or without brackets
			Parser valueParser = ('(' & (+!(Parser)')').Named("value") & ')')
				| (+!Terminals.WhiteSpace).Named("value");

			// top level
			var parser = ws & valueParser.Named("first") & ws & valueParser.Named("second") & ws & Terminals.End;
			var match = parser.Match(input);
			Assert.IsTrue(match.Success);
			Assert.AreEqual("hello", match["first"]["value"].Value);
			Assert.AreEqual("parsing world", match["second"]["value"].Value);
		}
	}
}

