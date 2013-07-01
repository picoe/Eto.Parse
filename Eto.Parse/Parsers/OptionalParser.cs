using System;

namespace Eto.Parse.Parsers
{
	public class OptionalParser : UnaryParser
	{
		protected OptionalParser(OptionalParser other)
			: base(other)
		{
		}

		public OptionalParser()
		{
		}

		public OptionalParser(Parser inner)
		: base(inner)
		{
			
		}
		
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (!args.Push(this)) return args.NoMatch;
			
			ParseMatch match = Inner.Parse(args);
			args.Pop(true);
			if (!match.Success)
			{
				return args.EmptyMatch;
			}
			
			return match;
		}

		public override Parser Clone()
		{
			return new OptionalParser(this);
		}
		
	}
}
