using System;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class TestIrony : Test<JsonTestSuite>
	{
		global::Irony.Parsing.Parser parser;

		public TestIrony()
			: base("Irony")
		{
		}

		public override void Warmup(JsonTestSuite suite)
		{
			var g = new global::Irony.Samples.Json.IronyJsonGrammar();
			parser = new global::Irony.Parsing.Parser(g);
			parser.Parse(suite.Json);
		}

		public override void PerformTest(JsonTestSuite suite, StringBuilder output)
		{
			var pt = parser.Parse(suite.Json);
			if (pt.HasErrors())
			{
				foreach (var error in pt.ParserMessages)
					Console.WriteLine("Error: {0}, Location: {1}", error, error.Location);
			}
			if (suite.CompareOutput)
			{
				var results = pt.Root.ChildNodes.First(r => 
				{
					return r.Term.Name == "Property" && r.ChildNodes.Any(s => s.Term.Name == "string" && s.Token.ValueString == "result");
				}).ChildNodes.First(r => r.Term.Name == "Array").ChildNodes;
				for (int j = 0; j < results.Count; j++)
				{
					var item = results[j];
					var id = item.ChildNodes.First(r => r.Term.Name == "Property" && r.ChildNodes.Any(s => s.Term.Name == "string" && s.Token.ValueString == suite.CompareProperty)).ChildNodes.First(r => r.Term.Name == "number").Token.Value;
					output.Append(id);
				}
			}
		}
	}
}

