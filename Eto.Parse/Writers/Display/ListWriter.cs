using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Display
{
	public class ListWriter : ParserWriter<ListParser>
	{
		public override void WriteContents(TextParserWriterArgs args, ListParser parser, string name)
		{
			foreach (var item in parser.Items)
				args.Write(item);
		}
	}
	
}
