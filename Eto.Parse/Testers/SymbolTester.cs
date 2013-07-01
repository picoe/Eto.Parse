using System;

namespace Eto.Parse.Testers
{
	public class SymbolTester : CharTester
	{
		public override bool Test(char ch)
		{
			return Char.IsSymbol(ch);
		}
	}
}
