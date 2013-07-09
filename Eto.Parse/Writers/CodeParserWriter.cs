using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;
using Eto.Parse.Testers;

namespace Eto.Parse.Writers
{
	public class CodeParserWriter : TextParserWriter
	{
		public string ClassName { get; set; }

		public CodeParserWriter()
			: base(new ParserDictionary
			{
				{ typeof(Parser), new Code.ParserWriter<Parser>() },
				{ typeof(Grammar), new Code.GrammarWriter() },
				{ typeof(NamedParser), new Code.NamedWriter() },
				{ typeof(ListParser), new Code.ListWriter<ListParser>() },
				{ typeof(UnaryParser), new Code.UnaryWriter<UnaryParser>() },
				{ typeof(CharParser), new Code.CharWriter() },
				{ typeof(LiteralParser), new Code.LiteralWriter() },
				{ typeof(RepeatParser), new Code.RepeatWriter() },
				{ typeof(GroupParser), new Code.GroupWriter() },
				{ typeof(SequenceParser), new Code.SequenceWriter() },
			}, 
			new TesterDictionary
			{
				{ typeof(ICharTester), new Code.TesterWriter<ICharTester>() },
				{ typeof(IncludeTester), new Code.IncludeTesterWriter() },
				{ typeof(ExcludeTester), new Code.ExcludeTesterWriter() },
				{ typeof(RangeTester), new Code.RangeTesterWriter() },
				{ typeof(CharSetTester), new Code.CharSetTesterWriter() },
			})
		{
		}
	}
}
