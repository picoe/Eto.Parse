using System;
using Eto.Parse.Samples.Markdown;

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

