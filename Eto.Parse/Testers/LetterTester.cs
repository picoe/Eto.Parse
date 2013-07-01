using System;

namespace Eto.Parse.Testers
{
	public class LetterTester : CharTester
	{
		public override bool Test(char ch)
		{
			return Char.IsLetter(ch);
		}
	}
}
