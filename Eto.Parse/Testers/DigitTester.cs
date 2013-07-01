using System;

namespace Eto.Parse.Testers
{
	public class DigitTester : ICharTester
	{
		public bool Test(char ch)
		{
			return Char.IsDigit(ch);
		}
	}
}
