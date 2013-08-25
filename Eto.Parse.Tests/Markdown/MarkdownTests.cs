using System;
using NUnit.Framework;
using Eto.Parse.Samples.Markdown;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;

namespace Eto.Parse.Tests.Markdown
{
	[TestFixture]
	public class MarkdownTests
	{
		public static readonly string BasePath = Path.Combine(Path.GetDirectoryName(typeof(MarkdownTests).Assembly.Location), "Markdown", "tests");
		MarkdownGrammar grammar = new MarkdownGrammar();

		[Test, TestCaseSource("TestFiles")]
		public void TestFile(string name)
		{
			TestFile(name, grammar.Transform);
		}

		public void TestFile(string name, Func<string, string> generate)
		{
			var text = File.ReadAllText(Path.Combine(BasePath, name + ".text"));
			var html = File.ReadAllText(Path.Combine(BasePath, name + ".html"));
			var generatedHtml = generate(text);
			Assert.AreEqual(html.Replace("\n", ""), generatedHtml.Replace("\n", "").Replace("\r", ""));
		}

		public static void CompareHtml(string html, string generatedHtml)
		{
			Assert.AreEqual(html.Replace("\n", ""), generatedHtml.Replace("\n", "").Replace("\r", ""));
		}

		public IEnumerable<string> AllTests
		{
			get
			{
				return GetTests("simple")
					.Concat(GetTests("mdtest-1-1"))
					.Concat(GetTests("mstest-0-1"))
					.Concat(GetTests("pandoc"))
					.Concat(GetTests("php-markdown"));
			}
		}

		public IEnumerable<string> GetTests(string path, string pattern = "*.text")
		{
			foreach (var file in Directory.GetFiles(Path.Combine(BasePath, path), pattern))
			{
				yield return Path.Combine(path, Path.GetFileNameWithoutExtension(file));
			}
		}
	}
}

