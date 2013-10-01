using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class LetterTerminal : CharTerminal
	{
		protected LetterTerminal(LetterTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public LetterTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			return Char.IsLetter(ch);
		}

		protected override string CharName
		{
			get { return "Letter"; }
		}
		
		public override Parser Clone(ParserCloneArgs args)
		{
			return new LetterTerminal(this, args);
		}
	}
}
