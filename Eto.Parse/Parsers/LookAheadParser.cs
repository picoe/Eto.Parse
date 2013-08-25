using System;

namespace Eto.Parse.Parsers
{
	public class LookAheadParser : UnaryParser, IInverseParser
	{
		public bool Inverse { get; set; }

		protected LookAheadParser(LookAheadParser other, ParserCloneArgs args)
			: base(other, args)
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
				return Inverse ? ParseMatch.None : args.EmptyMatch;
			}
			else
				return Inverse ? args.EmptyMatch : ParseMatch.None;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new LookAheadParser(this, args);
		}
	}
}
