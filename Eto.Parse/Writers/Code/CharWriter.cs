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
			var testerType = parser.Tester.GetType();
			args.Output.WriteLine("{0}.Tester = new {1}.{2}();", name, testerType.Namespace, testerType.Name);
		}
	}
	
}
