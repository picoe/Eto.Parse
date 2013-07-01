using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Display
{

	public class NamedWriter : UnaryWriter<NamedParser>
	{
		public override string GetName(ParserWriterArgs args, NamedParser parser)
		{
			return string.Format("{0} [Name: {1}]", base.GetName(args, parser), parser.Id);
		}
	}
	
}
