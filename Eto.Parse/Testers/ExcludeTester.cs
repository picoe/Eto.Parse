using System;

namespace Eto.Parse.Testers
{
	public class ExcludeTester : CharTester
	{
		public CharTester Include { get; set; }

		public CharTester Exclude { get; set; }

		public ExcludeTester(CharTester include, CharTester exclude)
		{
			Include = include;
			Exclude = exclude;
		}

		public override bool Test(char ch)
		{
			return Include.Test(ch) && !Exclude.Test(ch);
		}
	}
}

