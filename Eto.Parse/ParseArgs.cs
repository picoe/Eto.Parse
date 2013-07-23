using System;
using System.Collections.Generic;

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

		public NamedMatchCollection Push(Parser parser)
		{
			var matches = CreateTempMatchCollection();
			nodes.Insert(nodes.Count, new ParseNode(parser, Scanner.Position, matches));
			return matches;
		}

		public void Pop(bool success, bool clear = true)
		{
			var last = nodes[nodes.Count - 1];
			nodes.RemoveAt(nodes.Count - 1);

			if (!success)
				Scanner.SetPosition(last.Position);
			else if (nodes.Count > 0)
				nodes[nodes.Count - 1].Matches.AddRange(last.Matches);

			if (clear)
			{
				last.Matches.Clear();
				reusableMatches.Push(last.Matches);
			}
		}

		public void Pop(NamedMatch match, bool success)
		{
			var last = nodes[nodes.Count - 1];
			nodes.RemoveAt(nodes.Count - 1);
			if (!success)
			{
				Scanner.SetPosition(last.Position);
				last.Matches.Clear();
				reusableMatches.Push(last.Matches);
			}
			else if (nodes.Count > 0)
				nodes[nodes.Count - 1].Matches.Add(match);
		}
	}

	struct ParseNode
	{
		NamedMatchCollection matches;
		Parser parser;
		int position;

		public NamedMatchCollection Matches { get { return matches; } }

		public Parser Parser { get { return parser; } }

		public int Position { get { return position; } }

		public ParseNode(Parser parser, int position, NamedMatchCollection matches)
		{
			this.parser = parser;
			this.position = position;
			this.matches = matches;
		}

	}
}
