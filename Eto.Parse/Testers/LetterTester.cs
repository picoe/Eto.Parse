using System;

namespace Eto.Parse.Testers
{
	public class LetterTester : ICharTester
	{
		public bool Test(char ch, bool caseSensitive)
		{
			return Char.IsLetter(ch);
		}

		public override string ToString()
		{
			return "Letter";
		}
	}
}
