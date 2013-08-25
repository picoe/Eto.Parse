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

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Advance(1);
			if (pos >= 0)
				return new ParseMatch(pos, 1);
			else
				return ParseMatch.None;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new AnyCharTerminal(this, args);
		}
	}
}

