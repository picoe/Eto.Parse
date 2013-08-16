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
			var sample = "Eto.Parse.TestSpeed.sample-large.json";
			var json = new StreamReader(typeof(MainClass).Assembly.GetManifestResourceStream(sample)).ReadToEnd();
			var iters = 100;
			Console.WriteLine("Json for {0} iterations:", iters);
			double? etoSpeed = null;
			double? etoWarmupSpeed = null;

			/**/
			{
				var warmupsw = new Stopwatch();
				warmupsw.Start();
				var g = new Eto.Parse.Samples.JsonGrammar { EnableMatchEvents = false };
				var m = g.Match(json);
				warmupsw.Stop();
				var sw = new Stopwatch();
				sw.Start();
				for (int i = 0; i < iters; i++)
				{
					m = g.Match(json);
					if (!m.Success)
					{
						Console.WriteLine("Error: {0}", m.ErrorMessage);
						break;
					}
				}
				sw.Stop();
				Console.WriteLine("Eto.Parse      : {0}, warmup: {1}", Speed((double)(etoSpeed = sw.Elapsed.TotalSeconds)), Speed((double)(etoWarmupSpeed = warmupsw.Elapsed.TotalSeconds)));
			}
			/**/
			{
				var warmupsw = new Stopwatch();
				var sw = new Stopwatch();
				try
				{
					warmupsw.Start();
					Newtonsoft.Json.Linq.JObject.Parse(json);
					warmupsw.Stop();
					sw.Start();
					for (int i = 0; i < iters; i++)
					{
						Newtonsoft.Json.Linq.JObject.Parse(json);
					}
				}
				catch (Newtonsoft.Json.JsonException ex)
				{
					warmupsw.Stop();
					Console.WriteLine("Error: {0}", ex);
				}
				sw.Stop();
				Console.WriteLine("Newtonsoft.Json: {0}, warmup: {1}", Speed(sw.Elapsed.TotalSeconds, etoSpeed), Speed(warmupsw.Elapsed.TotalSeconds, etoWarmupSpeed));
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			/**/
			{
				var warmupsw = new Stopwatch();
				warmupsw.Start();
				var g = new Irony.Samples.Json.JsonGrammar();
				var p = new Irony.Parsing.Parser(g);
				var pt = p.Parse(json);
				warmupsw.Stop();
				var sw = new Stopwatch();
				sw.Start();
				for (int i = 0; i < iters; i++)
				{
					pt = p.Parse(json);
					if (pt.HasErrors())
					{
						foreach (var error in pt.ParserMessages)
							Console.WriteLine("Error: {0}, Location: {1}", error, error.Location);
						break;
					}
				}
				sw.Stop();
				Console.WriteLine("Irony          : {0}, warmup: {1}", Speed(sw.Elapsed.TotalSeconds, etoSpeed), Speed(warmupsw.Elapsed.TotalSeconds, etoWarmupSpeed));
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			/**
			var test = new Eto.Parse.Tests.EbnfTests();
			test.EbnfToCode();
			/**
			var test = new Eto.Parse.Tests.BnfTests();
			test.Simple();
			/**
			var test = new Eto.Parse.Tests.GoldParserTests();
			test.ToCode();
			/**/
		}

		static string Speed(double speed, double? compareSpeed = null)
		{

			if (compareSpeed != null)
			{
				if (compareSpeed > speed)
					return string.Format("{0:0.00000}s ({1:0.000}x Faster)", speed, compareSpeed / speed);
				else
					return string.Format("{0:0.00000}s ({1:0.000}x Slower)", speed, speed / compareSpeed);
			}
			else
				return string.Format("{0:0.00000}s                ", speed);
		}
	}
}
