using System;
using System.Collections.Generic;
using Eto.Parse.Parsers;
using System.Linq;

namespace Eto.Parse.Samples.Markdown
{
	public class ReplacementParser : AlternativeParser
	{
		readonly MarkdownGrammar grammar;
		readonly Dictionary<string, IMarkdownReplacement> replacements;

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
		protected override int InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
		#endif

		public ReplacementParser(MarkdownGrammar grammar)
		{
			this.grammar = grammar;
			this.replacements = new Dictionary<string, IMarkdownReplacement>();
		}

		public void Add(IEnumerable<IMarkdownReplacement> replacements, bool initReplacements = true)
		{
			foreach (var replacement in replacements)
			{
				this.replacements.Add(replacement.Name, replacement);
				if (initReplacements)
					replacement.Initialize(grammar);
				Add((Parser)replacement);
			}
		}
	}
	
}