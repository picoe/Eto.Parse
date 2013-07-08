using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Code
{

	public class LiteralWriter : ParserWriter<LiteralParser>
	{
		public override void WriteObject(TextParserWriterArgs args, LiteralParser parser, string name)
		{
			base.WriteObject(args, parser, name);
			if (parser.Value != null)
				args.Output.WriteLine("{0}.Value = \"{1}\";", name, parser.Value.Replace("\"", "\\\""));
		}
	}
	
}
