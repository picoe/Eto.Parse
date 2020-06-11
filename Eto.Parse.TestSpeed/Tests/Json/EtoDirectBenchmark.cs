using System;
using System.Text;
using Eto.Parse.Samples.Json.MatchTokens;

namespace Eto.Parse.TestSpeed.Tests.Json
{
    public class EtoDirectBenchmark : Benchmark<JsonSuite, GrammarMatch>
    {
        Eto.Parse.Samples.Json.JsonGrammar grammar;

        public EtoDirectBenchmark()
        {
            grammar = new Eto.Parse.Samples.Json.JsonGrammar();
        }

        public override GrammarMatch Execute(JsonSuite suite)
        {
            return grammar.Match(suite.Json);
        }

		public override bool Verify(JsonSuite suite, GrammarMatch result)
		{
			if (suite.CompareOutput)
			{
				var output = new StringBuilder();
				var results = JsonObject.GetProperty(result.Matches[0], "result");
				for (int j = 0; j < results.Matches.Count; j++)
				{
					var item = results.Matches[j];
					var id = JsonObject.GetProperty(item, suite.CompareProperty).Value;
					output.Append(id);
				}
				suite.Compare(output.ToString());
			}

			return result.Success;
		}
	}

}
