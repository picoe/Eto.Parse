using System;
using Eto.Parse;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class LiteralParser : Parser
	{
		string value;
		public string Value { get { return this.value; } set { this.value = value; } }

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
			if (value == null)
				return args.EmptyMatch;

			var scanner = args.Scanner;
			int pos = scanner.Position;
			if (scanner.ReadString(value, args.CaseSensitive))
				return new ParseMatch(pos, value.Length);

			scanner.SetPosition(pos);
			return args.NoMatch;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new LiteralParser(this, chain);
		}

		public override IEnumerable<Parser> Children(ParserChain args)
		{
			yield break;
		}
	}
}
