using System;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class ParseArgs : EventArgs
	{
		ParseNodeList nodes;

		public Scanner Scanner { get; private set; }

		public Parser Parser
		{
			get { return (nodes.Last != null) ? nodes.Last.Value.Parser : null; }
		}
		
		public NamedMatchCollection Matches
		{
			get { return (nodes.Last != null) ? nodes.Last.Value.Matches : null; }
		}
		
		public ParseArgs(Scanner scanner)
		{
			nodes = new ParseNodeList();
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
		
		public ParseMatch NoMatch
		{
			get { return Scanner.NoMatch(Scanner.Offset); }
		}

		public ParseMatch Error { get; set; }

		public bool Push(Parser parser)
		{
			//for (int i=0; i< nodes.Count; i++) Console.Write("  ");
			//if (parser != null) Console.WriteLine("push parser {0}", parser.GetType().Name);
			//else Console.WriteLine("Push top node");
			if (IsRecursive(parser)) return false;
			nodes.AddLast(new ParseNode(parser, Scanner.Offset));
			return true;
		}

		public bool Push(NamedMatch namedMatch)
		{
			//for (int i=0; i< nodes.Count; i++) Console.Write("  ");
			//Console.WriteLine("push rule {0}", ruleMatch.Rule.Id);
			if (IsRecursive(namedMatch.Parser)) return false;
			nodes.AddLast(new ParseNode(namedMatch.Parser, Scanner.Offset, namedMatch.Matches));
			return true;
		}
		
		private bool IsRecursive(Parser parser)
		{
			LinkedListNode<ParseNode> node = nodes.Last;
			if (node != null) node = node.Previous;
			
			while (node != null)
			{
				ParseNode parseNode = node.Value;
				if (parseNode.Offset < Scanner.Offset) break;
				if (parseNode.Parser == parser)
				{
					// add current nodes to a new list and check to see if that is duplicated!
					ParseNodeList recurseList = new ParseNodeList(node);
					LinkedListNode<ParseNode> recurseNode = recurseList.Last;
					node = node.Previous;
					while (recurseNode != null && node != null)
					{
						if (node.Value.Parser != recurseNode.Value.Parser) return false;
						node = node.Previous;
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
			ParseNode last = nodes.Last.Value;
			nodes.RemoveLast();
			
			if (success)
			{
				if (nodes.Last != null) nodes.Last.Value.Matches.AddRange(last.Matches);
			}
			else Scanner.Offset = last.Offset;
			
			return last.Matches;
		}

		public void Pop()
		{
			nodes.RemoveLast();
		}
	}
	
	public struct ParseNode
	{
		NamedMatchCollection matches;
		Parser parser;
		long offset;

		public NamedMatchCollection Matches { get { return matches; } }

		public Parser Parser { get { return parser; } }

		public long Offset { get { return offset; } }

		public ParseNode(Parser parser, long offset, NamedMatchCollection matches = null)
		{
			this.parser = parser;
			this.offset = offset;
			this.matches = matches ?? new NamedMatchCollection();
		}
	}

	public class ParseNodeList : LinkedList<ParseNode>
	{
		public ParseNodeList()
		{
		}
		
		public ParseNodeList(LinkedListNode<ParseNode> node)
		{
			while (node != null)
			{
				AddLast(node.Value);
				node = node.Next;
			}
		}
		
	}
}
