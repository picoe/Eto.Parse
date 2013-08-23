using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using Eto.Parse.TestSpeed.Tests.Json.Gold;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class TestGold : Test<JsonTestSuite>
	{
		GoldJsonParser parser;

		public TestGold()
			: base("Gold Parser")
		{
		}

		public override void Warmup(JsonTestSuite suite)
		{
			parser = new GoldJsonParser();
			parser.Parse(suite.Json);
		}

		public override void PerformTest(JsonTestSuite suite, StringBuilder output)
		{
			parser.Parse(suite.Json);
			if (suite.CompareOutput)
			{
				// don't even bother - gold parser is suuuper slow so no point in trying
				//var red = g.Result as GOLD.Reduction;
				//var members = (GOLD.TokenList)((GOLD.TokenList)red[0].Data)[1].Data;
			}
		}
	}
}

