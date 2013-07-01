using System;

namespace Eto.Parse.Testers
{
	public class EolTester : ICharTester
	{
		public bool Test(char ch)
		{
			return ch == '\n' || ch == '\r';
		}
	}
}
