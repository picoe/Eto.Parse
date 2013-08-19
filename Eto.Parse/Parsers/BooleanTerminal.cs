using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			caseSensitive = this.CaseSensitive ?? args.Grammar.CaseSensitive;
		}

		public override object GetValue(Match match)
		{
			var val = match.Text;
			var compare = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			if (TrueValues != null)
			{
				for (int i = 0; i < TrueValues.Length; i++)
				{
					if (string.Equals(val, TrueValues[i], compare))
						return true;
				}
			}
			if (FalseValues != null)
			{
				for (int i = 0; i < FalseValues.Length; i++)
				{
					if (string.Equals(val, FalseValues[i], compare))
						return false;
				}
			}
			throw new ArgumentOutOfRangeException("Match value is invalid");
		}

		public override IEnumerable<Parser> Children(ParserChain args)
		{
			yield break;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var pos = args.Scanner.Position;
			if (TrueValues != null)
			{
				for (int i = 0; i < TrueValues.Length; i++)
				{
					var val = TrueValues[i];
					if (args.Scanner.ReadString(val, caseSensitive))
						return new ParseMatch(pos, val.Length);
				}
			}
			if (FalseValues != null)
			{
				for (int i = 0; i < FalseValues.Length; i++)
				{
					var val = FalseValues[i];
					if (args.Scanner.ReadString(val, caseSensitive))
						return new ParseMatch(pos, val.Length);
				}
			}
			return args.NoMatch;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new BooleanTerminal(this, args);
		}
	}
}

