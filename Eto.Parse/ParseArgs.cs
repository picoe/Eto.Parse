using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ParseArgs : EventArgs
	{
		ParseError childError;
		List<ParseNode> nodes = new List<ParseNode>(100);
		Stack<NamedMatchCollection> reusableMatches = new Stack<NamedMatchCollection>();

		public NamedMatch Top { get; private set; }

		public IScanner Scanner { get; private set; }

		public ParseArgs(IScanner scanner)
		{
			Scanner = scanner;
		}

		public ParseMatch Match(int offset, int length)
		{
			return new ParseMatch(offset, length);
		}

		public ParseMatch EmptyMatch
		{
			get { return new ParseMatch(Scanner.Position, 0); }
		}

		public ParseMatch NoMatch
		{
			get { return new ParseMatch(-1, -1); }
		}

		public void Push(Parser parser, NamedMatch match = null)
		{
			if (match != null)
				nodes.Insert(nodes.Count, new ParseNode(parser, Scanner.Position, match, CreateTempMatchCollection()));
			else
			{
				var current = Last();
				nodes.Insert(nodes.Count, new ParseNode(parser, Scanner.Position, current.Match, current.Matches));
			}
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
			if (nodes.Count > 0)
			{
				var node = Last();
				var match = node.Match;
				var pos = node.Position;
				var error = match.Error;
				if (error == null)
					error = match.Error = new ParseError(Scanner, pos);
				else if (pos > error.Index)
					error.Reset(pos);
				if (pos == error.Index)
					error.AddError(parser, childError);
			}
		}

		ParseNode Last()
		{
			return nodes[nodes.Count - 1];
		}

		void RemoveLast()
		{
			nodes.RemoveAt(nodes.Count - 1);
		}

		public bool IsRecursive(Parser parser)
		{
			if (nodes.Count <= 1)
				return false;

			for (int i = nodes.Count - 2; i >= 0; i--)
			{
				var parseNode = nodes[i];
				if (parseNode.Position < Scanner.Position)
					return false;
				if (object.ReferenceEquals(parseNode.Parser, parser))
				{
					// check to see if we have recursed through the same path already
					// going through different paths are okay!
					// e.g. A->B->A->C->B[->A]
					var count = Math.Min(i, nodes.Count - i);
					var recursePos = nodes.Count - 2;
					var prevPos = i;
					for (int j = 0; j < count; j ++)
					{
						var recurseNode = nodes[recursePos];
						var prevNode = nodes[prevPos];
						if (prevNode.Parser != recurseNode.Parser) return false;
						prevPos--;
						recursePos--;
					}

					return true;
				}
			}
			return false;
		}

		public void Pop(bool success)
		{
			var last = Last();
			RemoveLast();

			if (!success)
				Scanner.Position = last.Position;
		}

		public bool Pop(NamedMatch match, bool success)
		{
			//match.ThrowIfNull("match");
			var last = Last();
			var lastMatches = last.Matches;
			RemoveLast();
			if (success)
			{
				match.Matches.AddRange(lastMatches);
				if (nodes.Count > 0)
					Last().Matches.Add(match);
				childError = null;
			}
			else
			{
				Scanner.Position = last.Position;
				childError = match.Error;
			}
			lastMatches.Clear();
			reusableMatches.Push(lastMatches);
			if (nodes.Count == 0)
				Top = match;
			return nodes.Count > 0;
		}
	}

	struct ParseNode
	{
		NamedMatchCollection matches;
		NamedMatch match;
		Parser parser;
		int position;

		public NamedMatchCollection Matches { get { return matches; } }

		public NamedMatch Match { get { return match; } }

		public Parser Parser { get { return parser; } }

		public int Position { get { return position; } }

		public ParseNode(Parser parser, int position, NamedMatch match, NamedMatchCollection matches)
		{
			this.parser = parser;
			this.position = position;
			this.match = match;
			this.matches = matches;
		}
	}
}
