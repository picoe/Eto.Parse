using System;

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
	}
}

