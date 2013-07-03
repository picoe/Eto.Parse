using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Code
{

	public class CharWriter : ParserWriter<CharParser>
	{
		public override void WriteObject(TextParserWriterArgs args, CharParser parser, string name)
		{
			base.WriteObject(args, parser, name);
			if (parser.Tester != null)
			{
				args.Output.WriteLine("{0}.Tester = {1};", name, args.Write(parser.Tester));
			}
		}
	}
	
}
