using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class TestEto : Test<XmlTestSuite>
	{
		Eto.Parse.Samples.Xml.XmlGrammar grammar;

		public TestEto()
			: base("Eto.Parse")
		{
		}

		public override void Warmup(XmlTestSuite suite)
		{
			grammar = new Eto.Parse.Samples.Xml.XmlGrammar();
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

