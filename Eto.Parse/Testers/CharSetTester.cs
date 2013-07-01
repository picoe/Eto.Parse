using System;

namespace Eto.Parse.Testers
{
	public class CharSetTester : CharTester
	{
		public char[] CharSet { get; set; }

		public CharSetTester(params char[] chars)
		{
			this.CharSet = (char[])chars.Clone();
		}
		
		public override bool Test(char ch)
		{
			for (int i = 0; i < CharSet.Length; i++)
			{
				if (CharSet[i] == ch) return true;
			}
			return false;
		}
	}
}
