using System;

namespace Eto.Parse.Testers
{
	public class DigitTester : ICharTester
	{
		public bool Test(char ch, bool caseSensitive)
		{
			return Char.IsDigit(ch);
		}
	}
}
