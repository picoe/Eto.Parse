using Eto.Parse.Grammars;
using NUnit.Framework;

namespace Eto.Parse.Tests.Samples
{
	[TestFixture]
	public class LargeCharcterSetRange
	{
		[Test]
		public void LargeRangeShouldntCauseMemoryException()
		{
			var _grammar = new EbnfGrammar(EbnfStyle.W3c).Build($"id ::= [a-zA-Z\u0100-\uffff_][0-9a-zA-Z\u0100-\uffff_]*", "id");
			var _match = _grammar.Match("张三李四");
		}

	}
}