using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Eto.Parse
{
	/// <summary>
	/// Arguments for parsing
	/// </summary>
	/// <remarks>
	/// This is used during the parsing process to track the current match tree, errors, scanner, etc.
	/// </remarks>
	public class ParseArgs
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

		/// <summary>
		/// Creates an empty (zero length) match at the current position
		/// </summary>
		/// <value>A new instance of an empty match</value>
		public ParseMatch EmptyMatch
		{
			get { return new ParseMatch(Scanner.Position, 0); }
		}

		/// <summary>
		/// Creates a non-match when a parser fails to match
		/// </summary>
		/// <value>The non-match</value>
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

		/// <summary>
		/// Adds an error for the specified parser at the current position
		/// </summary>
		/// <param name="parser">Parser to add the error for</param>
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

		/// <summary>
		/// Pushes a new match tree node
		/// </summary>
		/// <remarks>
		/// Use this when there is a possibility that a child parser will not match, such as the <see cref="OptionalParser"/>,
		/// items in a <see cref="AlternativeParser"/>
		/// </remarks>
		public void Push()
		{
			nodes.PushDefault();
		}

		/// <summary>
		/// Pops the last match tree node, and returns its value
		/// </summary>
		/// <remarks>
		/// Use <see cref="PopSuccess"/> or <see cref="PopFailed"/> when implementing parsers.
		/// This does not perform any logic like merging the match tree with the parent node when succesful,
		/// nor does it allow for re-use of the match collection for added performance.
		/// </remarks>
		public MatchCollection Pop()
		{
			return nodes.Pop();
		}

		/// <summary>
		/// When an optional match is successful, this pops the current match tree node and merges it with the
		/// parent match tree node.
		/// </summary>
		/// <remarks>
		/// This call must be proceeded with a call to <see cref="Push"/> to push a match tree node.
		/// </remarks>
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

		/// <summary>
		/// When an optional match did not succeed, this pops the current match tree node and prepares it for re-use.
		/// </summary>
		/// <remarks>
		/// This call must be proceeded with a call to <see cref="Push"/> to push a match tree node.
		/// </remarks>
		public void PopFailed()
		{
			var last = nodes.Pop();
			if (last != null)
			{
				last.Clear();
				reusableMatches.Push(last);
			}
		}

		/// <summary>
		/// Pops a succesful named match
		/// </summary>
		/// <param name="parser">Parser with the name to add to the match tree</param>
		/// <param name="match">Match to add to the match tree</param>
		public void PopMatch(Parser parser, ParseMatch match, string name)
		{
			// always successful here, assume at least two or more nodes
			var last = nodes.Pop();
			var namedMatch = new Match(name, parser, Scanner, match, last);

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
