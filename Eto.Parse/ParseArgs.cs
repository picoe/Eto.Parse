using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public class ParseArgs : EventArgs
	{
		List<NamedMatchCollection> nodes = new List<NamedMatchCollection>(50);
		Stack<NamedMatchCollection> reusableMatches = new Stack<NamedMatchCollection>(10);
		List<Parser> errors = new List<Parser>();

		public GrammarMatch Root { get; internal set; }

		public Scanner Scanner { get; private set; }

		public Grammar Grammar { get; private set; }

		public int ErrorIndex { get; private set; }

		public bool CaseSensitive { get; private set; }

		public List<Parser> Errors { get { return errors; } }

		public string ErrorName { get; private set; }

		public ParseArgs(Grammar grammar, Scanner scanner)
		{
			Grammar = grammar;
			Scanner = scanner;
			CaseSensitive = grammar.CaseSensitive;
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
			nodes.Insert(nodes.Count, null);
		}

		public NamedMatchCollection Pop(bool success)
		{
			var index = nodes.Count - 1;
			var last = nodes[index];
			nodes.RemoveAt(index);

			if (!success)
			{
				if (last != null)
				{
					last.Clear();
					reusableMatches.Push(last);
				}
				return null;
			}
			else if (nodes.Count > 0)
			{
				index--;
				var node = nodes[index];
				if (last != null)
				{
					if (node != null)
					{
						node.AddRange(last);
						last.Clear();
						reusableMatches.Push(last);
					}
					else
					{
						node = last;
						nodes[index] = node;
					}
				}
				return node;
			}
			return last;
		}

		public void PopMatch(NamedParser parser, ParseMatch match)
		{
			// always successful here
			var index = nodes.Count - 1;
			var last = nodes[index];
			nodes.RemoveAt(index);

			index--;
			var node = nodes[index];
			if (node == null)
			{
				node = CreateTempMatchCollection();
				nodes[index] = node;
			}
			var namedMatch = new NamedMatch(parser, Scanner, match.Index, match.Length, last);
			node.Add(namedMatch);
		}
	}
}
