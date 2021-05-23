using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Collections.Generic;

namespace Eto.Parse.Tests
{
	public static class Helper
	{
		static IEnumerable<AssemblyName> GetReferences(Assembly assembly)
		{
			yield return assembly.GetName();
			
			foreach (var reference in assembly.GetReferencedAssemblies())
			{
				yield return reference;
			}
			
#if NETCOREAPP
			yield return new AssemblyName("System.Private.CoreLib");
			yield return new AssemblyName("netstandard");
			yield return new AssemblyName("System.Core");
			yield return new AssemblyName("System.Runtime");
			yield return new AssemblyName("System.Collections");
#endif
		}
		
		public static T Create<T>(string code, string className)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(code);

			var references = GetReferences(typeof(T).Assembly);
			
			var refs = references.Distinct().Select(r => MetadataReference.CreateFromFile(Assembly.Load(r).Location));

			CSharpCompilation compilation = CSharpCompilation.Create(
				"assemblyName",
				new[] { syntaxTree },
				refs,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			using (var dllStream = new MemoryStream())
			{
				var result = compilation.Emit(dllStream);
				if (!result.Success)
				{
					var errors = string.Join("\n", result.Diagnostics.Select(r => r.GetMessage()));
					throw new InvalidOperationException(string.Format("Error compiling:\n{0}", errors));
				}
				
				var assembly = Assembly.Load(dllStream.ToArray());
				return (T)assembly.CreateInstance(className);
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

