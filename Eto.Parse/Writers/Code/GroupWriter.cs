using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Writers.Code
{
	public class GroupWriter : ParserWriter<GroupParser>
	{
		public override void WriteContents(TextParserWriterArgs args, GroupParser parser, string name)
		{
			base.WriteContents(args, parser, name);
			if (parser.Start != null)
				args.Output.WriteLine("{0}.Start = {1};", name, args.Write(parser.Start));
			if (parser.End != null)
				args.Output.WriteLine("{0}.End = {1};", name, args.Write(parser.End));
			if (parser.Line != null)
				args.Output.WriteLine("{0}.Line = {1};", name, args.Write(parser.Line));
		}
	}
}

