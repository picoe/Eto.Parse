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

		public OptionalParser(string name, Parser inner)
			: base(name, inner)
		{
		}

		public OptionalParser(Parser inner)
			: base(null, inner)
		{
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (!HasNamedChildren)
			{
				var match = Inner.Parse(args);

				if (match.Success)
					return match;
				else
					return args.EmptyMatch;
			}
			else
			{
				args.Push();
				var match = Inner.Parse(args);

				if (match.Success)
				{
					args.PopSuccess();
					return match;
				}
				else
				{
					args.PopFailed();
					return args.EmptyMatch;
				}
			}
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new OptionalParser(this, chain);
		}
	}
}
