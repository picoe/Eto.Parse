using System;

namespace Eto.Parse.Parsers
{
	public class BooleanTerminal : Parser
	{
		bool caseSensitive;

		public bool? CaseSensitive { get; set; }

		public string[] TrueValues { get; set; }

		public string[] FalseValues { get; set; }

		protected BooleanTerminal(BooleanTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
			this.CaseSensitive = other.CaseSensitive;
			this.TrueValues = other.TrueValues != null ? (string[])other.TrueValues.Clone() : null;
			this.FalseValues = other.FalseValues != null ? (string[])other.FalseValues.Clone() : null;
		}

		public BooleanTerminal()
		{
			TrueValues = new string[] { "true", "yes", "on", "1" };
			FalseValues = new string[] { "false", "no", "off", "0" };
		}

		public override void Initialize(ParserInitializeArgs args)
		{
			base.Initialize(args);
			caseSensitive = CaseSensitive ?? args.Grammar.CaseSensitive;
		}

		public override object GetValue(string text)
		{
			var compare = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			if (TrueValues != null)
			{
				for (int i = 0; i < TrueValues.Length; i++)
				{
					if (string.Equals(text, TrueValues[i], compare))
						return true;
				}
			}
			if (FalseValues != null)
			{
				for (int i = 0; i < FalseValues.Length; i++)
				{
					if (string.Equals(text, FalseValues[i], compare))
						return false;
				}
			}
			throw new ArgumentOutOfRangeException("text", "Match value is invalid");
		}

		protected override int InnerParse(ParseArgs args)
		{
			if (TrueValues != null)
			{
				for (int i = 0; i < TrueValues.Length; i++)
				{
					var val = TrueValues[i];
					if (args.Scanner.ReadString(val, caseSensitive))
						return val.Length;
				}
			}
			if (FalseValues != null)
			{
				for (int i = 0; i < FalseValues.Length; i++)
				{
					var val = FalseValues[i];
					if (args.Scanner.ReadString(val, caseSensitive))
						return val.Length;
				}
			}
			return -1;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new BooleanTerminal(this, args);
		}
	}
}

