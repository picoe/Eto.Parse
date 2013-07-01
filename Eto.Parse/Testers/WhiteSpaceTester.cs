using System;

namespace Eto.Parse.Testers
{
	public class WhiteSpaceTester : CharTester
	{
		
		public override bool Test(char ch)
		{
			return Char.IsWhiteSpace(ch);
		}
	}
}
