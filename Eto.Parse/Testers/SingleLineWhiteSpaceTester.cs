using System;

namespace Eto.Parse.Testers
{
	public class SingleLineWhiteSpaceTester : CharTester
	{
		
		public override bool Test(char ch)
		{
			if (ch == '\n' || ch == '\r')
				return false;
			return Char.IsWhiteSpace(ch);
		}
	}
}
