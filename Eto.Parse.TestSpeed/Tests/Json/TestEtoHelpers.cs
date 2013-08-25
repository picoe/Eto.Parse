using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class TestEtoHelpers : Test<JsonTestSuite>
	{
		public TestEtoHelpers()
			: base("Eto.Parse-helpers")
		{
		}
		public override void Warmup(JsonTestSuite suite)
		{
			JsonObject.Parse(suite.Json);
		}

		public override void PerformTest(JsonTestSuite suite, StringBuilder output)
		{
			var obj = JsonObject.Parse(suite.Json);
			if (suite.CompareOutput)
			{
				var result = obj["result"] as JsonArray;
				foreach (var item in result.OfType<JsonObject>())
				{
					var id = item[suite.CompareProperty].Value;
					output.Append(id);
				}
			}
		}
	}
}

