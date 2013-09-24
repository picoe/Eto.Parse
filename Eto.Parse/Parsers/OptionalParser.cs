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

		protected override int InnerParse(ParseArgs args)
		{
			if (!HasNamedChildren)
			{
				var match = Inner.Parse(args);

				if (match >= 0)
					return match;
				else
					return 0;
			}
			else
			{
				args.Push();
				var match = Inner.Parse(args);

				if (match >= 0)
				{
					args.PopSuccess();
					return match;
				}
				else
				{
					args.PopFailed();
					return 0;
				}
			}
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new OptionalParser(this, chain);
		}
	}
}
