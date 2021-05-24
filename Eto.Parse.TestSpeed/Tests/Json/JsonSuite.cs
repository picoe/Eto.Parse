using System;
using System.IO;
using BenchmarkDotNet.Attributes;

namespace Eto.Parse.TestSpeed.Tests.Json
{

	public class JsonSmall : JsonSuite
	{
		public JsonSmall() : base("sample-small.json")
		{
		}
	}

	public class JsonLarge : JsonSuite
	{
		public JsonLarge() : base("sample-large.json")
		{
		}
	}

	public abstract class JsonSuite : BenchmarkSuite
	{
		string lastResult;
		protected JsonSuite(string sample)
		{
			sample = typeof(JsonSuite).Namespace + "." + sample;
			Json = new StreamReader(typeof(JsonSuite).Assembly.GetManifestResourceStream(sample)).ReadToEnd();
		}

		public string Json { get; }

		public bool CompareOutput { get; set; }

		public string CompareProperty { get; set; } = "id";

		public void Compare(string output)
		{
			if (lastResult == null)
				lastResult = output;
			else if (output != lastResult)
				throw new InvalidOperationException("Strings don't match");
		}

		public override void VerifyAll()
		{
			CompareOutput = true;
			base.VerifyAll();
		}

		[Benchmark]
		public void EtoDirect() => RunBenchmark<EtoDirectBenchmark>();

		[Benchmark]
		public void EtoHelpers() => RunBenchmark<EtoHelpersBenchmark>();

		[Benchmark]
		public void EtoAst() => RunBenchmark<EtoAstBenchmark>();

		[Benchmark]
		public void NewtonsoftJson() => RunBenchmark<NewtonsoftJsonBenchmark>();

		[Benchmark]
		public void Irony() => RunBenchmark<IronyBenchmark>();

		[Benchmark]
		public void ServiceStack() => RunBenchmark<ServiceStackBenchmark>();

#if !NETCOREAPP
		[Benchmark]
		public void Sprache() => RunBenchmark<SpracheBenchmark>();
#endif
	}

}
