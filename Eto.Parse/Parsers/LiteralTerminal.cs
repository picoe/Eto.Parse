using Eto.Parse;

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
			value.ThrowIfNull("value", "Value must not be null");
			Value = value;
		}

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			caseSensitive = CaseSensitive ?? args.Grammar.CaseSensitive;
		}

		protected override int InnerParse(ParseArgs args)
		{
			return !args.Scanner.ReadString(Value, caseSensitive) ? -1 : Value.Length;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new LiteralTerminal(this, args);
		}
	}
}
