using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class PunctuationTerminal : CharTerminal
	{
		protected PunctuationTerminal(PunctuationTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public PunctuationTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			return Char.IsPunctuation(ch);
		}

		protected override string CharName
		{
			get { return "Punctuation"; }
		}
		
		public override Parser Clone(ParserCloneArgs args)
		{
			return new PunctuationTerminal(this, args);
		}
	}
}
