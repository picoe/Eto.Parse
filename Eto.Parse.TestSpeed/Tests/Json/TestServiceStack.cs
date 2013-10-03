using System.Text;
using ServiceStack.Text;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class TestServiceStack : Test<JsonTestSuite>
	{
		public TestServiceStack()
			: base("ServiceStack.Text")
		{
		}
		public override void Warmup(JsonTestSuite suite)
		{
			var obj = JsonObject.Parse(suite.Json);
			// need this to parse all of the elements, otherwise servicestack doesn't actually do it
			var result = obj.ArrayObjects("result");
			for (int i = 0; i < result.Count; i++)
			{
				var item = result[i];
				// get child arrays to parse them (newtonsoft.json and eto.parse do this intrinsically)
				item.ArrayObjects("tags");
				item.ArrayObjects("friends");
			}
		}

		public override void PerformTest(JsonTestSuite suite, StringBuilder output)
		{
			var obj = JsonObject.Parse(suite.Json);
			if (suite.CompareOutput)
			{
				// need this to parse all of the elements, otherwise servicestack doesn't actually do it
				var result = obj.ArrayObjects("result");
				for (int i = 0; i < result.Count; i++)
				{
					var item = result[i];
					// get child arrays to parse them (newtonsoft.json and eto.parse do this intrinsically)
					item.ArrayObjects("tags");
					item.ArrayObjects("friends");

					var id = item[suite.CompareProperty];
					output.Append(id);
				}
			}
			else
			{
				// need this to parse all of the elements, otherwise servicestack doesn't actually do it
				var result = obj.ArrayObjects("result");
				for (int i = 0; i < result.Count; i++)
				{
					var item = result[i];
					// get child arrays to parse them (newtonsoft.json and eto.parse do this intrinsically)
					item.ArrayObjects("tags");
					item.ArrayObjects("friends");
				}
			}
		}
	}
}

