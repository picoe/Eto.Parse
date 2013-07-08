using System;
using System.IO;
using System.Diagnostics;
using Eto.Parse.Writers;

namespace Eto.Parse.TestSpeed
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var json = new StreamReader(typeof(MainClass).Assembly.GetManifestResourceStream("Eto.Parse.TestSpeed.sample.json")).ReadToEnd();
			var iters = 100;
			Console.WriteLine("Json for {0} iterations:", iters);

			/**/
			{
				var g = new Eto.Parse.Samples.JsonGrammar { EnableMatchEvents = false };
				var sw = new Stopwatch();
				sw.Start();
				for (int i = 0; i < iters; i++)
				{
					var m = g.Match(json);
					if (!m.Success)
						Console.WriteLine("Error: {0}", m.Error.LastError);
				}
				sw.Stop();
				Console.WriteLine("Eto.Parse: {0} seconds", sw.Elapsed.TotalSeconds);
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			/**/
			{
				var g = new Irony.Samples.Json.JsonGrammar();
				var p = new Irony.Parsing.Parser(g);
				var sw = new Stopwatch();
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
			/**/
			{
				Newtonsoft.Json.JsonConvert.DeserializeObject(json);
				var sw = new Stopwatch();
				sw.Start();
				for (int i = 0; i < iters; i++)
				{
					var m = Newtonsoft.Json.Linq.JObject.Parse(json);
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
