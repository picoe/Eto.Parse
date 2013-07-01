using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Display
{

	public class StringWriter : ParserWriter<StringParser>
	{
		public override string GetName(ParserWriterArgs args, StringParser parser)
		{
			return string.Format("{0} [Value: '{1}']", base.GetName(args, parser), parser.Value);
		}
	}
	
}
