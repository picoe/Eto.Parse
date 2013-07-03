using System;

namespace Eto.Parse.Testers
{
	public class IncludeTester : ICharTester
	{
		public ICharTester First { get; set; }

		public bool FirstNegative { get; set; }

		public ICharTester Second { get; set; }

		public bool SecondNegative { get; set; }

		public IncludeTester()
		{
		}

		public IncludeTester(ICharTester first, bool firstNegative, ICharTester second, bool secondNegative)
		{
			First = first;
			FirstNegative = firstNegative;
			Second = second;
			SecondNegative = secondNegative;
		}

		public IncludeTester(ICharTester first, ICharTester second)
		{
			First = first;
			Second = second;
		}

		public bool Test(char ch)
		{
			return First.Test(ch) != FirstNegative || Second.Test(ch) != SecondNegative;
		}
	}
}

