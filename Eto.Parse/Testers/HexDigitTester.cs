using System;

namespace Eto.Parse.Testers
{
	public class HexDigitTester : CharTester
	{
		public override bool Test(char ch)
		{
			return Char.IsDigit(ch)
				|| (ch >= 'A' && ch <= 'F')
				|| (ch >= 'a' && ch <= 'f');
		}
	}
}
