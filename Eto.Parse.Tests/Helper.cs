using System;
using System.Linq;
using System.Diagnostics;
using System.CodeDom.Compiler;

namespace Eto.Parse.Tests
{
	public static class Helper
	{
		public static T Create<T>(string code, string className, params string[] referencedAssemblies)
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
				if (res.Errors.HasErrors)
				{
					var errors = string.Join("\n", res.Errors.OfType<CompilerError>().Select(r => r.ToString()));
					throw new Exception(string.Format("Error compiling:\n{0}", errors));
				}

				return (T)res.CompiledAssembly.CreateInstance(className);
			}
		}

		public static void TestSpeed(Grammar grammar, string input, int iterations)
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < iterations; i++)
			{
				grammar.Match(input);
			}
			sw.Stop();
			Console.WriteLine("{0} seconds for {1} iterations", sw.Elapsed.TotalSeconds, iterations);
		}
	}
}

