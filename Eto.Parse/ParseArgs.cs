using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public class ParseArgs : EventArgs
	{
		List<ParseNode> nodes = new List<ParseNode>(100);
		Stack<NamedMatchCollection> reusableMatches = new Stack<NamedMatchCollection>();
		List<Parser> errors = new List<Parser>();

		public GrammarMatch Root { get; internal set; }

		public Scanner Scanner { get; private set; }

		public Grammar Grammar { get; private set; }

		public int ErrorIndex { get; private set; }

		public string Path
		{
			get
			{
				return string.Join(" > ", nodes.Select(r => r.Parser).OfType<NamedParser>().Select(r => r.Name));
			}
		}

		public List<Parser> Errors { get { return errors; } }

		public string ErrorName { get; private set; }

		public ParseArgs(Grammar grammar, Scanner scanner)
		{
			Grammar = grammar;
			Scanner = scanner;
		}

		public bool IsRoot
		{
			get { return nodes.Count <= 1; }
		}

		public ParseMatch EmptyMatch
		{
			get { return new ParseMatch(Scanner.Position, 0); }
		}

		public ParseMatch NoMatch
		{
			get { return new ParseMatch(-1, -1); }
		}

		NamedMatchCollection CreateTempMatchCollection()
		{
			if (reusableMatches.Count > 0)
				return reusableMatches.Pop();
			else
				return new NamedMatchCollection();
		}

		public void AddError(Parser parser)
		{
			var pos = Scanner.Position;
			if (pos > ErrorIndex)
			{
				ErrorIndex = pos;
				errors.Clear();
			}
			errors.Add(parser);
		}

		public void Push(Parser parser)
		{
			nodes.Insert(nodes.Count, new ParseNode(parser));
		}

		public NamedMatchCollection Pop(bool success)
		{
			var last = nodes[nodes.Count - 1];
			nodes.RemoveAt(nodes.Count - 1);

			if (!success)
			{
				if (last.Matches != null)
				{
					last.Matches.Clear();
					reusableMatches.Push(last.Matches);
				}
				return null;
			}
			else if (nodes.Count > 0)
			{
				var node = nodes[nodes.Count - 1];
				if (last.Matches != null)
				{
					if (node.Matches != null)
					{
						node.Matches.AddRange(last.Matches);
						last.Matches.Clear();
						reusableMatches.Push(last.Matches);
					}
					else
					{
						node.Matches = last.Matches;
						nodes[nodes.Count - 1] = node;
					}
				}
				return node.Matches;
			}
			return last.Matches;
		}

		public void PopMatch(NamedParser parser, ParseMatch match)
		{
			// allways successful here
			var last = nodes[nodes.Count - 1];
			nodes.RemoveAt(nodes.Count - 1);

			var node = nodes[nodes.Count - 1];
			if (node.Matches == null)
			{
				node.Matches = CreateTempMatchCollection();
				nodes[nodes.Count - 1] = node;
			}
			var namedMatch = new NamedMatch(parser, Scanner, match.Index, match.Length, last.Matches);
			node.Matches.Add(namedMatch);
		}
	}

	struct ParseNode
	{
		NamedMatchCollection matches;
		Parser parser;

		public NamedMatchCollection Matches { get { return matches; } set { matches = value; } }

		public Parser Parser { get { return parser; } }

		public ParseNode(Parser parser)
		{
			this.parser = parser;
			this.matches = null;
		}
	}
}
