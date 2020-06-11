using System;
using System.Linq;
using System.Text;
using Eto.Parse.Samples.Json.Ast;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class EtoAstBenchmark : Benchmark<JsonSuite, JsonToken>
	{
		Eto.Parse.Samples.Json.JsonGrammar grammar;
		JsonAstBuilder jsonAst;

		public EtoAstBenchmark()
		{
			jsonAst = new JsonAstBuilder();
			grammar = new Eto.Parse.Samples.Json.JsonGrammar();
		}

		public override JsonToken Execute(JsonSuite suite)
		{
			return jsonAst.Build(grammar.Match(suite.Json));
		}

		public override bool Verify(JsonSuite suite, JsonToken result)
		{
			if (suite.CompareOutput)
			{
				var obj = (JsonObject)result;
				var array = (JsonArray)obj["result"];
				var output = new StringBuilder();
				foreach (var item in array.OfType<JsonObject>())
				{
					var id = (JsonValue)item[suite.CompareProperty];
					output.Append(id.Value);
				}
				suite.Compare(output.ToString());
			}

			return result != null;
		}

	}

}
