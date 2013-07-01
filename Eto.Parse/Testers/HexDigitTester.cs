using System;

namespace Eto.Parse.Testers
{
	public class HexDigitTester : ICharTester
	{
		public bool Test(char ch)
		{
			return Char.IsDigit(ch)
				|| (ch >= 'A' && ch <= 'F')
				|| (ch >= 'a' && ch <= 'f');
		}
	}
}
