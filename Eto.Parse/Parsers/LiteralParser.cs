using System;
using Eto.Parse;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class LiteralParser : Parser
	{
		public string Value { get; set; }

		public override string DescriptiveName
		{
			get { return string.Format("Literal: '{0}'", Value); }
		}

		protected LiteralParser(LiteralParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			Value = other.Value;
			AddError = true;
		}

		public LiteralParser()
		{
		}

		public LiteralParser(string value)
		{
			Value = value;
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
