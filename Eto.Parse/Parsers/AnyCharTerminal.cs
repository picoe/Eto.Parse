using System;

namespace Eto.Parse.Parsers
{
	public class AnyCharTerminal : Parser
	{
		protected AnyCharTerminal(AnyCharTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public AnyCharTerminal()
		{
		}

		protected override int InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Advance(1);
			return pos < 0 ? -1 : 1;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new AnyCharTerminal(this, args);
		}
	}
}

