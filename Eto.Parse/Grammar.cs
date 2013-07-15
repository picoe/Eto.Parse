using System;
using Eto.Parse.Scanners;

namespace Eto.Parse
{
	public class Grammar : NamedParser
	{
		/// <summary>
		/// Gets or sets a value indicating that the match events will be triggered after a successful match
		/// </summary>
		/// <value></value>
		public bool EnableMatchEvents { get; set; }

		public Parser Separator { get; set; }

		public bool CaseSensitive { get; set; }

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

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			if (args.IsRoot)
			{
				var matches = args.Push(this, true);
				var match = (Inner != null) ? Inner.Parse(args) : args.EmptyMatch;
				args.Root = new GrammarMatch(this, args.Scanner, match.Index, match.Length, matches, args.ErrorIndex, args.Errors);
				args.Pop(match.Success);
				if (match.Success)
					return match;
				else
					return args.NoMatch;
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
			Parse(args);
			var root = args.Root;

			if (root.Success && EnableMatchEvents)
			{
				root.PreMatch();
				root.Match();
			}
			return root;
		}

		public NamedMatchCollection Matches(string value)
		{
			value.ThrowIfNull("value");
			return Matches(new StringScanner(value));
		}

		public NamedMatchCollection Matches(Scanner scanner)
		{
			scanner.ThrowIfNull("scanner");
			var matches = new NamedMatchCollection();
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

