using System;
using NUnit.Framework;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse.Tests.Parsers
{
	[TestFixture]
	public class StringParserTests
	{
		[Test]
		public void TestQuoting()
		{
			var sample = "string1,\"string 2\",'string 3'";

			var grammar = new Grammar();
			var str = new StringParser { AllowNonQuoted = true };

			grammar.Inner = (+str.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.IsTrue(match.Success, match.ErrorMessage);
			CollectionAssert.AreEquivalent(new string[] { "string1", "string 2", "string 3" }, match.Find("str").Select(m => str.GetValue(m)));
		}

		[Test]
		public void TestEscaping()
		{
			var sample = "\"string\\'\\\"\\0\\a\\b\\f\\n\\r\\t\\v\\x123\\u1234\\U00001234 1\",'string\\'\\\"\\0\\a\\b\\f\\n\\r\\t\\v\\x123\\u1234\\U00001234 2'";

			var grammar = new Grammar();
			var str = new StringParser { AllowEscapeCharacters = true  };

			grammar.Inner = (+str.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.IsTrue(match.Success, match.ErrorMessage);
			var values = match.Find("str").Select(m => str.GetValue(m)).ToArray();
			CollectionAssert.AreEquivalent(new string[] { "string\'\"\0\a\b\f\n\r\t\v\x123\u1234\U00001234 1", "string\'\"\0\a\b\f\n\r\t\v\x123\u1234\U00001234 2" }, values);
		}

		[Test]
		public void TestDoubleQuoting()
		{
			var sample = "\"string\"\" ''1'\",'string'' \"\"2\"'";

			var grammar = new Grammar();
			var str = new StringParser { AllowDoubleQuote = true };

			grammar.Inner = (+str.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.IsTrue(match.Success, match.ErrorMessage);
			CollectionAssert.AreEquivalent(new string[] { "string\" ''1'", "string' \"\"2\"" }, match.Find("str").Select(m => str.GetValue(m)));
		}
	}
}

