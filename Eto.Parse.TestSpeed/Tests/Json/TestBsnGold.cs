using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Grammar;
using System.Reflection;

namespace Eto.Parse.TestSpeed.Tests.Json
{
	public class TestBsnGold : Test<JsonTestSuite>
	{
		bsn.GoldParser.Grammar.CompiledGrammar grammar;

		public TestBsnGold()
			: base("bsn.GoldParser")
		{
		}

		public override void Warmup(JsonTestSuite suite)
		{
			grammar = CompiledGrammar.Load(new BinaryReader(GetType().Assembly.GetManifestResourceStream("Eto.Parse.TestSpeed.Tests.Json.Gold.JSON.egt")));
			Parse(suite);
		}

		void Parse(JsonTestSuite suite)
		{
			using (var reader = new StringReader(suite.Json)) {
				var tokenizer = new Tokenizer(reader, grammar);
				var processor = new LalrProcessor(tokenizer, true);
				ParseMessage parseMessage = processor.ParseAll();
				if (parseMessage != ParseMessage.Accept) {
					// you could build a detailed error message here:
					// the position is in processor.CurrentToken.Position
					// and use processor.GetExpectedTokens() on syntax errors
					throw new InvalidOperationException("Parsing failed");
				}
			}
		}

		public override void PerformTest(JsonTestSuite suite, StringBuilder output)
		{
			Parse(suite);
			if (suite.CompareOutput)
			{
				// todo
			}
		}
	}
}

