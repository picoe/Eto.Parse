using System;
using System.Text;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.IO;
using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace Eto.Parse.TestSpeed.Tests.Xml
{
	public class TestBsnGold : Test<XmlTestSuite>
	{
		bsn.GoldParser.Grammar.CompiledGrammar grammar;

		public TestBsnGold()
			: base("bsn.GoldParser")
		{
		}

		public override void Warmup(XmlTestSuite suite)
		{
			grammar = CompiledGrammar.Load(new BinaryReader(GetType().Assembly.GetManifestResourceStream("Eto.Parse.TestSpeed.Tests.Xml.Gold.XML.egt")));
			Parse(suite);
		}

		void Parse(XmlTestSuite suite)
		{
			using (var reader = new StringReader(suite.Xml))
			{
				var tokenizer = new Tokenizer(reader, grammar);
				var processor = new LalrProcessor(tokenizer, true);
				ParseMessage parseMessage = processor.ParseAll();
				if (parseMessage != ParseMessage.Accept)
				{
					// you could build a detailed error message here:
					// the position is in processor.CurrentToken.Position
					// and use processor.GetExpectedTokens() on syntax errors
					var ct = processor.CurrentToken;
					throw new InvalidOperationException(string.Format("Parsing failed, position: {0}, text: {1}", ct.Position, ct.Text));
				}
			}
		}

		public override void PerformTest(XmlTestSuite suite, StringBuilder output)
		{
			Parse(suite);
		}
	}
}

