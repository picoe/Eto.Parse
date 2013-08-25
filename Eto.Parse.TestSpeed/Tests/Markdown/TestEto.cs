using System;
using Eto.Parse.Samples.Markdown;
using Eto.Parse.Tests.Markdown;

namespace Eto.Parse.TestSpeed.Tests.Markdown
{
	public class TestEto : Test<MarkdownTestSuite>
	{
		MarkdownGrammar markdown;

		public TestEto()
			: base("Eto.Parse")
		{
		}

		public override void Warmup(MarkdownTestSuite suite)
		{
			var tests = suite.HtmlTests;
			markdown = new MarkdownGrammar();
			for (int testNum = 0; testNum < tests.Length; testNum++)
			{
				var test = tests[testNum];
				var generated = markdown.Transform(test.Text);
				MarkdownTests.CompareHtml(test.Html, generated);
			}
		}

		public override void PerformTest(MarkdownTestSuite suite, System.Text.StringBuilder output)
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

