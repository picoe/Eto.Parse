using System;
using Eto.Parse.Scanners;
using System.Linq;

namespace Eto.Parse
{
	public class Grammar : UnaryParser
	{
		bool initialized;

		/// <summary>
		/// Gets or sets a value indicating that the match events will be triggered after a successful match
		/// </summary>
		/// <value></value>
		public bool EnableMatchEvents { get; set; }

		/// <summary>
		/// Gets or sets the separator to use for <see cref="RepeatParser"/> and <see cref="SequenceParser"/> if not explicitly defined.
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

		protected Grammar(Grammar other)
		{
			this.EnableMatchEvents = other.EnableMatchEvents;
			this.Separator = other.Separator != null ? other.Separator.Clone() : null;
			this.CaseSensitive = other.CaseSensitive;
		}

		public Grammar(string name = null, Parser rule = null)
			: base(name, rule)
		{
			CaseSensitive = true;
			EnableMatchEvents = true;
		}

		public Grammar(Parser rule)
			: this(null, rule)
		{
		}

		public void Initialize()
		{
			Initialize(new ParserInitializeArgs(this));
			initialized = true;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (args.IsRoot)
			{
				var scanner = args.Scanner;
				var pos = scanner.Position;
				args.Push();
				var match = (Inner != null) ? Inner.Parse(args) : args.EmptyMatch;
				MatchCollection matches = null;
				if (match.Success && !AllowPartialMatch && !scanner.IsEof)
				{
					scanner.SetPosition(pos);
					match = args.NoMatch;
				}
				else
				{
					matches = args.Pop();
				}
				args.Root = new GrammarMatch(this, scanner, match.Index, match.Length, matches, args.ErrorIndex, args.Errors.Distinct().ToArray());
				return match;
			}
			else
				return base.InnerParse(args);
		}

		public GrammarMatch Match(string value)
		{
			value.ThrowIfNull("value");
			return Match(new StringScanner(value));
		}

		public GrammarMatch Match(Scanner scanner)
		{
			scanner.ThrowIfNull("scanner");
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
			while (!scanner.IsEof)
			{
				var match = Match(scanner);
				if (match.Success)
					matches.Add(match);
				else
					scanner.Advance(1);
			}
			return matches;
		}
	}
}

