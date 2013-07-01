using System;

namespace Eto.Parse.Testers
{
	public class IncludeTester : CharTester
	{
		public CharTester First { get; set; }

		public CharTester Second { get; set; }

		public IncludeTester(CharTester first, CharTester second)
		{
			First = first;
			Second = second;
		}

		public override bool Test(char ch)
		{
			return First.Test(ch) && !Second.Test(ch);
		}
	}
}

