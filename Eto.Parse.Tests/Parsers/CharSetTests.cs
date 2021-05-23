using Eto.Parse.Parsers;
using NUnit.Framework;

namespace Eto.Parse.Tests.Parsers
{
	[TestFixture]
	public class CharSetTests
	{
		[TestCase(true, false)]
		[TestCase(false, false)]
		[TestCase(null, false)]
		[TestCase(true, true)]
		[TestCase(false, true)]
		public void CaseSensitiveShouldBeCorrect(bool? caseSensitive, bool inherit)
		{
			var parser = new CharSetTerminal();
			parser.Characters = "abcdef".ToCharArray();
			var grammar = new Grammar { Inner = parser };
			
			if (inherit)
				grammar.CaseSensitive = caseSensitive.Value;
			else
				parser.CaseSensitive = caseSensitive;

			Assert.IsTrue(grammar.Match("a").Success, "1.1");
			Assert.IsTrue(grammar.Match("b").Success, "1.2");
			Assert.IsTrue(grammar.Match("c").Success, "1.3");
			
			if (caseSensitive != false)
			{
				Assert.IsFalse(grammar.Match("A").Success, "2.1");
				Assert.IsFalse(grammar.Match("B").Success, "2.2");
				Assert.IsFalse(grammar.Match("C").Success, "2.3");
			}
			else
			{
				Assert.IsTrue(grammar.Match("A").Success, "3.1");
				Assert.IsTrue(grammar.Match("B").Success, "3.2");
				Assert.IsTrue(grammar.Match("C").Success, "3.3");
			}
			
			// out of range
			Assert.IsFalse(grammar.Match("g").Success, "4.1");
			Assert.IsFalse(grammar.Match("G").Success, "4.2");
		}
	}
}