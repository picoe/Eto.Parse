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
			args.Push(this);
			var match = Inner.Parse(args);

			if (match.Success)
			{
				args.Pop(true);
				return match;
			}

			args.Pop(false);
			return args.EmptyMatch;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new OptionalParser(this, chain);
		}
	}
}
