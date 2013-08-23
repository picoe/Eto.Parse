using System;
using System.IO;
using System.Diagnostics;
using Eto.Parse.Samples.Json;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Eto.Parse.Tests.Markdown;
using Eto.Parse.Samples.Markdown;

namespace Eto.Parse.TestSpeed
{
	static class MainClass
	{
		static IEnumerable<TestSuite> TestSuites()
		{
			yield return new Tests.Json.JsonTestSuite("(large file)", "sample-large.json");

			yield return new Tests.Json.JsonTestSuite("(small file)", "sample-small.json");

			yield return new Tests.Markdown.MarkdownTestSuite();
		}

		public static void Main(string[] args)
		{
			foreach (var suite in TestSuites())
			{
				suite.Run();
			}
		}
	}
}
