using System;

namespace Eto.Parse.Testers
{
	public class ControlTester : CharTester
	{
		public override bool Test(char ch)
		{
			return Char.IsControl(ch);
		}
	}
}
