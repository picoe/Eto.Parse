using System;
using Eto.Parse;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class LiteralParser : Parser
	{
		public string Value { get; set; }

		protected LiteralParser(LiteralParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			Value = other.Value;
		}

		public LiteralParser()
		{
		}

		public LiteralParser(string value)
		{
			Value = value;
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			yield break;
		}
		
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (Value == null)
				return args.EmptyMatch;

			int pos = args.Scanner.Position;
			if (!args.Scanner.ReadString(Value, args.Grammar.CaseSensitive))
				return args.NoMatch;

			return new ParseMatch(pos, Value.Length);
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new LiteralParser(this, chain);
		}
	}
}
