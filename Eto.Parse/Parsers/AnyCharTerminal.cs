using System;
using System.Collections.Generic;

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
			if (pos >= 0)
				return 1;
			else
				return -1;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new AnyCharTerminal(this, args);
		}
	}
}

