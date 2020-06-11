using System;
using System.Collections.Generic;
using Eto.Parse.Samples.Markdown;
using Eto.Parse.Tests.Markdown;

namespace Eto.Parse.TestSpeed.Tests.Markdown
{
	public class EtoBenchmark : Benchmark<MarkdownSuite, IList<string>>
	{
		MarkdownGrammar markdown;

		public EtoBenchmark()
		{
			markdown = new MarkdownGrammar();
		}

		public override IList<string> Execute(MarkdownSuite suite)
		{
			var tests = suite.HtmlTests;
			var results = new List<string>();
			for (int testNum = 0; testNum < tests.Length; testNum++)
			{
				var test = tests[testNum];
				var generated = markdown.Transform(test.Text);
				results.Add(generated);
			}
			return results;
		}

		public override bool Verify(MarkdownSuite suite, IList<string> result)
		{
			return base.Verify(suite, result);
		}
	}
}

