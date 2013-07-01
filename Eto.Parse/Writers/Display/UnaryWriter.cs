using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Display
{
	public class UnaryWriter<T> : ParserWriter<T>
		where T: UnaryParser
	{
		public override void WriteContents(TextParserWriterArgs args, T parser, string name)
		{
			args.Write(parser.Inner);
		}
	}
	
}
