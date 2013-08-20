using System;
using Eto.Parse.Parsers;
using System.Text;
using System.Linq;

namespace Eto.Parse.Writers.Code
{
	public class InverseWriter<T> : ParserWriter<T>
		where T: Parser, IInverseParser
	{
		public override void WriteContents(TextParserWriterArgs args, T parser, string name)
		{
			base.WriteContents(args, parser, name);
			if (parser.Inverse)
				args.Output.WriteLine("{0}.Inverse = {1};", name, parser.Inverse.ToString().ToLower());
		}
	}
}

