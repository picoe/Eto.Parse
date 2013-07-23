using System;

namespace Eto.Parse.Testers
{
	public class HexDigitTester : ICharTester
	{
		public bool Test(char ch, bool caseSensitive)
		{
			return Char.IsDigit(ch)
				|| (ch >= 'A' && ch <= 'F')
				|| (ch >= 'a' && ch <= 'f');
		}

		public override string ToString()
		{
			return "Hex Digit";
		}
	}
}
