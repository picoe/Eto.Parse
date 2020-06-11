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
			var count = parser.Items.Count;
			for (int i = 0; i < count; i++)
			{
				var item = parser.Items[i];
				args.Write(item);
			}
		}
	}
	
}
