using System;

namespace Eto.Parse.Parsers
{
	public class NotParser : UnaryParser
	{
		protected NotParser(NotParser other)
			: base(other)
		{
		}

		public NotParser(Parser inner)
		: base(inner)
		{
		}
		
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (!args.Push(this)) return args.NoMatch;
			
			var match = Inner.Parse(args);
			args.Pop(true);
			if (match.Success)
				return args.NoMatch;
			else
				return args.EmptyMatch;
		}

		public override Parser Clone()
		{
			return new NotParser(this);
		}
	}
}
