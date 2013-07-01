using System;
using System.Diagnostics;

namespace Eto.Parse.Tests
{
	public static class Helper
	{
		public static T Execute<T>(string code, string className, string methodName, params string[] referencedAssemblies)
		{
			using (var csharp = new Microsoft.CSharp.CSharpCodeProvider())
			{
				var parameters = new System.CodeDom.Compiler.CompilerParameters()
				{  
					GenerateInMemory = true,
				};
				if (referencedAssemblies != null)
					parameters.ReferencedAssemblies.AddRange(referencedAssemblies);

				var res = csharp.CompileAssemblyFromSource(parameters, code);

				var type = res.CompiledAssembly.GetType(className);
				var method = type.GetMethod(methodName);
				return (T)method.Invoke(null, null);
			}
		}

		public static void TestSpeed(Parser parser, string input, int iterations)
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < iterations; i++)
			{
				parser.Match(input);
			}
			sw.Stop();
			Console.WriteLine("{0} seconds for {1} iterations", sw.Elapsed.TotalSeconds, iterations);
		}
	}
}

