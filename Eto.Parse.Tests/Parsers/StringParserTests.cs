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
			var sample = "\"string\\'\\\"\\a\\b\\f\\n\\r\\t\\v\\x123\\u1234\\U00001234\\0 1\",'string\\'\\\"\\a\\b\\f\\n\\r\\t\\v\\x123\\u1234\\U00001234\\0 2'";

			var grammar = new Grammar();
			var str = new StringParser { AllowEscapeCharacters = true  };

			grammar.Inner = (+str.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.IsTrue(match.Success, match.ErrorMessage);
			var values = match.Find("str").Select(m => str.GetValue(m)).ToArray();
			CollectionAssert.AreEqual(new string[] { "string\'\"\a\b\f\n\r\t\v\x123\u1234\U00001234\0 1", "string\'\"\a\b\f\n\r\t\v\x123\u1234\U00001234\0 2" }, values);
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

		[Test]
		public void TestErrorConditionsNoOptions()
		{
			var samples = new string[] { "string1", "\"string 2", "'string 3", "'string ''4", "string5'", "string6\"", "\"string\\\"7\"", "string 8" };

			var grammar = new Grammar();
			var str = new StringParser { AllowDoubleQuote = false, AllowNonQuoted = false, AllowEscapeCharacters = false };

			grammar.Inner = str.Named("str");

			foreach (var sample in samples)
			{
				var match = grammar.Match(sample);
				Assert.IsFalse(match.Success, "Should not match string {0}", sample);
			}
		}

		[Test]
		public void TestErrorConditionsWithOptions()
		{
			var samples = new string[] { "string 1", "\"string 2", "'string 3", "'string ''4", "string5'", "string6\"", "\"string\\\"7" };

			var grammar = new Grammar();
			var str = new StringParser { AllowDoubleQuote = true, AllowNonQuoted = true, AllowEscapeCharacters = true };

			grammar.Inner = str.Named("str");

			foreach (var sample in samples)
			{
				var match = grammar.Match(sample);
				Assert.IsFalse(match.Success, "Should not match string {0}", sample);
			}
		}
	}
}

