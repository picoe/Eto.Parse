using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class HexDigitTerminal : CharTerminal
	{
		protected HexDigitTerminal(HexDigitTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public HexDigitTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			return Char.IsDigit(ch)
				|| (ch >= 'A' && ch <= 'F')
				|| (ch >= 'a' && ch <= 'f');
		}

		protected override string CharName
		{
			get { return "Hex Digit"; }
		}
		
		public override Parser Clone(ParserCloneArgs args)
		{
			return new HexDigitTerminal(this, args);
		}
	}
}
