using System;
using System.Collections.Generic;
using Eto.Parse.Tests.Markdown;
using System.Linq;
using System.IO;

namespace Eto.Parse.TestSpeed.Tests.Markdown
{
	public class MarkdownSuite : BenchmarkSuite
	{
		HtmlTest[] htmlTests;

		public struct HtmlTest
		{
			public string Name { get; set; }

			public string Text { get; set; }

			public string Html { get; set; }
		}

		public HtmlTest[] HtmlTests
		{
			get
			{
				if (htmlTests == null)
				{
					var testList = new List<HtmlTest>();

					var tests = Enumerable.Empty<string>();
					//tests = tests.Concat(MarkdownTests.GetTests("blocktests"));
					//tests = tests.Concat(MarkdownTests.GetTests("extramode")); // no extra mode yet
					//tests = tests.Concat(MarkdownTests.GetTests("mdtest01"));
					//tests = tests.Concat(MarkdownTests.GetTests("mdtest11"));
					//tests = tests.Concat(MarkdownTests.GetTests("pandoc"));
					//tests = tests.Concat(MarkdownTests.GetTests("phpmarkdown"));
					//tests = tests.Concat(MarkdownTests.GetTests("safemode"));
					tests = tests.Concat(MarkdownTests.GetTests("simple"));
					//tests = tests.Concat(MarkdownTests.GetTests("spantests"));
					//tests = tests.Concat(MarkdownTests.GetTests("xsstests"));
					foreach (var name in tests)
					{
						var textName = Path.Combine(MarkdownTests.BasePath, name + ".text");
						if (!File.Exists(textName))
							textName = Path.Combine(MarkdownTests.BasePath, name + ".txt");
						var text = File.ReadAllText(textName);
						var html = File.ReadAllText(Path.Combine(MarkdownTests.BasePath, name + ".html"));
						testList.Add(new HtmlTest { Name = name, Text = text, Html = html });
					}
					htmlTests = testList.ToArray();
				}

				return htmlTests;
			}
		}

	}
}

