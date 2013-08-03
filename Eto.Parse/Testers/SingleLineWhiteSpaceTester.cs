using System;

namespace Eto.Parse.Testers
{
	public class SingleLineWhiteSpaceTester : ICharTester
	{
		public bool Test(char ch, bool caseSensitive)
		{
			if (ch == '\n' || ch == '\r')
				return false;
			return Char.IsWhiteSpace(ch);
		}

		public override string ToString()
		{
			return "Single Line White Space";
		}
	}
}
