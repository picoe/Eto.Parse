using System;
using Eto.Parse;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class LiteralTerminal : Parser
	{
		bool caseSensitive;
		public bool? CaseSensitive { get; set; }

		public string Value { get; set; }

		public override string DescriptiveName
		{
			get { return string.Format("Literal: '{0}'", Value); }
		}

		protected LiteralTerminal(LiteralTerminal other, ParserCloneArgs chain)
			: base(other, chain)
		{
			CaseSensitive = other.CaseSensitive;
			Value = other.Value;
		}

		public LiteralTerminal()
		{
		}

		public LiteralTerminal(string value)
		{
			Value = value;
		}

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			caseSensitive = CaseSensitive ?? args.Grammar.CaseSensitive;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (Value == null)
				return args.EmptyMatch;

			var scanner = args.Scanner;
			int pos = scanner.Position;
			if (scanner.ReadString(Value, caseSensitive))
				return new ParseMatch(pos, Value.Length);

			return args.NoMatch;
		}

		public override Parser Clone(ParserCloneArgs chain)
		{
			return new LiteralTerminal(this, chain);
		}

		public override IEnumerable<Parser> Children(ParserChain args)
		{
			yield break;
		}
	}
}
