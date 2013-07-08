using System;

namespace Eto.Parse.Testers
{
	public class EolTester : ICharTester
	{
		public bool Test(char ch, bool caseSensitive)
		{
			return ch == '\n' || ch == '\r';
		}
	}
}
