
//#define COMPARE_OUTPUT

using System;
using System.IO;
using System.Diagnostics;
using Eto.Parse.Writers;
using Eto.Parse.Grammars;
using System.Linq;
using System.Text;

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
			string etoResult;
			string compareProperty = "id";

			Console.WriteLine("Framework       | Parsing | Diff         | Warmup | Diff        ");
			Console.WriteLine("--------------- | ------- | ------------ | ------ | ------------");
			/**/
			{
				var sb = new StringBuilder();
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
					#if COMPARE_OUTPUT
					var result = m["object"].Find("property").First(r => r["name"].StringValue == "result")["array"];
					foreach (var item in result.Matches)
					{
						var id = item.Find("property").First(r => r["name"].StringValue == compareProperty)["number"].Int32Value;
						sb.Append(id);
					}
					#endif
				}
				sw.Stop();
				etoSpeed = sw.Elapsed.TotalSeconds;
				etoWarmupSpeed = warmupsw.Elapsed.TotalSeconds;
				Result("Eto.Parse", etoSpeed.Value, etoWarmupSpeed.Value);
				etoResult = sb.ToString();
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			/**/
			{
				var sb = new StringBuilder();
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
						Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(json);
						#if COMPARE_OUTPUT
						var result = obj["result"] as Newtonsoft.Json.Linq.JArray;
						foreach (var item in result)
						{
							var id = item[compareProperty];
							sb.Append(id);
						}
						#endif
					}
				}
				catch (Newtonsoft.Json.JsonException ex)
				{
					warmupsw.Stop();
					Console.WriteLine("Error: {0}", ex);
				}
				sw.Stop();
				if (sb.ToString() != etoResult)
					Console.WriteLine("ERROR: Parser output does not match!");
				Result("Newtonsoft.Json", sw.Elapsed.TotalSeconds, warmupsw.Elapsed.TotalSeconds, etoSpeed, etoWarmupSpeed); 
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			/**/
			{
				var sb = new StringBuilder();
				var warmupsw = new Stopwatch();
				warmupsw.Start();
				var g = new Irony.Samples.Json.IronyJsonGrammar();
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
					#if COMPARE_OUTPUT
					var results = pt.Root.ChildNodes.First(r => {
						return r.Term.Name == "Property" && r.ChildNodes.Any(s => s.Term.Name == "string" && s.Token.ValueString == "result");
					}).ChildNodes.First(r => r.Term.Name == "Array").ChildNodes;
					foreach (var item in results)
					{
						var id = item.ChildNodes.First(r => r.Term.Name == "Property" && r.ChildNodes.Any(s => s.Term.Name == "string" && s.Token.ValueString == compareProperty)).ChildNodes.First(r => r.Term.Name == "number").Token.Value;
						sb.Append(id);
					}
					#endif
				}
				sw.Stop();
				if (sb.ToString() != etoResult)
					Console.WriteLine("ERROR: Parser output does not match!");
				Result("Irony", sw.Elapsed.TotalSeconds, warmupsw.Elapsed.TotalSeconds, etoSpeed, etoWarmupSpeed); 
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			/**/
		}

		static void Result(string framework, double speed, double warmupSpeed, double? compareSpeed = null, double? compareWarmupSpeed = null)
		{
			Console.WriteLine("{0}|  {1} | {2}", framework.PadRight(16), 
			                  Speed(speed, compareSpeed),
			                  Speed(warmupSpeed, compareWarmupSpeed));

		}

		static string Speed(double speed, double? compareSpeed = null)
		{

			if (compareSpeed != null)
			{
				if (compareSpeed > speed)
					return string.Format("{0:0.000}s | {1:0.00}x Faster", speed, compareSpeed / speed);
				else
					return string.Format("{0:0.000}s | {1:0.00}x Slower", speed, speed / compareSpeed);
			}
			else
				return string.Format("{0:0.000}s | 1x          ", speed);
		}
	}
}
