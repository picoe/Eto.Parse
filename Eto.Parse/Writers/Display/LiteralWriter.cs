using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Display
{
	public class LiteralWriter : ParserWriter<LiteralTerminal>
	{
		public override string GetName(ParserWriterArgs args, LiteralTerminal parser)
		{
			return string.Format("{0} [Value: '{1}']", base.GetName(args, parser), parser.Value);
		}
	}
}
