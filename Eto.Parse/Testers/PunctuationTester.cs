using System;

namespace Eto.Parse.Testers
{
	public class PunctuationTester : CharTester
	{
		public override bool Test(char ch)
		{
			return Char.IsPunctuation(ch);
		}
	}
}
