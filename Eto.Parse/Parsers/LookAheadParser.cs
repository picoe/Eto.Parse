using System;

namespace Eto.Parse.Parsers
{
	public class LookAheadParser : UnaryParser, IInverseParser
	{
		public bool Inverse { get; set; }

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
				return Inverse ? args.NoMatch : args.EmptyMatch;
			}
			else
				return Inverse ? args.EmptyMatch : args.NoMatch;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new LookAheadParser(this, chain);
		}
	}
}
