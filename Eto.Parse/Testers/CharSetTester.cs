using System;

namespace Eto.Parse.Testers
{
	public class CharSetTester : ICharTester
	{
		public char[] Characters { get; set; }

		public CharSetTester(params char[] chars)
		{
			this.Characters = (char[])chars.Clone();
		}
		
		public bool Test(char ch)
		{
			for (int i = 0; i < Characters.Length; i++)
			{
				if (Characters[i] == ch) return true;
			}
			return false;
		}
	}
}
