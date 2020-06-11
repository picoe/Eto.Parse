#if !NETCOREAPP
using System.Text;
using ServiceStack.Text;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class ServiceStackBenchmark : Benchmark<JsonSuite, JsonObject>
	{
		public override JsonObject Execute(JsonSuite suite)
		{
			return JsonObject.Parse(suite.Json);
		}

		public override bool Verify(JsonSuite suite, JsonObject result)
		{
			if (suite.CompareOutput)
			{
				// need this to parse all of the elements, otherwise servicestack doesn't actually do it
				var output = new StringBuilder();
				var obj = result.ArrayObjects("result");
				for (int i = 0; i < obj.Count; i++)
				{
					var item = obj[i];
					// get child arrays to parse them (newtonsoft.json and eto.parse do this intrinsically)
					item.ArrayObjects("tags");
					item.ArrayObjects("friends");

					var id = item[suite.CompareProperty];
					output.Append(id);
				}
				suite.Compare(output.ToString());
			}
			else
			{
				// need this to go through all of the elements, otherwise servicestack doesn't actually do it
				var results = result.ArrayObjects("result");
				for (int i = 0; i < results.Count; i++)
				{
					var item = results[i];
					// get child arrays to parse them (newtonsoft.json and eto.parse do this intrinsically)
					item.ArrayObjects("tags");
					item.ArrayObjects("friends");
				}
			}
			return base.Verify(suite, result);
		}
	}
}

#endif