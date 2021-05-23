#if !NETCOREAPP
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
	public class TestBsnGold : Benchmark<XmlSuite, object>
	{
		bsn.GoldParser.Grammar.CompiledGrammar grammar;

		public TestBsnGold()
		{
			grammar = CompiledGrammar.Load(new BinaryReader(GetType().Assembly.GetManifestResourceStream("Eto.Parse.TestSpeed.Tests.Xml.Gold.XML.egt")));
		}

		public override object Execute(XmlSuite suite)
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
				return null;
			}
		}
	}
}

#endif