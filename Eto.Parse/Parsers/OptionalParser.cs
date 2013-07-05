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
			if (args.IsRecursive(this))
				return args.NoMatch;

			args.Push(this);
			ParseMatch match = Inner.Parse(args);
			args.Pop(true);
			if (match.Success)
				return match;
			else
				return args.EmptyMatch;
		}

		public override Parser Clone()
		{
			return new OptionalParser(this);
		}
		
	}
}
