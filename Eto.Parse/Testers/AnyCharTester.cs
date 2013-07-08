using System;

namespace Eto.Parse.Testers
{
	public class AnyCharTester : ICharTester
	{
		public bool Test(char ch, bool caseSensitive)
		{
			return true;
		}
		
	}
}
