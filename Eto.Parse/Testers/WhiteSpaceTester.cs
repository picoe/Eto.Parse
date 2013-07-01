using System;

namespace Eto.Parse.Testers
{
	public class WhiteSpaceTester : ICharTester
	{
		public bool Test(char ch)
		{
			return Char.IsWhiteSpace(ch);
		}
	}
}
