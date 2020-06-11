using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Code
{
	public class ListWriter<T> : ParserWriter<T>
		where T: ListParser
	{
		public override void WriteContents(TextParserWriterArgs args, T parser, string name)
		{
			base.WriteContents(args, parser, name);
			var items = new List<string>();
            for (int i = 0; i < parser.Items.Count; i++)
			{
                Parser r = parser.Items[i];
                var child = r != null ? args.Write(r) : "null";
				items.Add(child);
			};
			args.Output.WriteLine("{0}.Items.AddRange(new Eto.Parse.Parser[] {{ {1} }});", name, string.Join(", ", items));
		}
	}
	
}
