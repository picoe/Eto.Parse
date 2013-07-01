using System;

namespace Eto.Parse.Testers
{
	public class ExcludeTester : ICharTester
	{
		public ICharTester Include { get; set; }

		public ICharTester Exclude { get; set; }

		public ExcludeTester(ICharTester include, ICharTester exclude)
		{
			Include = include;
			Exclude = exclude;
		}

		public bool Test(char ch)
		{
			return Include.Test(ch) && !Exclude.Test(ch);
		}
	}
}

