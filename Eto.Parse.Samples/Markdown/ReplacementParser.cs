using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse.Samples.Markdown
{
	public class ReplacementParser : AlternativeParser
	{
		Dictionary<string, IMarkdownReplacement> replacements;

		public IMarkdownReplacement GetReplacement(string name)
		{
			return replacements[name];
		}

		public T GetReplacement<T>()
			where T: IMarkdownReplacement
		{
			return replacements.Values.OfType<T>().FirstOrDefault();
		}

		#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public ReplacementParser(MarkdownGrammar grammar, IEnumerable<IMarkdownReplacement> replacements)
		{
			this.replacements = new Dictionary<string, IMarkdownReplacement>();

			foreach (var replacement in replacements)
			{
				this.replacements.Add(replacement.Name, replacement);
				replacement.Initialize(grammar);
				this.Add((Parser)replacement);
			}
		}
	}
	
}