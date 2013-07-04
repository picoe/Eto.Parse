using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Code
{

	public class NamedWriter : UnaryWriter<NonTerminalParser>
	{
		public static string GetIdentifier(string parserId)
		{
			return parserId.Replace(' ', '_').Replace('-', '_');
		}

		public override string GetName(TextParserWriterArgs args, NonTerminalParser parser)
		{
			return GetIdentifier(parser.Id);
		}

		public override void WriteObject(TextParserWriterArgs args, NonTerminalParser parser, string name)
		{
			args.Output.WriteLine();
			base.WriteObject(args, parser, name);
			args.Output.WriteLine("{0}.Id = \"{1}\";", name, parser.Id);
		}
	}
	
}
