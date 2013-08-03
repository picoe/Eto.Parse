using System;

namespace Eto.Parse.Parsers
{
	public class ExceptParser : UnaryParser
	{
		public Parser Except { get; set; }

		protected ExceptParser(ExceptParser other, ParserCloneArgs args)
			: base(other, args)
		{
			Except = args.Clone(other.Except);
		}

		public ExceptParser()
		{
		}

		public ExceptParser(Parser inner, Parser except)
		{
			this.Inner = inner;
			this.Except = except;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Position;
			var match = Except.Parse(args);
			if (!match.Success)
				return base.InnerParse(args);
			else
			{
				args.Scanner.SetPosition(pos);
				return args.NoMatch;
			}
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new ExceptParser(this, args);
		}
	}
}

