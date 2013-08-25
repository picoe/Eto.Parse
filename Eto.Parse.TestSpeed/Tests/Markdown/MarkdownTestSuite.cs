using System;
using System.Collections.Generic;
using Eto.Parse.Tests.Markdown;
using System.Linq;
using System.IO;

namespace Eto.Parse.TestSpeed.Tests.Markdown
{
	public class MarkdownTestSuite : TestSuite
	{
		HtmlTest[] tests;

		public struct HtmlTest
		{
			public string Text { get; set; }

			public string Html { get; set; }
		}

		public HtmlTest[] HtmlTests
		{
			get
			{
				if (tests == null)
				{
					var markdownTests = new MarkdownTests();
					var testList = new List<HtmlTest>();
					var testNames = markdownTests.GetTests("simple").ToArray();
					foreach (var test in testNames)
					{
						var text = File.ReadAllText(Path.Combine(MarkdownTests.BasePath, test + ".text"));
						var html = File.ReadAllText(Path.Combine(MarkdownTests.BasePath, test + ".html"));
						testList.Add(new HtmlTest { Text = text, Html = html });
					}
					tests = testList.ToArray();
				}

				return tests;
			}
		}

		public MarkdownTestSuite()
			: base("Markdown")
		{
			Iterations = 1000;
		}

		public override IEnumerable<ITest> GetTests()
		{
			yield return new TestEto();
			yield return new TestMarkdownDeep();
			yield return new TestMarkdownSharp();
		}
	}
}

