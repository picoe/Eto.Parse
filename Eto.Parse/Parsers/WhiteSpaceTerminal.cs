using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class WhiteSpaceTerminal : CharTerminal
	{
		protected WhiteSpaceTerminal(WhiteSpaceTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public WhiteSpaceTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			return Char.IsWhiteSpace(ch);
		}

		protected override string CharName
		{
			get { return "White Space"; }
		}
		
		public override Parser Clone(ParserCloneArgs args)
		{
			return new WhiteSpaceTerminal(this, args);
		}
	}
}
