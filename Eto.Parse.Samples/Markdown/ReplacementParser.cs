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
		Dictionary<string, MarkdownReplacement> replacements;

		public MarkdownReplacement GetReplacement(string name)
		{
			return replacements[name];
		}

		public T GetReplacement<T>()
			where T: MarkdownReplacement
		{
			return replacements.Values.OfType<T>().FirstOrDefault();
		}

		public ReplacementParser(MarkdownGrammar grammar, IEnumerable<MarkdownReplacement> replacements)
		{
			this.replacements = new Dictionary<string, MarkdownReplacement>();

			foreach (var replacement in replacements)
			{
				this.replacements.Add(replacement.Name, replacement);
				this.Add(replacement.GetParser(grammar));
			}
		}
	}
	
}