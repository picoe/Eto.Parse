using System;

namespace Eto.Parse.Testers
{
	public class ControlTester : ICharTester
	{
		public bool Test(char ch, bool caseSensitive)
		{
			return Char.IsControl(ch);
		}
	}
}
