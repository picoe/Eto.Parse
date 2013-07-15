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

		public void AddError(Parser parser, int pos)
		{
			if (pos > ErrorIndex)
			{
				ErrorIndex = pos;
				errors.Clear();
			}
			errors.Add(parser);
		}

		ParseNode Last
		{
			get { return nodes[nodes.Count - 1]; }
		}

		void RemoveLast()
		{
			nodes.RemoveAt(nodes.Count - 1);
		}

		public bool IsRecursive(Parser parser)
		{
			var recursePos = nodes.Count - 2;
			if (recursePos < 0)
				return false;

			var pos = Scanner.Position;
			for (int i = recursePos; i >= 0; i--)
			{
				var parseNode = nodes[i];
				if (parseNode.Position < pos)
					return false;
				if (object.ReferenceEquals(parseNode.Parser, parser))
				{
					// check to see if we have recursed through the same path already
					// going through different paths are okay!
					// e.g. A->B->A->C->B[->A]
					var count = nodes.Count - i;
					if (i < count) // if we have enough to test for recursion to this path
						return false;

					var prevPos = i;
					for (int j = 0; j < count; j ++)
					{
						var recurseNode = nodes[recursePos--];
						var prevNode = nodes[prevPos--];
						if (!object.ReferenceEquals(prevNode.Parser, recurseNode.Parser)
						    || prevNode.Position < recurseNode.Position)
							return false;
					}
					return true;
				}
			}
			return false;
		}

		public NamedMatchCollection Push(Parser parser, bool newMatches = false)
		{
			NamedMatchCollection matches;
			if (newMatches || nodes.Count == 0)
				matches = CreateTempMatchCollection();
			else
				matches = Last.Matches;

			nodes.Insert(nodes.Count, new ParseNode(parser, Scanner.Position, matches));
			return matches;
		}

		public void Pop(bool success)
		{
			var last = Last;
			RemoveLast();

			if (!success)
				Scanner.SetPosition(last.Position);
		}

		public void Pop(NamedMatch match, bool success)
		{
			//match.ThrowIfNull("match");
			if (nodes.Count > 0)
			{
				var last = Last;
				RemoveLast();
				if (success)
				{
					if (nodes.Count > 0)
						Last.Matches.Add(match);
				}
				else
				{
					Scanner.SetPosition(last.Position);
					last.Matches.Clear();
					reusableMatches.Push(last.Matches);
				}
			}
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
