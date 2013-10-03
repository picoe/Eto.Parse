using System.Text;
using Newtonsoft.Json.Linq;

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
			JObject.Parse(suite.Json);
		}

		public override void PerformTest(JsonTestSuite suite, StringBuilder output)
		{
			var obj = JObject.Parse(suite.Json);
			if (suite.CompareOutput)
			{
				var result = (JArray)obj["result"];
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

