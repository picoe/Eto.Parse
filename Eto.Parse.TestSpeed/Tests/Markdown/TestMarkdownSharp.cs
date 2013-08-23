using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Eto.Parse.TestSpeed.Tests.Markdown
{
	public class TestMarkdownSharp : Test<MarkdownTestSuite>
	{
		MarkdownSharp.Markdown markdown;

		public TestMarkdownSharp()
			: base("MarkdownSharp")
		{
		}

		public override void Warmup(MarkdownTestSuite suite)
		{
			var tests = suite.HtmlTests;
			markdown = new MarkdownSharp.Markdown();

			for (int testNum = 0; testNum < tests.Length; testNum++)
			{
				var test = tests[testNum];
				markdown.Transform(test.Text);
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

