using System;

namespace Eto.Parse.Testers
{
	public class ExcludeTester : ICharTester
	{
		public ICharTester Include { get; set; }

		public bool IncludeNegative { get; set; }

		public ICharTester Exclude { get; set; }

		public bool ExcludeNegative { get; set; }

		public ExcludeTester()
		{
		}

		public ExcludeTester(ICharTester include, ICharTester exclude)
		{
			Include = include;
			Exclude = exclude;
		}

		public ExcludeTester(ICharTester include, bool includeNegative, ICharTester exclude, bool excludeNegative)
		{
			Include = include;
			IncludeNegative = includeNegative;
			Exclude = exclude;
			ExcludeNegative = excludeNegative;
		}

		public bool Test(char ch)
		{
			return Include.Test(ch) != IncludeNegative && Exclude.Test(ch) == ExcludeNegative;
		}
	}
}

