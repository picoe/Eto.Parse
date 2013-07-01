using System;

namespace Eto.Parse.Testers
{
	public class EolTester : CharTester
	{
		public override bool Test(char ch)
		{
			return ch == '\n' || ch == '\r';
		}
	}
}
