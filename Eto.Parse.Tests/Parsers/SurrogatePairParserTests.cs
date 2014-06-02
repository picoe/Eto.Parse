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
            var parser = new SurrogatePairParser();
            grammar.Inner = (+parser.Named("char")).SeparatedBy(",");

            var match = grammar.Match(chars);

            Assert.IsTrue(match.Success, match.ErrorMessage);
            CollectionAssert.AreEquivalent(new []{0x10000, 0x87FFF, 0x10FFFF}, match.Find("char").Select(m => char.ConvertToUtf32((string) parser.GetValue(m),0)));
        }
    }
}