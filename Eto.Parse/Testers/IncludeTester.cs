using System;

namespace Eto.Parse.Testers
{
	public class IncludeTester : CharTester
	{
		public CharTester First { get; set; }

		public CharTester Second { get; set; }

		public IncludeTester(CharTester include, CharTester exclude)
		{
			First = include;
			Second = exclude;
		}

		public override bool Test(char ch)
		{
			return First.Test(ch) && !Second.Test(ch);
		}
	}
}

