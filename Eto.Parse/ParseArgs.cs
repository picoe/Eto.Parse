using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ParseArgs : EventArgs
	{
		ParseError childError;
		LinkedList<ParseNode> nodes = new LinkedList<ParseNode>();
		LinkedListNode<ParseNode> currentNode;

		public IScanner Scanner { get; private set; }

		public ParseError ChildError
		{
			get { return childError; }
		}

		public ParseArgs(IScanner scanner)
		{
			Scanner = scanner;
		}

		public ParseMatch Match(long offset, int length)
		{
			return new ParseMatch(offset, length);
		}

		public ParseMatch EmptyMatch
		{
			get { return new ParseMatch(Scanner.Position, 0); }
		}

		public void Push(Parser parser, NonTerminalMatch match = null)
		{
			if (match != null)
				currentNode = nodes.AddLast(new ParseNode(parser, Scanner.Position, match, new NonTerminalMatchCollection()));
			else
				currentNode = nodes.AddLast(new ParseNode(parser, Scanner.Position, currentNode.Value.Match, currentNode.Value.Matches));
		}

		public void AddError(Parser parser)
		{
			var node = currentNode;
			if (node != null)
			{
				var match = node.Value.Match;
				var pos = node.Value.Position;
				var error = match.Error;
				if (error == null)
					error = match.Error = new ParseError(Scanner, pos);
				else if (pos > error.Index)
					error.Reset(pos);
				if (pos == error.Index)
					error.AddError(parser, childError);
			}
		}

		public bool IsRecursive(Parser parser)
		{
			LinkedListNode<ParseNode> node = nodes.Last;
			if (node != null)
				node = node.Previous;
			
			while (node != null)
			{
				ParseNode parseNode = node.Value;
				if (parseNode.Position < Scanner.Position)
					return false;
				if (object.ReferenceEquals(parseNode.Parser, parser))
				{
					// check to see if we have recursed through the same path already
					// going through different paths are okay!
					// e.g. A->B->A->C->B[->A]
					var recurseNode = nodes.Last;
					var prevNode = node.Previous;
					while (recurseNode != null && prevNode != null && recurseNode != node)
					{
						if (prevNode.Value.Parser != recurseNode.Value.Parser) return false;
						prevNode = prevNode.Previous;
						recurseNode = recurseNode.Previous;
					}
					
					return true;
				}
				node = node.Previous;
			}
			return false;
		}

		public void Pop(bool success)
		{
			var last = nodes.Last;
			currentNode = last.Previous;
			nodes.RemoveLast();

			if (!success)
				Scanner.Position = last.Value.Position;
		}

		public bool Pop(NonTerminalMatch match, bool success)
		{
			//match.ThrowIfNull("match");
			var last = nodes.Last;
			currentNode = last.Previous;
			nodes.RemoveLast();
			if (success)
			{
				match.Matches.AddRange(last.Value.Matches);
				var current = nodes.Last;
				if (current != null)
					current.Value.Matches.Add(match);
				childError = null;
			}
			else
			{
				Scanner.Position = last.Value.Position;
				childError = match.Error;
			}
			return currentNode != null;
		}
	}

	struct ParseNode
	{
		NonTerminalMatchCollection matches;
		NonTerminalMatch match;
		Parser parser;
		long position;

		public NonTerminalMatchCollection Matches { get { return matches; } }

		public NonTerminalMatch Match { get { return match; } }

		public Parser Parser { get { return parser; } }

		public long Position { get { return position; } }

		public ParseNode(Parser parser, long position, NonTerminalMatch match, NonTerminalMatchCollection matches)
		{
			this.parser = parser;
			this.position = position;
			this.match = match;
			this.matches = matches;
		}
	}
}
