using System;
using NUnit.Framework;
using Eto.Parse;
using System.Diagnostics;
using Eto.Parse.Grammars;
using System.Linq;

namespace Eto.Parse.Tests
{
	[TestFixture]
	public class BnfTests
	{
		const int speedIterations = 1000;

		public const string Address = @"Joe Smith
123 Elm Street
Vancouver, BC V5V5V5";

		public const string AddressMissingZipPart = @"Joe Smith
123 Elm Street";

		const string postalAddressBnf = @"
 <postal-address> 
                  ::= <name-part> <EOL> <street-address> [<EOL>] <zip-part>
 
      <name-part> ::= (<personal-part> <last-name> [<suffix-part>])
                      | (<personal-part> <name-part>)
 
  <personal-part> ::= <first-name> | <initial> '.'
 
 <street-address> ::= <house-num> <street> [<apt-num>]
 
       <zip-part> ::= <town-name> ',' <state-code> <zip-code> 
 
<suffix-part> ::= ['Sr.' | 'Jr.' | <roman-numeral>]

<street> ::= <street-name> [<street-type>]

<street-type> ::= 'Street' | 'Drive' | 'Ave' | 'Avenue'

<apt-num> ::= ('Apt'|'Suite') ['#'] {<digit>}
";

		public static Grammar GetAddressParser()
		{
			var bnfParser = new BnfGrammar();
			return bnfParser.Build(postalAddressBnf, "postal-address");
		}

		[Test]
		public void BnfParser()
		{
			TestAddress(GetAddressParser());
		}

		[Test]
		public void BnfParsingSpeed()
		{
			var bnfParser = new BnfGrammar();
			Helper.TestSpeed(bnfParser, postalAddressBnf, speedIterations);
		}

		[Test]
		public void AddressParsingSpeed()
		{
			var addressParser = GetAddressParser();
			Helper.TestSpeed(addressParser, Address, speedIterations);
		}

		[Test]
		public void BnfToCode()
		{
			// roundtrip to generated code then back again
			var bnfParser = new BnfGrammar();

			// generate code from bnf
			var code = bnfParser.ToCode(postalAddressBnf, "postal-address", "PostalGrammar");

			// execute the code and test
			var addressParser = Helper.Create<Grammar>(code, "PostalGrammar", "Eto.Parse");
			TestAddress(addressParser);
		}

		[Test]
		public void FailedMatch()
		{
			var addressParser = GetAddressParser();
			var match = addressParser.Match(AddressMissingZipPart);
			Assert.IsFalse(match.Success);
			Assert.That(!match.Success, "Error was not specified");
			Assert.That(match.ErrorIndex == AddressMissingZipPart.Length, "Error should be where the zip code is specified");
		}

		public static void TestAddress(NamedMatch match)
		{
			Assert.IsTrue(match.Success);
			Assert.AreEqual("Joe", match["first-name", true].Value);
			Assert.AreEqual("Smith", match["last-name", true].Value);
			Assert.AreEqual("123", match["house-num", true].Value);
			Assert.AreEqual("Elm Street", match["street", true].Value);
			Assert.AreEqual("Elm", match["street-name", true].Value);
			Assert.AreEqual("Street", match["street-type", true].Value);
			Assert.AreEqual("Vancouver", match["town-name", true].Value);
			Assert.AreEqual("BC", match["state-code", true].Value);
			Assert.AreEqual("V5V5V5", match["zip-code", true].Value);
		}

		public static void TestAddress(Grammar addressParser)
		{
			TestAddress(addressParser.Match(Address));
		}
	}
}

