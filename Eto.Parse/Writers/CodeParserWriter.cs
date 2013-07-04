using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;
using Eto.Parse.Testers;

namespace Eto.Parse.Writers
{
	public class CodeParserWriter : TextParserWriter
	{
		public CodeParserWriter()
			: base(new ParserDictionary
			{
				{ typeof(Parser), new Code.ParserWriter<Parser>() },
				{ typeof(NonTerminalParser), new Code.NamedWriter() },
				{ typeof(ListParser), new Code.ListWriter() },
				{ typeof(UnaryParser), new Code.UnaryWriter<UnaryParser>() },
				{ typeof(CharParser), new Code.CharWriter() },
				{ typeof(StringParser), new Code.StringWriter() },
				{ typeof(RepeatParser), new Code.RepeatWriter() }
			}, 
			new TesterDictionary
			{
				{ typeof(ICharTester), new Code.TesterWriter<ICharTester>() },
				{ typeof(IncludeTester), new Code.IncludeTesterWriter() },
				{ typeof(ExcludeTester), new Code.ExcludeTesterWriter() },
				{ typeof(RangeTester), new Code.RangeTesterWriter() },
			})
		{
		}
	}
}
