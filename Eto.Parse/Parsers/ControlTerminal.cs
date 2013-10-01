using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class ControlTerminal : CharTerminal
	{
		protected ControlTerminal(ControlTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
		}

		public ControlTerminal()
		{
		}

		protected override bool Test(char ch)
		{
			return Char.IsControl(ch);
		}

		protected override string CharName
		{
			get { return "Control"; }
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new ControlTerminal(this, args);
		}
	}
}
