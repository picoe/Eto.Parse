using System;
using System.Linq;
using System.Text;
using Eto.Parse.Samples.Json.Ast;
using Eto.Parse.Ast;
using System.Collections.Generic;

namespace Eto.Parse.TestSpeed.Tests.JsonAst
{
    public class EtoAstBenchmark : Benchmark<JsonAstSuite, SampleObject>
    {
        Eto.Parse.Samples.Json.JsonGrammar grammar;
        AstBuilder<SampleObject> jsonAst;

        public EtoAstBenchmark()
        {
            jsonAst = new AstBuilder<SampleObject>();
            var top = jsonAst.Create<SampleObject>("object");
            var properties = top.Children("property");
            properties.Condition("name", "id").Property<int>("number", (o, v) => o.Id = v);
            properties.Condition("name", "jsonrpc").Property<string>("string", (o, v) => o.JsonRpc = v);
            properties.Condition("name", "total").Property<int>("number", (o, v) => o.Total = v);
            var child = properties.Condition("name", "result").Children("array")
                .HasMany<List<SampleInfo>, SampleInfo>(c => c.Result, (c, v) => c.Result = v)
                .Create<SampleInfo>("object").Children("property");

            child.Condition("name", "id").Property<int>("number", (o, v) => o.Id = v);
            child.Condition("name", "age").Property<int>("number", (o, v) => o.Age = v);
            child.Condition("name", "isActive").Property<bool>("bool", (o, v) => o.IsActive = v);
            child.Condition("name", "name").Property<string>("string", (o, v) => o.Name = v);
            child.Condition("name", "guid").Property<string>("string", (o, v) =>
            {
                if (Guid.TryParse(v, out var g)) o.Guid = g;
            });

            jsonAst.Initialize();
            grammar = new Eto.Parse.Samples.Json.JsonGrammar();
        }

        public override SampleObject Execute(JsonAstSuite suite)
        {
            var match = grammar.Match(suite.Json);
            return jsonAst.Build(match);
        }

        public override bool Verify(JsonAstSuite suite, SampleObject result)
        {
            return result != null && result.Result.Count > 0;
        }

    }

}
