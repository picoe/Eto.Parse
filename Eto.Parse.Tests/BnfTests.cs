using System;
using NUnit.Framework;
using Eto.Parse;
using System.Diagnostics;

namespace Eto.Parse.Tests
{
	[TestFixture]
	public class BnfTests
	{
		const int speedIterations = 500;

		const string address = @"Joe Smith
123 Elm Street
Vancouver, BC V5V5V5";

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

		[Test]
		public void BnfParser()
		{
			var bnfParser = new BnfParser();
			var addressParser = bnfParser.Build(postalAddressBnf, "postal-address");
			TestAddress(addressParser);
		}

		[Test]
		public void BnfParsingSpeed()
		{
			var bnfParser = new BnfParser();
			Helper.TestSpeed(bnfParser, postalAddressBnf, speedIterations);
		}

		[Test]
		public void AddressParsingSpeed()
		{
			var bnfParser = new BnfParser();
			var addressParser = bnfParser.Build(postalAddressBnf, "postal-address");
			Helper.TestSpeed(addressParser, address, speedIterations);
		}

		[Test]
		public void BnfToCode()
		{
			// roundtrip to generated code then back again
			var bnfParser = new BnfParser();

			// generate code from bnf
			var code = bnfParser.ToCode(postalAddressBnf, "postal-address");

			// execute the code and test
			var addressParser = Helper.Execute<Parser>(code, "GeneratedParser", "GetParser", "Eto.Parse");
			TestAddress(addressParser);
		}

		void TestAddress(Parser addressParser)
		{
			var match = addressParser.Match(address);
			Assert.IsTrue(match.Success);
			Assert.AreEqual("Joe", match["first-name"].Value);
			Assert.AreEqual("Smith", match["last-name"].Value);
			Assert.AreEqual("123", match["house-num"].Value);
			Assert.AreEqual("Elm Street", match["street"].Value);
			Assert.AreEqual("Elm", match["street-name"].Value);
			Assert.AreEqual("Street", match["street-type"].Value);
			Assert.AreEqual("Vancouver", match["town-name"].Value);
			Assert.AreEqual("BC", match["state-code"].Value);
			Assert.AreEqual("V5V5V5", match["zip-code"].Value);
		}
	}
}

