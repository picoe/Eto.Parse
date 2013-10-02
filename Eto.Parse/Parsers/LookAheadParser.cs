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
		
		protected override int InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Position;
			var match = Inner.Parse(args);
			if (match >= 0)
			{
				args.Scanner.Position = pos;
				return Inverse ? -1 : 0;
			}
			return Inverse ? 0 : -1;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new LookAheadParser(this, args);
		}
	}
}
