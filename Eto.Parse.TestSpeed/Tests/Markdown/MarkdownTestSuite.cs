using System;
using System.Collections.Generic;
using Eto.Parse.Tests.Markdown;
using System.Linq;
using System.IO;

namespace Eto.Parse.TestSpeed.Tests.Markdown
{
	public class MarkdownTestSuite : TestSuite
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
					var markdownTests = new MarkdownTests();
					var testList = new List<HtmlTest>();

					var tests = Enumerable.Empty<string>();
					//tests = tests.Concat(markdownTests.GetTests("blocktests"));
					//tests = tests.Concat(markdownTests.GetTests("extramode")); // no extra mode yet
					//tests = tests.Concat(markdownTests.GetTests("mdtest01"));
					//tests = tests.Concat(markdownTests.GetTests("mdtest11"));
					//tests = tests.Concat(markdownTests.GetTests("pandoc"));
					//tests = tests.Concat(markdownTests.GetTests("phpmarkdown"));
					//tests = tests.Concat(markdownTests.GetTests("safemode"));
					tests = tests.Concat(markdownTests.GetTests("simple"));
					//tests = tests.Concat(markdownTests.GetTests("spantests"));
					//tests = tests.Concat(markdownTests.GetTests("xsstests"));
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

		public MarkdownTestSuite()
			: base("Markdown")
		{
			Iterations = 200;
		}

		public override IEnumerable<ITest> GetTests()
		{
			yield return new TestEto();
			yield return new TestMarkdownDeep();
			yield return new TestMarkdownSharp();
		}
	}
}

