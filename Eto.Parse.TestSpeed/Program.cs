using System;
using System.IO;
using System.Diagnostics;
using Eto.Parse.Writers;
using Eto.Parse.Grammars;

namespace Eto.Parse.TestSpeed
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var json = new StreamReader(typeof(MainClass).Assembly.GetManifestResourceStream("Eto.Parse.TestSpeed.sample.json")).ReadToEnd();
			var iters = 100;
			Console.WriteLine("Json for {0} iterations:", iters);
			double etoSpeed = 0;

			/**/
			{
				var g = new Eto.Parse.Samples.JsonGrammar { EnableMatchEvents = false };
				var m = g.Match(json);
				var sw = new Stopwatch();
				sw.Start();
				for (int i = 0; i < iters; i++)
				{
					m = g.Match(json);
					if (!m.Success)
						Console.WriteLine("Error: {0}", m.ErrorMessage);
				}
				sw.Stop();
				Console.WriteLine("Eto.Parse: {0} seconds", etoSpeed = sw.Elapsed.TotalSeconds);
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			/**
			{
				var g = new Irony.Samples.Json.JsonGrammar();
				var p = new Irony.Parsing.Parser(g);
				var pt = p.Parse(json);
				var sw = new Stopwatch();
				sw.Start();
				for (int i = 0; i < iters; i++)
				{
					pt = p.Parse(json);
					if (pt.HasErrors())
					{
						foreach (var error in pt.ParserMessages)
							Console.WriteLine("Error: {0}", error);
						break;
					}
				}
				sw.Stop();
				Console.WriteLine("Irony: {0} seconds {1}", sw.Elapsed.TotalSeconds, FasterOrSlower(etoSpeed, sw.Elapsed.TotalSeconds));
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			/**
			{
				Newtonsoft.Json.Linq.JObject.Parse(json);
				var sw = new Stopwatch();
				sw.Start();
				for (int i = 0; i < iters; i++)
				{
					Newtonsoft.Json.Linq.JObject.Parse(json);
				}
				sw.Stop();
				Console.WriteLine("Newtonsoft.Json: {0} seconds {1}", sw.Elapsed.TotalSeconds, FasterOrSlower(etoSpeed, sw.Elapsed.TotalSeconds));
			}
			/**
			var test = new Eto.Parse.Tests.BnfTests();
			test.BnfToCode();
			/**
			var test = new Eto.Parse.Tests.GoldParserTests();
			test.ToCode();
			/**/
		}

		static string FasterOrSlower(double etoSpeed, double newSpeed)
		{
			if (etoSpeed > 0)
			{
				if (etoSpeed > newSpeed)
					return string.Format("({0:0.000}x Faster)", etoSpeed / newSpeed);
				else
					return string.Format("({0:0.000}x Slower)", newSpeed / etoSpeed);
			}
			else
				return string.Empty;
		}
	}
}
