using System;
using System.Linq;
using Eto.Parse.Parsers;

namespace Eto.Parse.Writers.Code
{
	public class CharWriter : InverseWriter<CharTerminal>
	{
		public override void WriteContents(TextParserWriterArgs args, CharTerminal parser, string name)
		{
			base.WriteContents(args, parser, name);
			if (parser.CaseSensitive != null)
				args.Output.WriteLine("{0}.CaseSensitive = {1};", name, parser.CaseSensitive.HasValue ? parser.CaseSensitive.ToString().ToLowerInvariant() : "null");
		}
	}
}

