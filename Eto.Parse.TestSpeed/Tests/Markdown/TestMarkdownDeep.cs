using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Eto.Parse.Tests.Markdown;

namespace Eto.Parse.TestSpeed.Tests.Markdown
{
	public class TestMarkdownDeep : Test<MarkdownTestSuite>
	{
		MarkdownDeep.Markdown markdown;

		public TestMarkdownDeep()
			: base("MarkdownDeep")
		{
		}

		public override void Warmup(MarkdownTestSuite suite)
		{
			markdown = new MarkdownDeep.Markdown();
			var tests = suite.HtmlTests;
			for (int testNum = 0; testNum < tests.Length; testNum++)
			{
				var test = tests[testNum];
				var generated = markdown.Transform(test.Text);
				MarkdownTests.CompareHtml(test.Html, generated);
			}
		}

		public override void PerformTest(MarkdownTestSuite suite, StringBuilder output)
		{
			var tests = suite.HtmlTests;
			for (int testNum = 0; testNum < tests.Length; testNum++)
			{
				var test = tests[testNum];
				var generated = markdown.Transform(test.Text);
				if (suite.CompareOutput)
					output.Append(generated);
			}
		}
	}
}

