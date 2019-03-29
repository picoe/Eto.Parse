using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.IO;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class TestEtoFromGold : Benchmark<XmlSuite, GrammarMatch>
	{
		Grammars.GoldDefinition gold;

		public TestEtoFromGold()
		{
			var grm = new StreamReader(GetType().Assembly.GetManifestResourceStream("Eto.Parse.TestSpeed.Tests.Xml.Gold.XML.grm")).ReadToEnd();
			gold = new Grammars.GoldGrammar().Build(grm);
			gold.Grammar.EnableMatchEvents = false;
			gold.Grammar.Initialize();
		}

		public override GrammarMatch Execute(XmlSuite suite)
		{
			return gold.Grammar.Match(suite.Xml);
		}

		public override bool Verify(XmlSuite suite, GrammarMatch result)
		{
			return result.Success;
		}
	}
}