using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Code
{

	public class ListWriter : ParserWriter<ListParser>
	{
		public override void WriteContents(TextParserWriterArgs args, ListParser parser, string name)
		{
			parser.Items.ForEach(r => {
				var child = r != null ? args.Write(r) : "null";
				args.Output.WriteLine("{0}.Items.Add({1});", name, child);
			});
		}
	}
	
}
