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

		public bool Test(char ch, bool caseSensitive)
		{
			return Include.Test(ch, caseSensitive) != IncludeInverse
				&& Exclude.Test(ch, caseSensitive) == ExcludeInverse;
		}

		public override string ToString()
		{
			return string.Format("{1}{0}, Excluding: {3}{2}", Include, IncludeInverse ? "!" : "", Exclude, ExcludeInverse ? "!" : "");
		}
	}
}

