using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers
{
	public class CodeParserWriter : TextParserWriter
	{
		public CodeParserWriter()
			: base(new Dictionary<Type, IParserWriterHandler<TextParserWriterArgs>>
			{
				{ typeof(Parser), new Code.ParserWriter<Parser>() },
				{ typeof(NamedParser), new Code.NamedWriter() },
				{ typeof(ListParser), new Code.ListWriter() },
				{ typeof(UnaryParser), new Code.UnaryWriter<UnaryParser>() },
				{ typeof(CharParser), new Code.CharWriter() },
				{ typeof(StringParser), new Code.StringWriter() },
				{ typeof(RepeatParser), new Code.RepeatWriter() }
			})
		{
		}
	}
}
