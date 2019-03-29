using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using SpracheJSON;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class SpracheBenchmark : Benchmark<JsonSuite, JSONObject>
	{
		public override JSONObject Execute(JsonSuite suite)
		{
			return JSON.Parse(suite.Json);
		}

		public override bool Verify(JsonSuite suite, JSONObject result)
		{
			if (suite.CompareOutput)
			{
				var output = new StringBuilder();
				var results = (JSONArray)result["result"];
				foreach (var item in results.Elements)
				{
					var id = item[suite.CompareProperty];
					output.Append(id);
				}
				suite.Compare(output.ToString());
			}
			return base.Verify(suite, result);
		}
	}
}
