using System;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class TestNewtonsoft : Test<JsonTestSuite>
	{
		public TestNewtonsoft()
			: base("Newtonsoft Json")
		{
		}
		public override void Warmup(JsonTestSuite suite)
		{
			var obj = JObject.Parse(suite.Json);
		}

		public override void PerformTest(JsonTestSuite suite, StringBuilder output)
		{
			var obj = JObject.Parse(suite.Json);
			if (suite.CompareOutput)
			{
				var result = obj["result"] as JArray;
				for (int j = 0; j < result.Count; j++)
				{
					var item = result[j];
					var id = item[suite.CompareProperty];
					output.Append(id);
				}
			}
		}
	}
}

