using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ParseArgs : EventArgs
	{
		LinkedList<ParseNode> nodes = new LinkedList<ParseNode>();

		public Scanner Scanner { get; private set; }

		public NamedMatchCollection Matches
		{
			get { 
				var last = nodes.Last;
				return (last != null) ? last.Value.Matches : null;
			}
		}

		public ParseArgs(Scanner scanner)
		{
			Scanner = scanner;
		}

		public ParseMatch Match(long offset, int length)
		{
			return new ParseMatch(Scanner, offset, length);
		}

		public ParseMatch EmptyMatch
		{
			get { return Scanner.EmptyMatch; }
		}

		public ParseMatch Error { get; set; }

		public bool Push(Parser parser, NamedMatchCollection matches = null)
		{
			if (IsRecursive(parser))
				return false;
			nodes.AddLast(new ParseNode(parser, Scanner.Offset, matches ?? Matches ?? new NamedMatchCollection()));
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
				if (parseNode.Offset < Scanner.Offset)
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
				Scanner.Offset = last.Offset;

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
		long offset;

		public NamedMatchCollection Matches { get { return matches; } }

		public Parser Parser { get { return parser; } }

		public long Offset { get { return offset; } }

		public ParseNode(Parser parser, long offset, NamedMatchCollection matches)
		{
			this.parser = parser;
			this.offset = offset;
			this.matches = matches;
		}
	}
}
