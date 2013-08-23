using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown
{
	public struct MarkdownReference
	{
		public string Url { get; set; }
		public string Title { get; set; }
	}

	public class MarkdownReplacementArgs
	{
		Dictionary<string, MarkdownReference> references = new Dictionary<string, MarkdownReference>();
		public Dictionary<string, MarkdownReference> References { get { return references; } }
		public StringBuilder Output { get; set; }
		public MarkdownEncoding Encoding { get; set; }
	}

	public abstract class MarkdownReplacement
	{
		public bool AddLinesBefore { get; protected set; }

		public MarkdownReplacement()
		{
			AddLinesBefore = true;
		}

		public abstract string Name { get; }

		public abstract Parser GetParser(MarkdownGrammar grammar);

		public abstract void Replace(Match match, MarkdownReplacementArgs args);
	}
	
}