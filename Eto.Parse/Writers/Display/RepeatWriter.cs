using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Display
{
	public class RepeatWriter : UnaryWriter<RepeatParser>
	{
		public override string GetName(ParserWriterArgs args, RepeatParser parser)
		{
			if (parser.Maximum == Int32.MaxValue)
				return string.Format("{0} [Min: {1}]", base.GetName(args, parser), parser.Minimum);
			else
				return string.Format("{0} [Min: {1}, Max: {2}]", base.GetName(args, parser), parser.Minimum, parser.Maximum);
		}
	}
	
}
