using System;
using System.Linq;
using System.Text;
using Eto.Parse.Samples.Json.MatchTokens;

namespace Eto.Parse.TestSpeed.Tests.Json
{
    public class EtoHelpersBenchmark : Benchmark<JsonSuite, JsonObject>
    {
        Eto.Parse.Samples.Json.JsonGrammar grammar;

        public EtoHelpersBenchmark()
        {
            grammar = new Eto.Parse.Samples.Json.JsonGrammar();
        }

        public override JsonObject Execute(JsonSuite suite)
        {
            return JsonObject.Parse(suite.Json) as JsonObject;
        }

		public override bool Verify(JsonSuite suite, JsonObject result)
		{
			if (suite.CompareOutput)
			{
				var output = new StringBuilder();
				var results = (JsonArray)result["result"];
				foreach (var item in results.OfType<JsonObject>())
				{
					var id = item[suite.CompareProperty].Value;
					output.Append(id);
				}
				suite.Compare(output.ToString());
			}
			return result != null;
		}
	}

}
