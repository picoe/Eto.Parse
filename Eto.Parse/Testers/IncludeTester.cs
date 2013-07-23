using System;

namespace Eto.Parse.Testers
{
	public class IncludeTester : ICharTester
	{
		public ICharTester First { get; set; }

		public bool FirstInverse { get; set; }

		public ICharTester Second { get; set; }

		public bool SecondInverse { get; set; }

		public IncludeTester()
		{
		}

		public IncludeTester(ICharTester first, bool firstInverse, ICharTester second, bool secondInverse)
		{
			First = first;
			FirstInverse = firstInverse;
			Second = second;
			SecondInverse = secondInverse;
		}

		public IncludeTester(ICharTester first, ICharTester second)
		{
			First = first;
			Second = second;
		}

		public bool Test(char ch, bool caseSensitive)
		{
			return First.Test(ch, caseSensitive) != FirstInverse
				|| Second.Test(ch, caseSensitive) != SecondInverse;
		}

		public override string ToString()
		{
			return string.Format("{1}{0}, {3}{2}", First, FirstInverse ? "!" : "", Second, SecondInverse ? "!" : "");
		}
	}
}

