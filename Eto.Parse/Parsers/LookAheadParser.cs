using System;

namespace Eto.Parse.Parsers
{
	public class LookAheadParser : UnaryParser
	{
		protected LookAheadParser(LookAheadParser other)
			: base(other)
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
			if (match != null)
			{
				args.Scanner.Position = pos;
				return null;
			}
			else
				return args.EmptyMatch;
		}

		public override Parser Clone()
		{
			return new LookAheadParser(this);
		}
	}
}
