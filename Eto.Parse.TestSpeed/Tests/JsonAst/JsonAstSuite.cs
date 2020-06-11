using System;
using System.IO;
using BenchmarkDotNet.Attributes;

namespace Eto.Parse.TestSpeed.Tests.JsonAst
{

	public class JsonAstSmall : JsonAstSuite
	{
		public JsonAstSmall() : base("sample-small.json")
		{
		}
	}

	public class JsonAstLarge : JsonAstSuite
	{
		public JsonAstLarge() : base("sample-large.json")
		{
		}
	}

	public abstract class JsonAstSuite : BenchmarkSuite
	{
		string lastResult;
		protected JsonAstSuite(string sample)
		{
			sample = typeof(JsonAstSuite).Namespace + "." + sample;
			Json = new StreamReader(typeof(JsonAstSuite).Assembly.GetManifestResourceStream(sample)).ReadToEnd();
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
		public void Eto() => RunBenchmark<EtoAstBenchmark>();

		[Benchmark]
		public void NewtonsoftJson() => RunBenchmark<NewtonsoftJsonBenchmark>();
	}

}
