using System;

namespace Eto.Parse.Testers
{
	public class SymbolTester : ICharTester
	{
		public bool Test(char ch)
		{
			return Char.IsSymbol(ch);
		}
	}
}
