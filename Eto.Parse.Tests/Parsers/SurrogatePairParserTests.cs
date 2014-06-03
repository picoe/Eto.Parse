using System;
using System.Linq;
using Eto.Parse.Parsers;
using NUnit.Framework;

namespace Eto.Parse.Tests.Parsers
{
    [TestFixture]
    public class SurrogatePairParserTests
    {
        [Test]
        public void TestAnySurrogatePair()
        {
            var chars = string.Format("{0},{1},{2}",
                char.ConvertFromUtf32(0x10000),
                char.ConvertFromUtf32(0x87FFF),
                char.ConvertFromUtf32(0x10FFFF));

            var grammar = new Grammar();
            var parser = new AnySurrogatePairTerminal();
            grammar.Inner = (+parser.Named("char")).SeparatedBy(",");

            var match = grammar.Match(chars);

            Assert.IsTrue(match.Success, match.ErrorMessage);
            CollectionAssert.AreEquivalent(new []{0x10000, 0x87FFF, 0x10FFFF}, match.Find("char").Select(m => char.ConvertToUtf32((string) parser.GetValue(m),0)));
        }

        [Test]
        public void TestMatchingSpecificSurrogatePairByCodePoint()
        {
            var sample = char.ConvertFromUtf32(0x87FFF);

            var grammar = new Grammar();
            var parser = new SingleSurrogatePairTerminal(0x87FFF);
            grammar.Inner = parser.Named("char");

            var match = grammar.Match(sample);

            Assert.IsTrue(match.Success, match.ErrorMessage);
            Assert.AreEqual(0x87FFF, char.ConvertToUtf32((string)parser.GetValue(match.Find("char").Single()), 0));
        }

        [Test]
        public void TestUnmatchedSpecificSurrogatePairByCodePoint()
        {
            var sample = char.ConvertFromUtf32(0x17DF6);

            var grammar = new Grammar();
            var parser = new SingleSurrogatePairTerminal(0x87FFF);
            grammar.Inner = parser.Named("char");

            var match = grammar.Match(sample);

            Assert.IsFalse(match.Success, match.ErrorMessage);
        }

        [TestCase(0x12345, TestName = "Lower bound")]
        [TestCase(0x57FFF, TestName = "Within range")]
        [TestCase(0x8F4FE, TestName = "Upper bound")]
        public void TestMatchingRange(int codePoint)
        {
            var sample = char.ConvertFromUtf32(codePoint);

            var grammar = new Grammar();
            var parser = new SurrogatePairRangeTerminal(0x12345, 0x8F4FE);
            grammar.Inner = parser.Named("char");

            var match = grammar.Match(sample);

            Assert.IsTrue(match.Success, match.ErrorMessage);
        }

        [TestCase(0x12345, TestName = "Outside lower bound")]
        [TestCase(0x8F4FE, TestName = "Outside upper bound")]
        public void TestMatchOutsideRange(int codePoint)
        {
            var sample = char.ConvertFromUtf32(codePoint);

            var grammar = new Grammar();
            var parser = new SurrogatePairRangeTerminal(0x12346, 0x8F4FD);
            grammar.Inner = parser.Named("char");

            var match = grammar.Match(sample);

            Assert.IsFalse(match.Success, "Value {0} should be outside given range", codePoint);
        }

        [TestCase(50, TestName = "Ordinary char")]
        [TestCase(0x10FFFF + 1, TestName = "Value too high")]
        [TestCase(-1, TestName = "Negative value")]
        public void TestInvaldCodePoint(int codePoint)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SingleSurrogatePairTerminal(codePoint);
                });

            Assert.That("codePoint", Is.EqualTo(exception.ParamName));
        }

        [Test]
        public void TestUseWithOtherParser()
        {
            var sample = "abc" + char.ConvertFromUtf32(0x8F4FE) + "def" + char.ConvertFromUtf32(0x56734);

            var grammar = new Grammar();
            var parser = new LetterTerminal() | new AnySurrogatePairTerminal();
            grammar.Inner = (+parser).Named("char");

            var match = grammar.Match(sample);

            Assert.IsTrue(match.Success, match.ErrorMessage);
            Assert.That(match.Length, Is.EqualTo(10));
        }
    }
}