using System;

namespace Eto.Parse.Testers
{
	public class RangeTester : ICharTester
	{
		public char Start { get; set; }

		public char End { get; set; }

		public RangeTester()
		{
		}

		public RangeTester(char start, char end)
		{
			this.Start = start;
			this.End = end;
		}
		
		public bool Test(char ch, bool caseSensitive)
		{
			return ch >= Start && ch <= End;
		}
	}
}
