using System;

namespace Eto.Parse.Testers
{
	public class PunctuationTester : ICharTester
	{
		public bool Test(char ch, bool caseSensitive)
		{
			return Char.IsPunctuation(ch);
		}

		public override string ToString()
		{
			return "Punctuation";
		}
	}
}
