using System;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class TestSpracheJSON : Test<JsonTestSuite>
	{
		public TestSpracheJSON()
			: base("SpracheJSON")
		{
		}

		public override void Warmup(JsonTestSuite suite)
		{
			global::SpracheJSON.JSON.Parse(suite.Json);
		}

		public override void PerformTest(JsonTestSuite suite, StringBuilder output)
		{
			var json = global::SpracheJSON.JSON.Parse(suite.Json);
			if (suite.CompareOutput)
			{
				var result = (global::SpracheJSON.JSONArray)json["result"];
				foreach (var item in result.Elements)
				{
					var id = item[suite.CompareProperty];
					output.Append(id);
				}
			}
		}
	}
}
