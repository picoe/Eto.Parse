using Eto.Parse.Parsers;
using NUnit.Framework;

namespace Eto.Parse.Tests.Parsers
{
	[TestFixture]
    public class CharRangeTests
    {
		[TestCase(true, false)]
		[TestCase(false, false)]
		[TestCase(null, false)]
		[TestCase(true, true)]
		[TestCase(false, true)]
		public void CaseSensitiveShouldBeCorrect(bool? caseSensitive, bool inherit)
		{
			var parser = new CharRangeTerminal();
			parser.Start = 'a';
			parser.End = 'f';
			var grammar = new Grammar { Inner = parser };
			
			if (inherit)
				grammar.CaseSensitive = caseSensitive.Value;
			else
				parser.CaseSensitive = caseSensitive;

			Assert.IsTrue(grammar.Match("a").Success, "1.1");
			Assert.IsTrue(grammar.Match("b").Success, "1.2");
			Assert.IsTrue(grammar.Match("f").Success, "1.3");
			
			if (caseSensitive != false)
			{
				Assert.IsFalse(grammar.Match("A").Success, "2.1");
				Assert.IsFalse(grammar.Match("B").Success, "2.2");
				Assert.IsFalse(grammar.Match("F").Success, "2.3");
			}
			else
			{
				Assert.IsTrue(grammar.Match("A").Success, "3.1");
				Assert.IsTrue(grammar.Match("B").Success, "3.2");
				Assert.IsTrue(grammar.Match("F").Success, "3.3");
			}

			// out of range
			Assert.IsFalse(grammar.Match("g").Success, "4.1");
			Assert.IsFalse(grammar.Match("G").Success, "4.2");
		}
    }
}