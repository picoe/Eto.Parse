using System;

namespace Eto.Parse.Testers
{
	public class ControlTester : ICharTester
	{
		public bool Test(char ch)
		{
			return Char.IsControl(ch);
		}
	}
}
