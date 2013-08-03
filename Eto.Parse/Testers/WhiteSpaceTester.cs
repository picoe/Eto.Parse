using System;

namespace Eto.Parse.Testers
{
	public class WhiteSpaceTester : ICharTester
	{
		public bool Test(char ch, bool caseSensitive)
		{
			return Char.IsWhiteSpace(ch);
		}

		public override string ToString()
		{
			return "White Space";
		}
	}
}
