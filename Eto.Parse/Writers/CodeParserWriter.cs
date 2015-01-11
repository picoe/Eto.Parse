using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

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
				{ typeof(ListParser), new Code.ListWriter<ListParser>() },
				{ typeof(UnaryParser), new Code.UnaryWriter<UnaryParser>() },
				{ typeof(LiteralTerminal), new Code.LiteralWriter() },
				{ typeof(RepeatParser), new Code.RepeatWriter() },
				{ typeof(GroupParser), new Code.GroupWriter() },
				{ typeof(SequenceParser), new Code.SequenceWriter() },
				{ typeof(ExceptParser), new Code.ExceptWriter() },
				{ typeof(StringParser), new Code.StringWriter() },
				{ typeof(NumberParser), new Code.NumberWriter() },
				{ typeof(CharRangeTerminal), new Code.CharRangeWriter() },
				{ typeof(CharSetTerminal), new Code.CharSetWriter() },
				{ typeof(BooleanTerminal), new Code.BooleanWriter() },
				{ typeof(CharTerminal), new Code.CharWriter() },
				{ typeof(SingleCharTerminal), new Code.SingleCharWriter() },
				{ typeof(LookAheadParser), new Code.InverseWriter<LookAheadParser>() },
				{ typeof(TagParser), new Code.TagWriter() },
			})
		{
		}
	}
}
