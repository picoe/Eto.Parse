using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class SingleLineWhiteSpaceTerminal : CharTerminal
	{
		protected SingleLineWhiteSpaceTerminal(SingleLineWhiteSpaceTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public SingleLineWhiteSpaceTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			if (ch == '\n' || ch == '\r')
				return false;
			return Char.IsWhiteSpace(ch);
		}

		protected override string CharName
		{
			get { return "Single Line White Space"; }
		}
		
		public override Parser Clone(ParserCloneArgs args)
		{
			return new SingleLineWhiteSpaceTerminal(this, args);
		}
	}
}
