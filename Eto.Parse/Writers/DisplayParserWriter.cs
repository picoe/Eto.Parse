using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers
{
	public class DisplayParserWriter : TextParserWriter
	{
		public DisplayParserWriter()
			: base(new Dictionary<Type, IParserWriterHandler<TextParserWriterArgs>>
			{
				{ typeof(Parser), new Display.ParserWriter<Parser>() },
				{ typeof(NamedParser), new Display.NamedWriter() },
				{ typeof(ListParser), new Display.ListWriter() },
				{ typeof(UnaryParser), new Display.UnaryWriter<UnaryParser>() },
				{ typeof(CharParser), new Display.CharWriter() },
				{ typeof(StringParser), new Display.StringWriter() },
				{ typeof(RepeatParser), new Display.RepeatWriter() }
			})
		{
			Indent = " ";
		}
	}
}
