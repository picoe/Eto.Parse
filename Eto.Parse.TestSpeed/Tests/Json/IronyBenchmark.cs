using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using Irony.Parsing;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class IronyBenchmark : Benchmark<JsonSuite, ParseTree>
	{
		global::Irony.Parsing.Parser parser;

		public IronyBenchmark()
		{
			var g = new global::Irony.Samples.Json.IronyJsonGrammar();
			parser = new global::Irony.Parsing.Parser(g);
		}

		public override ParseTree Execute(JsonSuite suite)
		{
			return parser.Parse(suite.Json);
		}

		public override bool Verify(JsonSuite suite, ParseTree result)
		{
			if (result.HasErrors())
			{
				foreach (var error in result.ParserMessages)
					Console.WriteLine("Error: {0}, Location: {1}", error, error.Location);
			}
			if (suite.CompareOutput)
			{
				var output = new StringBuilder();
				var results = result.Root.ChildNodes.First(r =>
				{
					return r.Term.Name == "Property" && r.ChildNodes.Any(s => s.Term.Name == "string" && s.Token.ValueString == "result");
				}).ChildNodes.First(r => r.Term.Name == "Array").ChildNodes;
				for (int j = 0; j < results.Count; j++)
				{
					var item = results[j];
					var id = item.ChildNodes.First(r => r.Term.Name == "Property" && r.ChildNodes.Any(s => s.Term.Name == "string" && s.Token.ValueString == suite.CompareProperty)).ChildNodes.First(r => r.Term.Name == "number").Token.Value;
					output.Append(id);
				}
				suite.Compare(output.ToString());
			}
			return !result.HasErrors();
		}
	}
}

