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
				var child = args.Write(r);
				args.Output.WriteLine("{0}.Items.Add({1});", name, child);
			});
		}
	}
	
}
