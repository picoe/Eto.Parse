using System;

namespace Eto.Parse.Parsers
{
	public class LookAheadParser : UnaryParser
	{
		protected LookAheadParser(LookAheadParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
		}

		public LookAheadParser(Parser inner)
		: base(inner)
		{
		}
		
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Position;
			var match = Inner.Parse(args);
			if (match.Success)
			{
				args.Scanner.SetPosition(pos);
				return args.NoMatch;
			}
			else
				return new ParseMatch(pos, 0);
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new LookAheadParser(this, chain);
		}
	}
}
