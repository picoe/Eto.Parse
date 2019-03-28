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
		MarkdownDeep.Markdown deep;

        [SetUp]
		public void SetUp()
		{
			grammar = new MarkdownGrammar();
			deep = new MarkdownDeep.Markdown();
		}

		[Test, TestCaseSource("AllTests")]
		public void TestFile(string name)
		{
			//TestFile(name, deep.Transform);
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
			//Console.WriteLine(generatedHtml);
			CompareHtml(html, generatedHtml);
		}

		public static void CompareHtml(string html, string generatedHtml)
		{
			Assert.AreEqual(RemoveNewlines(html), RemoveNewlines(generatedHtml));
			/**
			var constraint = Is.EqualTo(generatedHtml);
			if (!constraint.Matches(html))
			{
				var writer = new TextMessageWriter("Whitespace is different");
				constraint.WriteMessageTo(writer);
				Assert.Inconclusive(writer.ToString());
			}
			/**/
		}

		static string RemoveNewlines(string html)
		{
			html = html.Replace("\r\n", "\n");
			html = html.Replace("\r", "\n");
			html = html.Replace("\n", " ");
			html = html.Replace("\t", " ");
			// ignore multiple newlines
			//html = Regex.Replace(html, "((?<=[>])[ ]+)?[\\n]+([ ]{0,3}(?=[<]))?", "\n", RegexOptions.Compiled);
			// ignore space between two tags
			html = Regex.Replace(html, "[>]\\s+[<]", "><", RegexOptions.Compiled);
			// ignore whitespace before beginning/ending tag
			html = Regex.Replace(html, "[\\n ]+<", "<", RegexOptions.Compiled);
			// ignore whitespace after start tag
			html = Regex.Replace(html, ">[\\n ]+", ">", RegexOptions.Compiled);
			// ignore space after newline
			html = Regex.Replace(html, "[\\n ]+", " ", RegexOptions.Compiled);
			// ignore tabs
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
				//tests = tests.Concat(GetTests("pandoc")); // other parsers don't pass these either
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
			var dir = Path.Combine(BasePath, path);
			if (Directory.Exists(dir))
			{
				foreach (var file in Directory.GetFiles(dir, pattern))
				{
					yield return Path.Combine(path, Path.GetFileNameWithoutExtension(file));
				}
			}
		}
	}
}

