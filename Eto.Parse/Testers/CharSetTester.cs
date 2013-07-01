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
			foreach (char c in chars)
			{
				if (c == ch) return true;
			}
			return false;
		}
	}
}
