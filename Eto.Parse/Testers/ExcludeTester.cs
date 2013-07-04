using System;

namespace Eto.Parse.Testers
{
	public class ExcludeTester : ICharTester
	{
		public ICharTester Include { get; set; }

		public bool IncludeInverse { get; set; }

		public ICharTester Exclude { get; set; }

		public bool ExcludeInverse { get; set; }

		public ExcludeTester()
		{
		}

		public ExcludeTester(ICharTester include, ICharTester exclude)
		{
			Include = include;
			Exclude = exclude;
		}

		public ExcludeTester(ICharTester include, bool includeInverse, ICharTester exclude, bool excludeInverse)
		{
			Include = include;
			IncludeInverse = includeInverse;
			Exclude = exclude;
			ExcludeInverse = excludeInverse;
		}

		public bool Test(char ch)
		{
			return Include.Test(ch) != IncludeInverse && Exclude.Test(ch) == ExcludeInverse;
		}
	}
}

