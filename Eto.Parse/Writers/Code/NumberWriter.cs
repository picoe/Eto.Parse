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
				args.Output.WriteLine("{0}.AllowDecimal = {1};", name, parser.AllowDecimal.ToString().ToLower());
			if (parser.AllowExponent)
				args.Output.WriteLine("{0}.AllowExponent = {1};", name, parser.AllowExponent.ToString().ToLower());
			if (parser.AllowSign)
				args.Output.WriteLine("{0}.AllowSign = {1};", name, parser.AllowSign.ToString().ToLower());
			if (parser.ValueType != null)
				args.Output.WriteLine("{0}.ValueType = typeof({1});", name, parser.ValueType.FullName);
			if (parser.DecimalSeparator != '.')
				args.Output.WriteLine("{0}.DecimalSeparator = (char)0x{1};", name, (int)parser.DecimalSeparator);
		}
	}
}

