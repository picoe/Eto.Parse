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

		public override IEnumerable<Parser> Children(ParserChain args)
		{
			yield break;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Advance(1);
			if (pos >= 0)
				return new ParseMatch(pos, 1);
			else
				return args.NoMatch;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new AnyCharTerminal(this, args);
		}
	}
}

