using System;
using Eto.Parse.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Eto.Parse.Writers.Code
{

	public class RepeatWriter : UnaryWriter<RepeatParser>
	{
		public override void WriteObject(TextParserWriterArgs args, RepeatParser parser, string name)
		{
			base.WriteObject(args, parser, name);
			args.Output.WriteLine("{0}.Minimum = {1};", name, parser.Minimum);
			if (parser.Maximum != Int32.MaxValue)
				args.Output.WriteLine("{0}.Maximum = {1};", name, parser.Maximum);
		}
	}
	
}
