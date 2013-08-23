using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class TestEtoDirect : Test<JsonTestSuite>
	{
		Eto.Parse.Samples.Json.JsonGrammar grammar;

		public TestEtoDirect()
			: base("Eto.Parse-direct")
		{
		}

		public override void Warmup(JsonTestSuite suite)
		{
			grammar = new Eto.Parse.Samples.Json.JsonGrammar();
			var match = grammar.Match(suite.Json);
		}

		public override void PerformTest(JsonTestSuite suite, StringBuilder output)
		{
			var match = grammar.Match(suite.Json);
			if (!match.Success)
			{
				throw new Exception(match.ErrorMessage);
			}
			if (suite.CompareOutput)
			{
				var result = JsonObject.GetProperty(match.Matches[0], "result");
				for (int j = 0; j < result.Matches.Count; j++)
				{
					var item = result.Matches[j];
					var id = JsonObject.GetProperty(item, suite.CompareProperty).Value;
					output.Append(id);
				}
			}
		}
	}
}

