using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.IO;
using Eto.Parse.Grammars;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class TestEtoFromEbnf : Test<XmlTestSuite>
	{
		Grammar grammar;

		public TestEtoFromEbnf()
			: base("Eto.Parse with w3c ebnf spec")
		{
		}

		public override void Warmup(XmlTestSuite suite)
		{
			const string grm = Eto.Parse.Tests.Grammars.EbnfW3cTests.xmlW3cEbnf;
			grammar = new EbnfGrammar(EbnfStyle.W3c).Build(grm, "document");
			grammar.Match(suite.Xml);
		}

		public override void PerformTest(XmlTestSuite suite, StringBuilder output)
		{
			var match = grammar.Match(suite.Xml);
			if (!match.Success)
			{
				throw new Exception(match.ErrorMessage);
			}
			
		}
	}
}

