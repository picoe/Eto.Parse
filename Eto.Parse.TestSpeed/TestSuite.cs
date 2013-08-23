using System;
using System.IO;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Eto.Parse.TestSpeed
{
	public abstract class TestSuite
	{
		public bool WriteInitialResults { get; set; }

		public string Name { get; private set; }

		public int Iterations { get; set; }

		public bool CompareOutput { get; set; }

		public abstract IEnumerable<ITest> GetTests();

		public TestSuite(string name)
		{
			this.Name = name;
			this.WriteInitialResults = true;
		}

		public void Run()
		{
			Console.WriteLine();
			Console.WriteLine("Test Suite: {0} - {1} iterations", this.Name, this.Iterations);
			if (this.CompareOutput)
			{
				Console.WriteLine("\tcomparing output");
			}

			if (WriteInitialResults)
			{
				Console.WriteLine();
				Console.WriteLine("Test             | Parsing | Warmup ");
				Console.WriteLine("---------------- | ------- | -------");
			}

			var results = new List<TestResult>();
			TestResult compare = null;
			foreach (var result in PerformTests())
			{
				if (WriteInitialResults)
					Console.WriteLine("{0,-17}| {1,6:0.000}s | {2,6:0.000}s", result.Test.Name, result.Speed, result.WarmupSpeed);
				if (compare != null && result.CompareResult != compare.CompareResult)
				{
					Console.WriteLine("ERROR: Output does not match!");
				}
				results.Add(result);
				compare = result;
			}

			Console.WriteLine();
			Console.WriteLine("Comparison:");
			Console.WriteLine();
			Console.WriteLine("Test             | Parsing | Slower than best |  Warmup | Slower than best");
			Console.WriteLine("---------------- | ------: | :--------------: | ------: | :--------------:");

			var minSpeed = results.Min(r => r.Speed);
			var minWarmup = results.Min(r => r.WarmupSpeed);
			foreach (var result in results)
			{
				Console.WriteLine("{0,-17}| {1,6:0.000}s | {2,8:0.00}x        | {3,6:0.000}s | {4,8:0.00}x", result.Test.Name, 
				                  result.Speed, result.Speed / minSpeed,
				                  result.WarmupSpeed, result.WarmupSpeed / minWarmup);
			}
		}

		public IEnumerable<TestResult> PerformTests()
		{
			var tests = GetTests().ToArray();
			for (int testNum = 0; testNum < tests.Length; testNum++)
			{
				var test = tests[testNum];
				var warmupsw = new Stopwatch();
				warmupsw.Start();
				test.Warmup(this);
				warmupsw.Stop();

				var output = this.CompareOutput ? new StringBuilder(): null;
				var speed = new Stopwatch();
				speed.Start();
				for (int i = 0; i < Iterations; i++)
				{
					test.PerformTest(this, output);
				}
				speed.Stop();
				yield return new TestResult
				{
					Test = test,
					Speed = speed.Elapsed.TotalSeconds,
					WarmupSpeed = warmupsw.Elapsed.TotalSeconds,
					CompareResult = output != null ? output.ToString() : null
				};
			}
		}

	}

	public interface ITest
	{
		string Name { get; }
		void PerformTest(TestSuite suite, StringBuilder output);
		void Warmup(TestSuite suite);
	}

	public abstract class Test : Test<TestSuite>
	{
		public Test(string name) : base(name)
		{
		}
	}

	public abstract class Test<T> : ITest
		where T: TestSuite
	{
		public string Name { get; private set; }

		public Test(string name)
		{
			this.Name = name;
		}

		public abstract void PerformTest(T suite, StringBuilder output);
		public virtual void Warmup(T suite)
		{
		}

		void ITest.PerformTest(TestSuite suite, StringBuilder output)
		{
			PerformTest((T)suite, output);
		}

		void ITest.Warmup(TestSuite suite)
		{
			Warmup((T)suite);
		}
	}

	public class TestResult
	{
		public ITest Test { get; set; }

		public double Speed { get; set; }

		public double WarmupSpeed { get; set; }

		public string CompareResult { get; set; }
	}

}
