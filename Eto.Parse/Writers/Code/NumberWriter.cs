using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Writers.Code
{
	public class NumberWriter : ParserWriter<NumberParser>
	{
		public override void WriteContents(TextParserWriterArgs args, NumberParser parser, string name)
		{
			base.WriteContents(args, parser, name);
			if (parser.AllowDecimal)
				args.Output.WriteLine("{0}.AllowDecimal = {1};", name, parser.AllowDecimal);
			if (parser.AllowExponent)
				args.Output.WriteLine("{0}.AllowExponent = {1};", name, parser.AllowExponent);
			if (parser.AllowSign)
				args.Output.WriteLine("{0}.AllowSign = {1};", name, parser.AllowSign);
			if (parser.DecimalSeparator != '.')
				args.Output.WriteLine("{0}.DecimalSeparator = (char)0x{1};", name, (int)parser.DecimalSeparator);
		}
	}
}

