using System;

namespace Eto.Parse.Testers
{
	public class LetterOrDigitTester : ICharTester
	{
		public bool Test(char ch, bool caseSensitive)
		{
			return Char.IsLetterOrDigit(ch);
		}

		public override string ToString()
		{
			return "Letter or Digit";
		}
	}
}
