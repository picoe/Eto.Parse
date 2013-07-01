using System;

namespace Eto.Parse.Testers
{
	public class LetterTester : ICharTester
	{
		public bool Test(char ch)
		{
			return Char.IsLetter(ch);
		}
	}
}
