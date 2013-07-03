using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ParseArgs : EventArgs
	{
		LinkedList<ParseNode> nodes = new LinkedList<ParseNode>();

		public IScanner Scanner { get; private set; }

		public NamedMatchCollection Matches
		{
			get { 
				var last = nodes.Last;
				return (last != null) ? last.Value.Matches : null;
			}
		}

		public long NodePosition
		{
			get { 
				var last = nodes.Last;
				return (last != null) ? last.Value.Position : -1;
			}
		}

		public ParseArgs(IScanner scanner)
		{
			Scanner = scanner;
		}

		public ParseMatch Match(long offset, int length)
		{
			return new ParseMatch(Scanner, offset, length);
		}

		public ParseMatch EmptyMatch
		{
			get { return new ParseMatch(Scanner, Scanner.Position, 0); }
		}

		public ParseError Error { get; set; }

		public bool Push(Parser parser, NamedMatchCollection matches = null)
		{
			if (IsRecursive(parser))
				return false;
			nodes.AddLast(new ParseNode(parser, Scanner.Position, matches ?? Matches ?? new NamedMatchCollection()));
			return true;
		}

		bool IsRecursive(Parser parser)
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

		public NamedMatchCollection Pop(bool success)
		{
			var last = nodes.Last.Value;
			nodes.RemoveLast();

			if (!success)
				Scanner.Position = last.Position;

			return last.Matches;
		}

		public void Pop()
		{
			nodes.RemoveLast();
		}
	}

	struct ParseNode
	{
		NamedMatchCollection matches;
		Parser parser;
		long position;

		public NamedMatchCollection Matches { get { return matches; } }

		public Parser Parser { get { return parser; } }

		public long Position { get { return position; } }

		public ParseNode(Parser parser, long position, NamedMatchCollection matches)
		{
			this.parser = parser;
			this.position = position;
			this.matches = matches;
		}
	}
}
