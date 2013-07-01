using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Display
{

	public class CharWriter : ParserWriter<CharParser>
	{
		public override string GetName(ParserWriterArgs args, CharParser parser)
		{
			return string.Format("{0} [Tester: {1}]", base.GetName(args, parser), parser.Tester.GetType().Name);
		}
	}
	
}
