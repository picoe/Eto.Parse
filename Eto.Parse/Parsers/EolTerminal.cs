using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class EolTerminal : CharTerminal
	{
		protected EolTerminal(EolTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public EolTerminal()
		{
		}

		protected override bool Test(char ch, bool caseSensitive)
		{
			return ch == '\n' || ch == '\r';
		}

		protected override string CharName
		{
			get { return "EOL"; }
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new EolTerminal(this, args);
		}
	}
}
