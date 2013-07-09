using System;

namespace Eto.Parse.Parsers
{
	public class OptionalParser : UnaryParser
	{
		protected OptionalParser(OptionalParser other, ParserCloneArgs chain)
			: base(other, chain)
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

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new OptionalParser(this, chain);
		}
		
	}
}
