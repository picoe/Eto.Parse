using System;

namespace Eto.Parse.Testers
{
	public class IncludeTester : ICharTester
	{
		public ICharTester First { get; set; }

		public ICharTester Second { get; set; }

		public IncludeTester(ICharTester first, ICharTester second)
		{
			First = first;
			Second = second;
		}

		public bool Test(char ch)
		{
			return First.Test(ch) && !Second.Test(ch);
		}
	}
}

