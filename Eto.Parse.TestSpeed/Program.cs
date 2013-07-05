using System;

namespace Eto.Parse.TestSpeed
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			/**
			var test = new Eto.Parse.Tests.BnfTests();
			test.AddressParsingSpeed();
			/**/
			var test = new Eto.Parse.Tests.BnfTests();
			test.BnfParsingSpeed();
			/**/
		}
	}
}
