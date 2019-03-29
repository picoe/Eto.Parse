using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace Eto.Parse.TestSpeed
{
    public class BenchmarkSuite : ISuite
    {
        IBenchmark benchmark;

        public T GetBenchmark<T>()
            where T : IBenchmark, new()
        {
            if (benchmark is T typedTest)
                return typedTest;
            typedTest = new T();
            benchmark = typedTest;
            return typedTest;
        }

        public void RunBenchmark<T>()
            where T : IBenchmark, new()
        {
			var t = GetBenchmark<T>();
			var result = t.Execute(this);
			if (!t.Verify(this, result))
				throw new InvalidOperationException($"Benchmark {typeof(T)} failed");
        }

		public virtual void VerifyAll()
		{
			foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
			{
				if (method.GetCustomAttribute<BenchmarkAttribute>() != null)
				{
					method.Invoke(this, null);
				}
			}
		}
	}

	public interface ISuite
	{
	}

	public interface IBenchmark
	{
		object Execute(ISuite suite);
		bool Verify(ISuite suite, object result);
	}

	public abstract class Benchmark<T, TResult> : IBenchmark
	{
		public abstract TResult Execute(T suite);

		public virtual bool Verify(T suite, TResult result) => true;

		object IBenchmark.Execute(ISuite suite) => Execute((T)suite);

		bool IBenchmark.Verify(ISuite suite, object result) => Verify((T)suite, (TResult)result);
	}

}
