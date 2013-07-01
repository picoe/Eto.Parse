using System;

namespace Eto.Parse.Testers
{
	public class PunctuationTester : ICharTester
	{
		public bool Test(char ch)
		{
			return Char.IsPunctuation(ch);
		}
	}
}
