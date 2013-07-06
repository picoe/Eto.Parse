using System;
using System.IO;
using System.Diagnostics;

namespace Eto.Parse.TestSpeed
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var json = new StreamReader(typeof(MainClass).Assembly.GetManifestResourceStream("Eto.Parse.TestSpeed.sample.json")).ReadToEnd();
			var sw = new Stopwatch();
			var iters = 100;
			/**
			Console.WriteLine("Json for {0} iterations:", iters);
			{
				var g = new Irony.Samples.Json.JsonGrammar();
				var p = new Irony.Parsing.Parser(g);
				sw.Start();
				for (int i = 0; i < iters; i++)
				{
					var pt = p.Parse(json);
					if (pt.HasErrors())
					{
						foreach (var error in pt.ParserMessages)
							Console.WriteLine("Error: {0}", error);
						break;
					}
				}
				sw.Stop();
				Console.WriteLine("Irony: {0} seconds", sw.Elapsed.TotalSeconds);
			}

			/**/
			{
				var g = new Eto.Parse.Samples.JsonGrammar();
				sw = new Stopwatch();
				sw.Start();
				for (int i = 0; i < iters; i++)
				{
					var m = g.Match(json, false);
					if (!m.Success)
						Console.WriteLine("Error: {0}", m.Error);
				}
				sw.Stop();
				Console.WriteLine("Eto.Parse: {0} seconds", sw.Elapsed.TotalSeconds);
			}
			/**
			var test = new Eto.Parse.Tests.BnfTests();
			test.AddressParsingSpeed();
			/**
			var test = new Eto.Parse.Tests.BnfTests();
			test.BnfParsingSpeed();
			/**/
		}
	}
}
