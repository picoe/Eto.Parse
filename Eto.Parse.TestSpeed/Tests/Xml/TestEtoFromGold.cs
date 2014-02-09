using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.IO;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class TestEtoFromGold : Test<XmlTestSuite>
	{
		Grammars.GoldDefinition gold;

		public TestEtoFromGold()
			: base("Eto.Parse from grm")
		{
		}

		public override void Warmup(XmlTestSuite suite)
		{
			var grm = new StreamReader(GetType().Assembly.GetManifestResourceStream("Eto.Parse.TestSpeed.Tests.Xml.Gold.XML.grm")).ReadToEnd();
			gold = new Grammars.GoldGrammar().Build(grm);
			gold.Grammar.Match(suite.Xml);
		}

		public override void PerformTest(XmlTestSuite suite, StringBuilder output)
		{
			var match = gold.Grammar.Match(suite.Xml);
			if (!match.Success)
			{
				throw new Exception(match.ErrorMessage);
			}
			
		}
	}
}

