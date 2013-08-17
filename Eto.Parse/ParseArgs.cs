using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Eto.Parse
{
	public class ParseArgs : EventArgs
	{
		SlimStack<MatchCollection> nodes = new SlimStack<MatchCollection>(50);
		SlimStack<MatchCollection> reusableMatches = new SlimStack<MatchCollection>(10);
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

		MatchCollection CreateTempMatchCollection()
		{
			if (reusableMatches.Count > 0)
				return reusableMatches.Pop();
			else
				return new MatchCollection();
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

		public void Push()
		{
			nodes.PushDefault();
		}

		public MatchCollection Pop()
		{
			return nodes.Pop();
		}

		public void PopSuccess()
		{
			var last = nodes.Pop();
			if (last != null)
			{
				var node = nodes.Last;
				if (node != null)
				{
					node.AddRange(last);
					last.Clear();
					reusableMatches.Push(last);
				}
				else
				{
					nodes.Last = last;
				}
			}
		}

		public void PopFailed()
		{
			var last = nodes.Pop();
			if (last != null)
			{
				last.Clear();
				reusableMatches.Push(last);
			}
		}

		public void PopMatch(Parser parser, ParseMatch match)
		{
			// always successful here, assume at least two or more nodes
			var last = nodes.Pop();
			var namedMatch = new Match(parser, Scanner, match.Index, match.Length, last);

			if (nodes.Count > 0)
			{
				var node = nodes.Last;
				if (node == null)
				{
					node = CreateTempMatchCollection();
					nodes.Last = node;
				}
				node.Add(namedMatch);
			}
		}
	}
}
