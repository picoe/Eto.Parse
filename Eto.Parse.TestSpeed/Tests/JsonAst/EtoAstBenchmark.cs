using System;
using Eto.Parse.Samples.Json.AstObject;
using Eto.Parse.Samples.Json;

namespace Eto.Parse.TestSpeed.Tests.JsonAst
{
	public class EtoAstBenchmark : Benchmark<JsonAstSuite, SampleObject>
    {
        JsonGrammar grammar;
        JsonObjectAstBuilder jsonAst;

        public EtoAstBenchmark()
        {
            jsonAst = new JsonObjectAstBuilder();
            grammar = new Eto.Parse.Samples.Json.JsonGrammar();
        }

        public override SampleObject Execute(JsonAstSuite suite)
        {
            var match = grammar.Match(suite.Json);
            return jsonAst.Build(match);
        }

        public override bool Verify(JsonAstSuite suite, SampleObject result)
        {
			if (result?.Result == null || result.Result.Count == 0)
				return false;
			
			var first = result.Result[0];
			if (first.Guid != new Guid("613cad29-f7dd-4ec6-be4d-259e37fe1261"))
				return false;
			if (!first.IsActive)
				return false;
			if (first.Name != "David Alvarado")
				return false;
				
			return true;
        }

    }

}
