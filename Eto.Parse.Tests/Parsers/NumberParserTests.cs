using System;
using NUnit.Framework;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse.Tests.Parsers
{
	[TestFixture]
	public class NumberParserTests
	{
		[Test]
		public void TestDecimal()
		{
			var sample = "123.4567,1234567";

			var grammar = new Grammar();
			var num = new NumberParser { AllowDecimal = true, AllowSign = false };

			grammar.Inner = (+num.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.IsTrue(match.Success, match.ErrorMessage);
			CollectionAssert.AreEquivalent(new Decimal[] { 123.4567M, 1234567M }, match.Find("str").Select(m => num.GetValue(m)));
		}

		[Test]
		public void TestSign()
		{
			var sample = "123.4567,+123.4567,-123.4567";

			var grammar = new Grammar();
			var num = new NumberParser { AllowSign = true, AllowDecimal = true };

			grammar.Inner = (+num.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.IsTrue(match.Success, match.ErrorMessage);
			CollectionAssert.AreEquivalent(new Decimal[] { 123.4567M, 123.4567M, -123.4567M }, match.Find("str").Select(m => num.GetValue(m)));
		}

		[Test]
		public void TestExponent()
		{
			var sample = "123E-02,123E+10,123.4567E+5,1234E2";

			var grammar = new Grammar();
			var num = new NumberParser { AllowDecimal = true, AllowExponent = true };

			grammar.Inner = (+num.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.IsTrue(match.Success, match.ErrorMessage);
			CollectionAssert.AreEquivalent(new Decimal[] { 123E-2M, 123E+10M, 123.4567E+5M, 1234E+2M }, match.Find("str").Select(m => num.GetValue(m)));
		}

		[Test]
		public void TestDecimalValues()
		{
			var sample = "123.4567,+123.4567,-123.4567";

			var grammar = new Grammar();
			var num = new NumberParser { AllowSign = true, AllowDecimal = true };

			grammar.Inner = (+num.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.IsTrue(match.Success, match.ErrorMessage);
			CollectionAssert.AreEquivalent(new Decimal[] { 123.4567M, 123.4567M, -123.4567M }, match.Find("str").Select(m => m.DecimalValue));
		}

		[Test]
		public void TestInt32Values()
		{
			var sample = "123.4567,+123.4567,-123.4567";

			var grammar = new Grammar();
			var num = new NumberParser { AllowSign = true, AllowDecimal = true };

			grammar.Inner = (+num.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.IsTrue(match.Success, match.ErrorMessage);
			CollectionAssert.AreEquivalent(new Int32[] { 123, 123, -123 }, match.Find("str").Select(m => m.Int32Value));
		}
	}
}

