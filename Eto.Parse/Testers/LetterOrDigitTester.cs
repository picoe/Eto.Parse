using System;

namespace Eto.Parse.Testers
{
	public class LetterOrDigitTester : CharTester
	{
		public override bool Test(char ch)
		{
			return Char.IsLetterOrDigit(ch);
		}
	}
}
