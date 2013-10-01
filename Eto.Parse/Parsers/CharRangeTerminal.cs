using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Parsers
{
	public class CharRangeTerminal : CharTerminal
	{
		public char Start { get; set; }

		public char End { get; set; }

		protected CharRangeTerminal(CharRangeTerminal other, ParserCloneArgs args)
			: base(other, args)
		{
			this.Start = other.Start;
			this.End = other.End;
		}

		public CharRangeTerminal()
		{
		}

		public CharRangeTerminal(char start, char end)
		{
			this.Start = start;
			this.End = end;
		}

		protected override bool Test(char ch)
		{
			return ch >= Start && ch <= End;
		}

		protected override string CharName
		{
			get { return string.Format("0x{0:x2} to 0x{1:x2}", (int)Start, (int)End); }
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new CharRangeTerminal(this, args);
		}
	}
}
