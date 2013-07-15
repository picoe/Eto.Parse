using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Code
{

	public class NamedWriter : UnaryWriter<NamedParser>
	{
		public static string GetIdentifier(string parserId)
		{
			return parserId.Replace(' ', '_').Replace('-', '_');
		}

		public override string GetName(TextParserWriterArgs args, NamedParser parser)
		{
			return args.GenerateName(parser, GetIdentifier(parser.Name));
		}

		public override void WriteObject(TextParserWriterArgs args, NamedParser parser, string name)
		{
			args.Output.WriteLine();
			base.WriteObject(args, parser, name);
			args.Output.WriteLine("{0}.Name = \"{1}\";", name, parser.Name);
		}
	}
	
}
