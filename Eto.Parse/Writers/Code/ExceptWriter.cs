using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Writers.Code
{
	public class ExceptWriter : UnaryWriter<ExceptParser>
	{
		public override void WriteContents(TextParserWriterArgs args, ExceptParser parser, string name)
		{
			base.WriteContents(args, parser, name);
			if (parser.Except != null)
				args.Output.WriteLine("{0}.Except = {1};", name, args.Write(parser.Except));
		}
	}
}

