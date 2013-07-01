using System;

namespace Eto.Parse.Testers
{
	public class DigitTester : CharTester
	{
		
		public override bool Test(char ch)
		{
			return Char.IsDigit(ch);
		}
	}
}
