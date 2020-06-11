using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class EtoBenchmark : Benchmark<XmlSuite, GrammarMatch>
	{
		Eto.Parse.Samples.Xml.XmlGrammar grammar;

		public EtoBenchmark()
		{
			grammar = new Eto.Parse.Samples.Xml.XmlGrammar();
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

