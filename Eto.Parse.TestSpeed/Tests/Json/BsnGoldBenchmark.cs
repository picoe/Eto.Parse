#if !NETCOREAPP
using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Grammar;
using System.Reflection;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class BsnGoldBenchmark : Benchmark<JsonSuite, object>
	{
		bsn.GoldParser.Grammar.CompiledGrammar grammar;

		public BsnGoldBenchmark()
		{
			grammar = CompiledGrammar.Load(new BinaryReader(GetType().Assembly.GetManifestResourceStream("Eto.Parse.TestSpeed.Tests.Json.Gold.JSON.egt")));
		}

		public override object Execute(JsonSuite suite)
		{
			using (var reader = new StringReader(suite.Json))
			{
				var tokenizer = new Tokenizer(reader, grammar);
				var processor = new LalrProcessor(tokenizer, true);
				ParseMessage parseMessage = processor.ParseAll();
				if (parseMessage != ParseMessage.Accept)
				{
					// you could build a detailed error message here:
					// the position is in processor.CurrentToken.Position
					// and use processor.GetExpectedTokens() on syntax errors
					throw new InvalidOperationException("Parsing failed");
				}

				return null; // how do we actually get the results?? wierd.
			}
		}
	}
}
#endif