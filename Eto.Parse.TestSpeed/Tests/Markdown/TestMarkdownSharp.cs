#if BLAH
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Eto.Parse.Tests.Markdown;

namespace Eto.Parse.TestSpeed.Tests.Markdown
{
	public class TestMarkdownSharp : Benchmark<MarkdownSuite>
	{
		MarkdownSharp.Markdown markdown;

		public TestMarkdownSharp()
			: base("MarkdownSharp")
		{
		}

		public override void Warmup(MarkdownSuite suite)
		{
			var tests = suite.HtmlTests;
			markdown = new MarkdownSharp.Markdown();

			for (int testNum = 0; testNum < tests.Length; testNum++)
			{
				var test = tests[testNum];
				var generated = markdown.Transform(test.Text);
				MarkdownTests.CompareHtml(test.Html, generated);
			}
		}

		public override void PerformTest(MarkdownSuite suite, StringBuilder output)
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

#endif