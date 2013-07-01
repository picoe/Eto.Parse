using System;

namespace Eto.Parse.Testers
{
	public class LetterOrDigitTester : ICharTester
	{
		public bool Test(char ch)
		{
			return Char.IsLetterOrDigit(ch);
		}
	}
}
