using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.IO;
using Eto.Parse.Grammars;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class TestEtoFromEbnf : Benchmark<XmlSuite, GrammarMatch>
	{
		Grammar grammar;

		public TestEtoFromEbnf()
		{
			const string grm = Eto.Parse.Tests.Grammars.EbnfW3cTests.xmlW3cEbnf;
			grammar = new EbnfGrammar(EbnfStyle.W3c).Build(grm, "document");
			// no need to return a match for each terminal character
			grammar.SetTerminals("Letter", "BaseChar", "Ideographic", "CombiningChar", "Digit", "Extender", "PubidChar", "Char", "S", "EnumeratedType", "NameChar", "Eq");
			grammar.EnableMatchEvents = false;
		}

		public override GrammarMatch Execute(XmlSuite suite)
		{
			return grammar.Match(suite.Xml);
		}

		public override bool Verify(XmlSuite suite, GrammarMatch result)
		{
			return result.Success;
		}
	}
}
