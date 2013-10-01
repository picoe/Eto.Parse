using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class LetterOrDigitTerminal : CharTerminal
	{
		protected LetterOrDigitTerminal(LetterOrDigitTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public LetterOrDigitTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			return Char.IsLetterOrDigit(ch);
		}

		protected override string CharName
		{
			get { return "Letter or Digit"; }
		}
		
		public override Parser Clone(ParserCloneArgs args)
		{
			return new LetterOrDigitTerminal(this, args);
		}
	}
}
