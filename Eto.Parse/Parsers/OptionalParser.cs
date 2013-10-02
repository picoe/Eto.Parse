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

				return match < 0 ? 0 : match;
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
				args.PopFailed();
				return 0;
			}
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new OptionalParser(this, args);
		}
	}
}
