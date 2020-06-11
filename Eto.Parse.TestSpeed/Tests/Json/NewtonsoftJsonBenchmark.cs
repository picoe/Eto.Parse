using System.Text;
using Newtonsoft.Json.Linq;

namespace Eto.Parse.TestSpeed.Tests.Json
{

    public class NewtonsoftJsonBenchmark : Benchmark<JsonSuite, JObject>
    {
        public override JObject Execute(JsonSuite suite)
        {
            return JObject.Parse(suite.Json);
        }

        public override bool Verify(JsonSuite suite, JObject result)
        {
			if (suite.CompareOutput)
			{
				var output = new StringBuilder();
				var results = (JArray)result["result"];
				for (int j = 0; j < results.Count; j++)
				{
					var item = results[j];
					var id = item[suite.CompareProperty];
					output.Append(id);
				}
				suite.Compare(output.ToString());
			}

            return base.Verify(suite, result);
        }
    }

}

