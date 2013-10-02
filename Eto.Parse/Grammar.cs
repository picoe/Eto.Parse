using System;
using Eto.Parse.Scanners;
using System.Collections.Generic;

namespace Eto.Parse
{
	/// <summary>
	/// Defines the top level parser (a grammar) used to parse text
	/// </summary>
	public class Grammar : UnaryParser
	{
		bool initialized;

		/// <summary>
		/// Gets or sets a value indicating that the match events will be triggered after a successful match
		/// </summary>
		/// <value></value>
		public bool EnableMatchEvents { get; set; }

		/// <summary>
		/// Gets or sets the separator to use for <see cref="Parsers.RepeatParser"/> and <see cref="Parsers.SequenceParser"/> if not explicitly defined.
		/// </summary>
		/// <value>The separator to use inbetween repeats and items of a sequence</value>
		public Parser Separator { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this grammar is case sensitive or not
		/// </summary>
		/// <value><c>true</c> if case sensitive; otherwise, <c>false</c>.</value>
		public bool CaseSensitive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a partial match of the input scanner is allowed
		/// </summary>
		/// <value><c>true</c> to allow a successful match if partially matched; otherwise, <c>false</c> to indicate that the entire input must be consumed to match.</value>
		public bool AllowPartialMatch { get; set; }

		public bool Trace { get; set; }

		/// <summary>
		/// Initializes a new copy of the <see cref="Eto.Parse.Grammar"/> class
		/// </summary>
		/// <param name="other">Other object to copy</param>
		/// <param name="args">Arguments for the copy</param>
		protected Grammar(Grammar other, ParserCloneArgs args)
		{
			this.EnableMatchEvents = other.EnableMatchEvents;
			this.Separator = args.Clone(other.Separator);
			this.CaseSensitive = other.CaseSensitive;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Parse.Grammar"/> class
		/// </summary>
		/// <param name="name">Name of the grammar</param>
		/// <param name="rule">Top level grammar rule</param>
		public Grammar(string name = null, Parser rule = null)
			: base(name, rule)
		{
			CaseSensitive = true;
			EnableMatchEvents = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Parse.Grammar"/> class
		/// </summary>
		/// <param name="rule">Top level grammar rule</param>
		public Grammar(Parser rule)
			: this(null, rule)
		{
		}

		/// <summary>
		/// Initializes this instance for parsing
		/// </summary>
		/// <remarks>
		/// Initialization (usually) occurs only once, and should only be called after
		/// the grammar is fully defined. This will be called automatically the first
		/// time you call the <see cref="Match(string)"/> method.
		/// </remarks>
		protected void Initialize()
		{
			Initialize(new ParserInitializeArgs(this));
			initialized = true;
		}

		protected override int InnerParse(ParseArgs args)
		{
			if (args.IsRoot)
			{
				var scanner = args.Scanner;
				var pos = scanner.Position;
				args.Push();
				var match = Inner.Parse(args);
				MatchCollection matches = null;
				if (match >= 0 && !AllowPartialMatch && !scanner.IsEof)
				{
					args.PopFailed();
					scanner.Position = pos;
					match = -1;
				}
				else
				{
					matches = args.Pop();
				}

				IEnumerable<Parser> errors = null;
				if (args.Errors != null)
				{
					var errorList = new List<Parser>(args.Errors.Count);
					for (int i = 0; i < args.Errors.Count; i++)
					{
						var error = args.Errors[i];
						if (!errorList.Contains(error))
							errorList.Add(error);
					}
					errors = errorList;
				}

				args.Root = new GrammarMatch(this, scanner, pos, match, matches, args.ErrorIndex, args.ChildErrorIndex, errors);
				return match;
			}
			return base.InnerParse(args);
		}

		public GrammarMatch Match(string value)
		{
			//value.ThrowIfNull("value");
			return Match(new StringScanner(value));
		}

		public GrammarMatch Match(Scanner scanner)
		{
			//scanner.ThrowIfNull("scanner");
			var args = new ParseArgs(this, scanner);

			if (!initialized)
				Initialize();
			Parse(args);
			var root = args.Root;

			if (root.Success && EnableMatchEvents)
			{
				root.TriggerPreMatch();
				root.TriggerMatch();
			}
			return root;
		}

		public MatchCollection Matches(string value)
		{
			value.ThrowIfNull("value");
			return Matches(new StringScanner(value));
		}

		public MatchCollection Matches(Scanner scanner)
		{
			scanner.ThrowIfNull("scanner");
			var matches = new MatchCollection();
			var eof = scanner.IsEof;
			while (!eof)
			{
				var match = Match(scanner);
				if (match.Success)
				{
					matches.AddRange(match.Matches);
					eof = scanner.IsEof;
				}
				else
					eof = scanner.Advance(1) < 0;
			}
			return matches;
		}

		public override Parser Clone(ParserCloneArgs args)
		{
			return new Grammar(this, args);
		}
	}
}

