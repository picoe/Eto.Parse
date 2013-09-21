using System;
using NUnit.Framework;
using Eto.Parse.Samples.Markdown;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Eto.Parse.Tests.Markdown
{
	[TestFixture]
	public class MarkdownTests
	{
		public static readonly string BasePath = Path.Combine(Path.GetDirectoryName(typeof(MarkdownTests).Assembly.Location), "Markdown", "tests");
		MarkdownGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp()
		{
			grammar = new MarkdownGrammar();
		}

		[Test, TestCaseSource("AllTests")]
		public void TestFile(string name)
		{
			TestFile(name, grammar.Transform);
		}

		public void TestFile(string name, Func<string, string> generate)
		{
			var fileName = Path.Combine(BasePath, name + ".text");
			if (!File.Exists(fileName))
				fileName = Path.Combine(BasePath, name + ".txt");
			var text = File.ReadAllText(fileName);
			var html = File.ReadAllText(Path.Combine(BasePath, name + ".html"));
			var generatedHtml = generate(text);
			CompareHtml(html, generatedHtml);
		}

		public static void CompareHtml(string html, string generatedHtml)
		{
			Assert.AreEqual(RemoveNewlines(html), RemoveNewlines(generatedHtml));
			/*if (html != generatedHtml)
				Assert.Inconclusive("Whitespace is different");*/
		}

		static string RemoveNewlines(string html)
		{
			// ignore multiple newlines
			html = Regex.Replace(html, "((?<=[>])[ ]+)?[\\n]+([ ]{0,3}(?=[<]))?", "\n", RegexOptions.Compiled);
			// ignore space between two tags
			html = Regex.Replace(html, "[>]\\s+[<]", "><", RegexOptions.Compiled);
			// ignore newline before ending tag
			html = html.Replace("\n</", "</");
			// ignore newline after start tag
			html = html.Replace(">\n", ">");
			// ignore tabs
			html = html.Replace("\t", "    ");
			return html.Trim();
		}

		public IEnumerable<string> AllTests
		{
			get
			{
				var tests = Enumerable.Empty<string>();
				tests = tests.Concat(GetTests("blocktests"));
				// tests = tests.Concat(GetTests("extramode")); // no extra mode yet
				tests = tests.Concat(GetTests("mdtest01"));
				tests = tests.Concat(GetTests("mdtest11"));
				tests = tests.Concat(GetTests("pandoc"));
				tests = tests.Concat(GetTests("phpmarkdown"));
				tests = tests.Concat(GetTests("safemode"));
				tests = tests.Concat(GetTests("simple"));
				tests = tests.Concat(GetTests("spantests"));
				tests = tests.Concat(GetTests("xsstests"));
				/*
				foreach (var dir in Directory.GetDirectories(BasePath))
				{
					tests = tests.Concat(GetTests(dir));
				}*/
				return tests;
			}
		}

		public IEnumerable<string> GetTests(string path, string pattern = "*.html")
		{
			foreach (var file in Directory.GetFiles(Path.Combine(BasePath, path), pattern))
			{
				yield return Path.Combine(path, Path.GetFileNameWithoutExtension(file));
			}
		}
	}
}

