using System;

namespace Eto.Parse.Testers
{
	public class CharSetTester : CharTester
	{
		char[] chars;
		
		public char[] CharSet
		{
			get { return chars; }
			set { chars = value; }
		}
		
		public CharSetTester(params char[] chars)
		{
			this.chars = (char[])chars.Clone();
		}
		
		public override bool Test(char ch)
		{
			for (int i = 0; i < chars.Length; i++)
			{
				if (chars[i] == ch) return true;
			}
			return false;
		}
	}
}
